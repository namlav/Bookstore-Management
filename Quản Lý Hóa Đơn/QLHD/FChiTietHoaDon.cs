using QLNS.Database;
using QLNS.Database.Entities;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QLNS
{
    public partial class FChiTietHoaDon : Form
    {
        private string _soHDCanXuLy;
        private bool isCreateMode = false;
        private DataTable dtChiTiet;

        public FChiTietHoaDon(string soHD = null)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(soHD))
            {
                isCreateMode = true;
                this.Text = "Tạo Hóa Đơn Mới";
                _soHDCanXuLy = null;
            }
            else
            {
                isCreateMode = false;
                _soHDCanXuLy = soHD;
                this.Text = "Chi Tiết Hóa Đơn: " + _soHDCanXuLy;
            }
        }

        private void ChiTietHoaDon_Load(object sender, EventArgs e)
        {
            LoadComboBoxNhanVien();
            LoadComboBoxKhachHang();
            LoadComboBoxSach();

            CauHinhDataTableChiTiet();
            dgvChiTietHoaDon.DataSource = dtChiTiet;

            if (isCreateMode)
            {
                SetDefaultsForCreate();
            }
            else
            {
                LoadThongTinChung(_soHDCanXuLy);
                LoadChiTietVaoDataTable(_soHDCanXuLy);
                TinhLaiTongTien();
                SetFormReadOnly(true);
            }
        }

        // ------------------ COMBOBOX ------------------
        private void LoadComboBoxNhanVien()
        {
            using (var db = new DataContext())
            {
                cboMaNV.DataSource = db.NhanViens
                    .Select(nv => new { nv.MaNV, nv.TenNV })
                    .ToList();
                cboMaNV.DisplayMember = "TenNV";
                cboMaNV.ValueMember = "MaNV";
            }
        }

        private void LoadComboBoxKhachHang()
        {
            using (var db = new DataContext())
            {
                cboMaKH.DataSource = db.KhachHangs
                    .Select(kh => new { kh.MaKH, kh.TenKH })
                    .ToList();
                cboMaKH.DisplayMember = "TenKH";
                cboMaKH.ValueMember = "MaKH";
            }
        }

        private void LoadComboBoxSach()
        {
            using (var db = new DataContext())
            {
                cboSach.DataSource = db.Saches
                    .Select(s => new { s.MaSach, s.TenSach, s.GiaBia })
                    .ToList();
                cboSach.DisplayMember = "TenSach";
                cboSach.ValueMember = "MaSach";
            }
        }

        // ------------------ DATA TABLE ------------------
        private void CauHinhDataTableChiTiet()
        {
            dtChiTiet = new DataTable();
            dtChiTiet.Columns.Add("MaSach", typeof(string));
            dtChiTiet.Columns.Add("TenSach", typeof(string));
            dtChiTiet.Columns.Add("SoLuong", typeof(int));
            dtChiTiet.Columns.Add("DonGia", typeof(decimal));
            dtChiTiet.Columns.Add("GiamGia", typeof(decimal));
            dtChiTiet.Columns.Add("ThanhTien", typeof(decimal),
                "SoLuong * DonGia * (1 - GiamGia / 100)");
        }

        // ------------------ CHẾ ĐỘ TẠO MỚI ------------------
        private void SetDefaultsForCreate()
        {
            txtSoHD.Text = "[Tự động tạo]";
            txtSoHD.ReadOnly = true;
            dtpNgayLap.Value = DateTime.Now;

            cboMaNV.SelectedIndex = -1;
            cboMaKH.SelectedIndex = -1;
            txtTongTien.Text = "0";

            SetFormReadOnly(false);
        }

        // ------------------ LOAD DỮ LIỆU ------------------
        private void LoadThongTinChung(string soHD)
        {
            using (var db = new DataContext())
            {
                var hd = db.HoaDons.FirstOrDefault(h => h.SoHD == soHD);
                if (hd == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn " + soHD);
                    return;
                }

                txtSoHD.Text = hd.SoHD;
                dtpNgayLap.Value = hd.NgayLap;
                cboMaNV.SelectedValue = hd.MaNV;
                cboMaKH.SelectedValue = hd.MaKH;
                txtTongTien.Text = hd.TongTien.ToString("");
            }
        }

        private void LoadChiTietVaoDataTable(string soHD)
        {
            dtChiTiet.Rows.Clear();

            using (var db = new DataContext())
            {
                var chiTietList = db.ChiTietHoaDons
                    .Where(ct => ct.SoHD == soHD)
                    .Join(db.Saches,
                          ct => ct.MaSach,
                          s => s.MaSach,
                          (ct, s) => new
                          {
                              ct.MaSach,
                              s.TenSach,
                              ct.SoLuong,
                              ct.DonGia,
                              ct.GiamGia
                          })
                    .ToList();

                foreach (var item in chiTietList)
                {
                    var row = dtChiTiet.NewRow();
                    row["MaSach"] = item.MaSach;
                    row["TenSach"] = item.TenSach;
                    row["SoLuong"] = item.SoLuong;
                    row["DonGia"] = item.DonGia;
                    row["GiamGia"] = item.GiamGia;
                    dtChiTiet.Rows.Add(row);
                }
            }
        }

        // ------------------ TÍNH TOÁN ------------------
        public void TinhLaiTongTien()
        {
            decimal tongTien = 0;
            if (dtChiTiet.Rows.Count > 0)
                tongTien = dtChiTiet.AsEnumerable()
                                    .Sum(r => r.Field<int>("SoLuong") *
                                              r.Field<decimal>("DonGia") *
                                              (1 - r.Field<decimal>("GiamGia") / 100));

            txtTongTien.Text = tongTien.ToString("N0"); // Định dạng số có dấu phẩy
        }


        // ------------------ KHÓA / MỞ FORM ------------------
        private void SetFormReadOnly(bool readOnly)
        {
            txtSoHD.ReadOnly = true;
            dtpNgayLap.Enabled = !readOnly;
            cboMaNV.Enabled = !readOnly;
            cboMaKH.Enabled = !readOnly;
            cboSach.Enabled = true;

            // Luôn bật các nút thao tác, không khoá
            btnThemSach.Enabled = true;
            btnXoaSach.Enabled = true;
            btnNhapLai.Enabled = true;
            btnDoiSach.Enabled = true;
        }


        private void btnThemSach_Click(object sender, EventArgs e)
        {
            if (cboSach.SelectedIndex == -1)
            {
                MessageBox.Show("Bạn chưa chọn sách để thêm.");
                return;
            }

            // Lấy thông tin sách từ ComboBox
            string maSach = cboSach.SelectedValue.ToString();

            // Kiểm tra số lượng nhập hợp lệ
            int soLuong = (int)numSach.Value;
            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0.");
                return;
            }

            // Lấy giá bìa (giá gốc) và giảm giá mặc định 0
            decimal donGia = 0;
            decimal giamGia = 0;

            using (var db = new DataContext())
            {
                var sach = db.Saches.FirstOrDefault(s => s.MaSach == maSach);
                if (sach == null)
                {
                    MessageBox.Show("Không tìm thấy sách đã chọn.");
                    return;
                }
                donGia = sach.GiaBia;
            }

            // Kiểm tra xem sách đã tồn tại trong dtChiTiet chưa
            var existingRow = dtChiTiet.Rows
                .Cast<DataRow>()
                .FirstOrDefault(r => r.Field<string>("MaSach") == maSach);

            if (existingRow != null)
            {
                // Nếu đã có, cộng dồn số lượng
                existingRow["SoLuong"] = existingRow.Field<int>("SoLuong") + soLuong;
            }
            else
            {
                // Thêm mới 1 dòng
                DataRow newRow = dtChiTiet.NewRow();
                newRow["MaSach"] = maSach;
                newRow["TenSach"] = cboSach.Text;
                newRow["SoLuong"] = soLuong;
                newRow["DonGia"] = donGia;
                newRow["GiamGia"] = giamGia;
                dtChiTiet.Rows.Add(newRow);
            }

            TinhLaiTongTien();
        }


        private void btnXoaSach_Click(object sender, EventArgs e)
        {
            if (dgvChiTietHoaDon.CurrentRow == null)
            {
                MessageBox.Show("Bạn chưa chọn sách để xóa.");
                return;
            }

            var maSach = dgvChiTietHoaDon.CurrentRow.Cells["MaSach"].Value.ToString();

            var rowToDelete = dtChiTiet.Rows
                .Cast<DataRow>()
                .FirstOrDefault(r => r.Field<string>("MaSach") == maSach);

            if (rowToDelete != null)
            {
                dtChiTiet.Rows.Remove(rowToDelete);
                TinhLaiTongTien();
            }
        }


        private void btnNhapLai_Click(object sender, EventArgs e)
        {
            dtChiTiet.Rows.Clear();
            TinhLaiTongTien();

            cboSach.SelectedIndex = -1;
            numSach.Value = 1;
        }


        private void btnDoiSach_Click(object sender, EventArgs e)
        {
            if (dgvChiTietHoaDon.CurrentRow == null)
            {
                MessageBox.Show("Bạn chưa chọn sách để đổi.");
                return;
            }

            int newSoLuong = (int)numSach.Value;
            if (newSoLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0.");
                return;
            }

            decimal newDonGia = numDonGia.Value;
            if (newDonGia < 0)
            {
                MessageBox.Show("Đơn giá không thể nhỏ hơn 0.");
                return;
            }

            decimal newGiamGia = numGiamGia.Value;
            if (newGiamGia < 0 || newGiamGia > 100)
            {
                MessageBox.Show("Giảm giá phải trong khoảng 0 đến 100%.");
                return;
            }

            string maSach = dgvChiTietHoaDon.CurrentRow.Cells["MaSach"].Value.ToString();

            var rowToEdit = dtChiTiet.Rows
                .Cast<DataRow>()
                .FirstOrDefault(r => r.Field<string>("MaSach") == maSach);

            if (rowToEdit != null)
            {
                rowToEdit["SoLuong"] = newSoLuong;
                rowToEdit["DonGia"] = newDonGia;
                rowToEdit["GiamGia"] = newGiamGia;
                // cột ThanhTien tự tính theo Expression Column
                TinhLaiTongTien();
            }
        }


        private void dgvChiTietHoaDon_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvChiTietHoaDon.CurrentRow != null)
            {
                var row = dgvChiTietHoaDon.CurrentRow;

                numSach.Value = Convert.ToDecimal(row.Cells["SoLuong"].Value);
                numDonGia.Value = Convert.ToDecimal(row.Cells["DonGia"].Value);
                numGiamGia.Value = Convert.ToDecimal(row.Cells["GiamGia"].Value);
            }
        }
    }
}
