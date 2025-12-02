using QLNS.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLNS
{
    public partial class QLNS : Form
    {
        public QLNS()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDN = txtDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim(); // Trong thực tế, bạn cần mã hóa mật khẩu này

            if (string.IsNullOrEmpty(tenDN) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!");
                return;
            }

            // 1. Khởi tạo DbContext (tên này được đặt ở Bước 1, lưu trong App.config)
            using (var db = new DataContext())
            {
                try
                {
                    // 2. Truy vấn bảng TaiKhoan để tìm người dùng
                    // Chúng ta dùng tài khoản 'binh_thungan' (đã cấu hình) để chạy câu lệnh SQL này
                    var taiKhoan = db.TaiKhoans.FirstOrDefault(tk =>
                                        tk.TenDangNhap == tenDN &&
                                        tk.MatKhau == matKhau);

                    // 3. Xử lý kết quả
                    if (taiKhoan != null)
                    {
                        // Tìm thấy tài khoản
                        if (taiKhoan.TrangThai == false)
                        {
                            MessageBox.Show("Tài khoản của bạn đã bị khóa!");
                        }
                        else
                        {
                            // Đăng nhập thành công
                            MessageBox.Show($"Chào mừng {taiKhoan.VaiTro}: {tenDN}!");

                            // TODO: Mở form chính (MainForm) và đóng form đăng nhập này
                            ChucNang f = new ChucNang();
                            f.Show();
                            // this.Hide();

                        }
                    }
                    else
                    {
                        // Không tìm thấy tài khoản
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
                }
            }
        }
    }
}
