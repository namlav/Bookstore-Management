using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace QLNS.Services
{
    public class DatabaseService
    {
        private readonly string _connString;

        public DatabaseService()
        {
            // Đảm bảo Web.config đã có connectionString tên là "DefaultConnection"
            _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        }

        // ============================================================
        // 1. PHẦN ĐĂNG NHẬP (Login)
        // ============================================================
        public bool Login(string user, string pass, string ip, string dev, string browser, out string msg)
        {
            msg = "";
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    // Gọi Stored Procedure đăng nhập
                    SqlCommand cmd = new SqlCommand("sp_AttemptLogin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TenDangNhap", user);
                    cmd.Parameters.AddWithValue("@MatKhau", pass); // Pass plaintext để SP tự hash và so sánh
                    cmd.Parameters.AddWithValue("@IP", ip);
                    cmd.Parameters.AddWithValue("@ThietBi", dev);
                    cmd.Parameters.AddWithValue("@TrinhDuyet", browser);
                    cmd.ExecuteNonQuery();

                    // Kiểm tra kết quả trong bảng lịch sử
                    SqlCommand check = new SqlCommand("SELECT TOP 1 ThanhCong, GhiChu FROM LichSuDangNhap WHERE TenDangNhap=@u ORDER BY ID DESC", conn);
                    check.Parameters.AddWithValue("@u", user);
                    using (var r = check.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            msg = r["GhiChu"].ToString();
                            return (bool)r["ThanhCong"];
                        }
                    }
                }
                catch (Exception ex) { msg = ex.Message; }
            }
            return false;
        }

        // ============================================================
        // 2. PHẦN QUÊN MẬT KHẨU
        // ============================================================
        public string RequestResetToken(string user)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_RequestPasswordReset", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TenDangNhap", user);
                    SqlParameter outP = new SqlParameter("@OutToken", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outP);
                    cmd.ExecuteNonQuery();
                    return outP.Value.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public string ResetPassword(string user, string token, string newpass)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_ResetPasswordWithToken", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TenDangNhap", user);
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@MatKhauMoi", newpass);
                    cmd.ExecuteNonQuery();
                    return "Success";
                }
                catch (Exception ex) { return ex.Message; }
            }
        }

        // ============================================================
        // 3. PHẦN QUẢN LÝ USER (Create / Update / Get)
        // ============================================================

        // [QUAN TRỌNG] Hàm tạo user đã được sửa để tránh lỗi DB
        public string CreateUser(string username, string password, string fullname, string role)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    // SỬA ĐỔI: Thêm cột MatKhau vào INSERT. 
                    // Lý do: Nếu bảng TaiKhoan của bạn cột MatKhau set là "NOT NULL", code cũ chỉ insert MatKhauHash sẽ bị lỗi.
                    string sql = @"
                        INSERT INTO dbo.TaiKhoan 
                        (TenDangNhap, MatKhau, MatKhauHash, HoTen, VaiTro, FailedLoginCount)
                        VALUES 
                        (@User, @Pass, HASHBYTES('SHA2_256', @Pass), @Name, @Role, 0)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@User", username);
                        cmd.Parameters.AddWithValue("@Pass", password); // Lưu cả pass thường (nếu cần) và dùng nó để tạo Hash
                        cmd.Parameters.AddWithValue("@Name", fullname ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Role", role);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "OK"; // Thành công
            }
            catch (SqlException ex)
            {
                // Bắt lỗi trùng tên đăng nhập (Mã lỗi SQL Server: 2627 hoặc 2601)
                if (ex.Number == 2627 || ex.Number == 2601)
                    return "Tên tài khoản này đã tồn tại! Vui lòng chọn tên khác.";

                return "Lỗi SQL: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Lỗi hệ thống: " + ex.Message;
            }
        }

        public bool UpdateUser(string username, string fullname, string role)
        {
            using (var con = new SqlConnection(_connString))
            {
                try
                {
                    con.Open();
                    // Đã khớp với logic HoTen
                    string query = "UPDATE TaiKhoan SET HoTen=@n, VaiTro=@r WHERE TenDangNhap=@u";
                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@n", fullname ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@r", role);
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                catch { return false; }
            }
        }

        public User GetUserByUsername(string username)
        {
            using (var con = new SqlConnection(_connString))
            {
                con.Open();
                string query = "SELECT TenDangNhap, HoTen, VaiTro, MaNV FROM TaiKhoan WHERE TenDangNhap=@u";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            return new User
                            {
                                Username = r["TenDangNhap"].ToString(),
                                // Kiểm tra DBNull an toàn
                                FullName = r["HoTen"] != DBNull.Value ? r["HoTen"].ToString() : "",
                                Role = r["VaiTro"] != DBNull.Value ? r["VaiTro"].ToString() : "",
                                MaNV = r["MaNV"] != DBNull.Value ? r["MaNV"].ToString() : ""
                            };
                        }
                    }
                }
            }
            return null;
        }

        public string GetRoleByUsername(string username)
        {
            var user = GetUserByUsername(username);
            return user?.Role;
        }

        // ============================================================
        // 4. CÁC TÍNH NĂNG NÂNG CAO
        // ============================================================

        public DataTable GetUsers()
        {
            // Lấy danh sách hiển thị ra bảng
            return GetTable(@"SELECT TenDangNhap, HoTen, VaiTro, MaNV, 
                                     FailedLoginCount, LockedUntil, LastLogin, LastLoginIP 
                              FROM TaiKhoan");
        }

        public DataTable GetHistory() => GetTable("SELECT TOP 20 * FROM LichSuDangNhap ORDER BY ID DESC");

        public DataTable CheckCursor() => GetTable("EXEC sp_CheckPasswordHealth");

        public DataTable GetAuditLogs() => GetTable("SELECT TOP 50 * FROM TaiKhoanAudit ORDER BY AuditID DESC");

        public string DeleteUserSafe(string username, bool rollback)
        {
            using (var con = new SqlConnection(_connString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_DeleteUserSafe";
                    cmd.Parameters.AddWithValue("@TenDangNhap", username);
                    cmd.Parameters.AddWithValue("@SimulateError", rollback ? 1 : 0);

                    try
                    {
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read()) return rd["Msg"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        return "Lỗi khi xóa: " + ex.Message;
                    }
                }
            }
            return "Lỗi không xác định.";
        }

        public void Unlock(string u)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_UnlockAccount", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TenDangNhap", u);
                cmd.ExecuteNonQuery();
            }
        }

        // Hàm helper chạy query trả về DataTable
        private DataTable GetTable(string q)
        {
            DataTable dt = new DataTable();
            using (SqlConnection c = new SqlConnection(_connString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(q, c))
                    da.Fill(dt);
            }
            return dt;
        }
    }

    // Class Model đơn giản để map dữ liệu
    public class User
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string MaNV { get; set; }
    }
}