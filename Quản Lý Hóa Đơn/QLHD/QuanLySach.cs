using QLNS.Database;
// using QLNS.Database.DTOs; // Không cần DTO nếu binding trực tiếp
using QLNS.Database.Entities; // <-- Cần using Entity 'Sach'
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLNS
{
    public partial class QuanLySach : Form
    {
        // --- CÁC CONTROL TRÊN FORM (GIỮ NGUYÊN) ---
        // TextBox: txtMasach, txtTensach, txtMaTacgia, txtMaTheLoai, txtMaNSX, txtNamxuatban
        // NumericUpDown: numGiaBia, numTon
        // Button: btnThem, btnSua, btnXoa
        // DataGridView: dataGridView1

        public QuanLySach()
        {
            InitializeComponent();
        }

        #region Load & Helper Functions

        private void QuanLySach_Load(object sender, EventArgs e)
        {
            LoadData();

            // Gán sự kiện CellClick cho DataGridView
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
        }

        // Tải dữ liệu lên DataGridView (ĐÃ SỬA)
        private void LoadData()
        {
            using (var db = new DataContext()) // Tên DbContext của bạn
            {
                try
                {
                    // SỬA LẠI: Dùng 'db.sachs' (viết thường)
                    var data = db.Saches
                                 .Select(s => new
                                 {
                                     // Dùng đúng tên thuộc tính của Entity
                                     MaSach = s.MaSach,
                                     TenSach = s.TenSach,
                                     MaTG = s.MaTG,
                                     MaTL = s.MaTL,
                                     MaNXB = s.MaNXB,
                                     NamXuatBan = s.NamXuatBan,
                                     GiaBia = s.GiaBia,
                                     SoLuongTon = s.SoLuongTon
                                 }).ToList();

                    if (data.Any())
                    {
                        dataGridView1.DataSource = data;

                        // (Tùy chọn) Cài đặt lại tên cột cho đẹp
                        dataGridView1.Columns["MaSach"].HeaderText = "Mã Sách";
                        dataGridView1.Columns["TenSach"].HeaderText = "Tên Sách";
                        dataGridView1.Columns["MaTG"].HeaderText = "Mã Tác Giả";
                        dataGridView1.Columns["MaTL"].HeaderText = "Mã Thể Loại";
                        dataGridView1.Columns["MaNXB"].HeaderText = "Mã NXB";
                        dataGridView1.Columns["NamXuatBan"].HeaderText = "Năm XB";
                        dataGridView1.Columns["GiaBia"].HeaderText = "Giá Bìa";
                        dataGridView1.Columns["SoLuongTon"].HeaderText = "SL Tồn";
                    }
                    else
                    {
                        dataGridView1.DataSource = null;
                        MessageBox.Show("Không có dữ liệu!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối: " + ex.Message);
                }
            }
            // UpdateSttColumn(); 
            ClearControls(); // Xóa trắng các ô nhập liệu
        }

        // Cập nhật cột STT 
        public void UpdateSttColumn()
        {
            int stt = 1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    // Giả sử cột STT là cột có index 0
                    if (dataGridView1.Columns.Count > 0 && dataGridView1.Columns[0].Name == "STT")
                    {
                        row.Cells[0].Value = stt++;
                    }
                }
            }
        }

        // Hàm xóa trắng các control nhập liệu
        private void ClearControls()
        {
            txtMasach.Clear();
            txtTensach.Clear();
            txtMaTacgia.Clear();
            txtMaTheLoai.Clear();
            txtMaNSX.Clear();
            txtNamxuatban.Clear();
            numGiaBia.Value = 0;
            numTon.Value = 0;

            // Quan trọng: Cho phép nhập lại Mã Sách
            txtMasach.ReadOnly = false;
            txtMasach.Focus();
        }

        // Sự kiện khi click vào 1 hàng trong DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                try
                {
                    // Lấy MaSach từ hàng được chọn
                    string maSachClick = dataGridView1.Rows[e.RowIndex].Cells["MaSach"].Value.ToString();

                    // Hiển thị trực tiếp từ GridView lên TextBox
                    txtMasach.Text = maSachClick;
                    txtTensach.Text = dataGridView1.Rows[e.RowIndex].Cells["TenSach"].Value.ToString();
                    txtMaTacgia.Text = dataGridView1.Rows[e.RowIndex].Cells["MaTG"].Value.ToString();
                    txtMaTheLoai.Text = dataGridView1.Rows[e.RowIndex].Cells["MaTL"].Value.ToString();
                    txtMaNSX.Text = dataGridView1.Rows[e.RowIndex].Cells["MaNXB"].Value.ToString();
                    txtNamxuatban.Text = dataGridView1.Rows[e.RowIndex].Cells["NamXuatBan"].Value.ToString();

                    numGiaBia.Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["GiaBia"].Value);
                    numTon.Value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["SoLuongTon"].Value);

                    // Khóa ô Mã Sách lại
                    txtMasach.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn hàng: " + ex.Message);
                    ClearControls();
                }
            }
        }
        #endregion

        #region CRUD Functions

        private void btnThem_Click(object sender, EventArgs e)
        {
            // (Validation giữ nguyên)
            if (string.IsNullOrWhiteSpace(txtMasach.Text) ||
                string.IsNullOrWhiteSpace(txtTensach.Text) ||
                string.IsNullOrWhiteSpace(txtMaTacgia.Text) ||
                string.IsNullOrWhiteSpace(txtMaTheLoai.Text) ||
                string.IsNullOrWhiteSpace(txtMaNSX.Text) ||
                string.IsNullOrWhiteSpace(txtNamxuatban.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường thông tin chữ.");
                return;
            }

            int namXB;
            if (!int.TryParse(txtNamxuatban.Text, out namXB) || namXB < 1900 || namXB > DateTime.Now.Year)
            {
                MessageBox.Show($"Năm xuất bản không hợp lệ (phải từ 1900 đến {DateTime.Now.Year}).");
                return;
            }
            if (numGiaBia.Value <= 0)
            {
                MessageBox.Show("Giá bìa phải lớn hơn 0.");
                return;
            }


            // Tạo newSach với đúng tên thuộc tính
            var newSach = new Sach
            {
                MaSach = txtMasach.Text.Trim(),
                TenSach = txtTensach.Text.Trim(),
                MaTG = txtMaTacgia.Text.Trim(),
                MaTL = txtMaTheLoai.Text.Trim(),
                MaNXB = txtMaNSX.Text.Trim(),
                NamXuatBan = namXB,
                GiaBia = numGiaBia.Value,
                SoLuongTon = (int)numTon.Value
            };

            using (var db = new DataContext())
            {
                try
                {
                    // SỬA LẠI: Dùng 'db.sachs' (viết thường)
                    if (db.Saches.Any(s => s.MaSach == newSach.MaSach))
                    {
                        MessageBox.Show("Mã sách này đã tồn tại.");
                        return;
                    }

                    db.Saches.Add(newSach); // SỬA LẠI: Dùng 'db.sachs' (viết thường)
                    db.SaveChanges();

                    MessageBox.Show("Thêm sách thành công!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    Exception innerEx = ex.InnerException;
                    while (innerEx != null)
                    {
                        errorMessage += "\n\n--> Lỗi bên trong:\n" + innerEx.Message;
                        innerEx = innerEx.InnerException;
                    }
                    MessageBox.Show("Lỗi khi thêm sách (Có thể Mã TG, Mã TL, Mã NXB không tồn tại?):\n" + errorMessage, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtMasach.ReadOnly == false || string.IsNullOrWhiteSpace(txtMasach.Text))
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách từ danh sách bên trên để sửa.");
                return;
            }

            string maSachCanSua = txtMasach.Text;

            // (Validation)
            int namXB;
            if (!int.TryParse(txtNamxuatban.Text, out namXB) || namXB < 1900 || namXB > DateTime.Now.Year)
            {
                MessageBox.Show($"Năm xuất bản không hợp lệ.");
                return;
            }
            if (numGiaBia.Value <= 0)
            {
                MessageBox.Show("Giá bìa phải lớn hơn 0.");
                return;
            }

            using (var db = new DataContext())
            {
                try
                {
                    // SỬA LẠI: Dùng 'db.sachs' (viết thường)
                    var sachCanSua = db.Saches.FirstOrDefault(s => s.MaSach == maSachCanSua);

                    if (sachCanSua != null)
                    {
                        // Gán đúng tên thuộc tính
                        sachCanSua.TenSach = txtTensach.Text.Trim();
                        sachCanSua.MaTG = txtMaTacgia.Text.Trim();
                        sachCanSua.MaTL = txtMaTheLoai.Text.Trim();
                        sachCanSua.MaNXB = txtMaNSX.Text.Trim();
                        sachCanSua.NamXuatBan = namXB;
                        sachCanSua.GiaBia = numGiaBia.Value;
                        sachCanSua.SoLuongTon = (int)numTon.Value;

                        db.SaveChanges();
                        MessageBox.Show("Cập nhật sách thành công!");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sách với mã này để cập nhật.");
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    Exception innerEx = ex.InnerException;
                    while (innerEx != null)
                    {
                        errorMessage += "\n\n--> Lỗi bên trong:\n" + innerEx.Message;
                        innerEx = innerEx.InnerException;
                    }
                    MessageBox.Show("Lỗi khi cập nhật sách (Có thể Mã TG, Mã TL, Mã NXB mới không tồn tại?):\n" + errorMessage, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maSachCanXoa = txtMasach.Text;
            if (string.IsNullOrWhiteSpace(maSachCanXoa) || txtMasach.ReadOnly == false)
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách từ danh sách để xóa.");
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa sách '{txtTensach.Text}' (Mã: {maSachCanXoa}) không?",
                "Xác nhận Xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                using (var db = new DataContext())
                {
                    try
                    {
                        // SỬA LẠI: Dùng 'db.sachs' (viết thường)
                        var sachCanXoa = db.Saches.FirstOrDefault(s => s.MaSach == maSachCanXoa);

                        if (sachCanXoa != null)
                        {
                            db.Saches.Remove(sachCanXoa); 
                            db.SaveChanges();

                            MessageBox.Show("Xóa sách thành công!");
                            LoadData(); // Tải lại lưới
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sách với mã này.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null && ex.InnerException.Message.Contains("REFERENCE constraint"))
                        {
                            MessageBox.Show("Không thể xóa sách này. Sách đã tồn tại trong Hóa Đơn hoặc Phiếu Nhập.", "Lỗi Ràng buộc Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Lỗi khi xóa sách: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        #endregion

        // Nút này bây giờ sẽ gọi hàm ClearControls đã được sửa
        private void btnNhapLai_Click(object sender, EventArgs e)
        {
            ClearControls();
        }
    }
}