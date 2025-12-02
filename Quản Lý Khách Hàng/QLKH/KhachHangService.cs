using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace QLNS
{
    internal class KhachHangService
    {
        private string connectionString;

        public KhachHangService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Không tìm thấy Connection String 'DBConnectionString' trong App.config.");
            }
        }

        private KhachHang CreateKhachHangFromReader(SqlDataReader reader)
        {
            return new KhachHang
            {
                MaKH = reader["MaKH"].ToString(),
                TenKH = reader["TenKH"].ToString(),
                DienThoai = reader["DienThoai"].ToString(),
                Email = reader["Email"].ToString(),
                DiaChi = reader["DiaChi"].ToString()
            };
        }

        // 1. READ
        public List<KhachHang> GetAllKhachHangs()
        {
            List<KhachHang> list = new List<KhachHang>();
            string query = "SELECT * FROM KhachHang";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(CreateKhachHangFromReader(reader));
                }
            }
            return list;
        }

        // 2. CREATE
        public bool AddKhachHang(KhachHang kh)
        {
            if (string.IsNullOrEmpty(kh.MaKH) || string.IsNullOrEmpty(kh.TenKH))
            {
                throw new ArgumentException("Mã và Tên khách hàng không được để trống.");
            }

            string query = "INSERT INTO KhachHang (MaKH, TenKH, DienThoai, Email, DiaChi) VALUES (@MaKH, @TenKH, @DienThoai, @Email, @DiaChi)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKH", kh.MaKH);
                cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                cmd.Parameters.AddWithValue("@DienThoai", kh.DienThoai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", kh.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DiaChi", kh.DiaChi ?? (object)DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 3. UPDATE
        public bool UpdateKhachHang(KhachHang kh)
        {
            if (string.IsNullOrEmpty(kh.MaKH))
            {
                throw new ArgumentException("Mã khách hàng không được để trống khi cập nhật.");
            }

            string query = "UPDATE KhachHang SET TenKH = @TenKH, DienThoai = @DienThoai, Email = @Email, DiaChi = @DiaChi WHERE MaKH = @MaKH";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKH", kh.MaKH);
                cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                cmd.Parameters.AddWithValue("@DienThoai", kh.DienThoai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", kh.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DiaChi", kh.DiaChi ?? (object)DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 4. DELETE
        public bool DeleteKhachHang(string maKH)
        {
            string query = "DELETE FROM KhachHang WHERE MaKH = @MaKH";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKH", maKH);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 5. SEARCH
        public List<KhachHang> SearchKhachHang(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return GetAllKhachHangs();
            }

            List<KhachHang> list = new List<KhachHang>();
            string query = "SELECT * FROM KhachHang WHERE MaKH LIKE @keyword OR TenKH LIKE @keyword";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(CreateKhachHangFromReader(reader));
                }
            }
            return list;
        }
    }
}
