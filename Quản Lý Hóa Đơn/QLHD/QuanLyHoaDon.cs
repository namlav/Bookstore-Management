using QLNS.Database;
using QLNS.Database.Entities;
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
    public partial class QuanLyHoaDon : Form
    {
        // ----- TÊN CÁC CONTROL CỦA BẠN (TỪ CODE GỐC) -----
        // dgvHoaDon (Grid duy nhất)
        // txtSoHD, txtTongTien, dtpNgayLap
        // cboMaNV, cboMaKH
        // btnXemChiTiet, btnNhapLai
        // --------------------------------------------------

        // Biến cờ để biết Grid đang ở chế độ nào:
        // true = Lưới đang hiển thị Danh Sách Hóa Đơn (Master)
        // false = Lưới đang hiển thị Chi Tiết Hóa Đơn (Detail)
        private bool isMasterView = true;

        public QuanLyHoaDon()
        {
            InitializeComponent();
        }

        // 1. SỰ KIỆN LOAD FORM (ĐÃ SỬA LẠI)
        private void QuanLyHoaDon_Load(object sender, EventArgs e)
        {
            // Tải ComboBoxes
            LoadComboBoxNhanVien();
            LoadComboBoxKhachHang();

            // Tải Lưới Danh Sách Hóa Đơn (Master)
            LoadDanhSachHoaDon();

            // Thiết lập giá trị mặc định cho form
            SetDefaults();

            // Tắt "Hàng mới" (*) 
            dgvHoaDon.AllowUserToAddRows = false;

            SetFormReadOnly(false); // Bắt đầu ở chế độ "Tạo Mới"
        }

        // 2. HÀM TẢI LƯỚI DANH SÁCH (MASTER) (ĐƯỢC TÁCH RA)
        private void LoadDanhSachHoaDon()
        {
            using (var db = new DataContext())
            {
                try
                {
                    // (Code truy vấn Linq để lấy Danh Sách Hóa Đơn... giữ nguyên)
                    var danhSachHD = db.HoaDons
                                       .Join(db.NhanViens,
                                             hd => hd.MaNV, nv => nv.MaNV,
                                             (hd, nv) => new { hd, nv })
                                       .Join(db.KhachHangs,
                                             hd_nv => hd_nv.hd.MaKH, kh => kh.MaKH,
                                             (hd_nv, kh) => new
                                             {
                                                 SoHD = hd_nv.hd.SoHD,
                                                 NgayLap = hd_nv.hd.NgayLap,
                                                 TenNV = hd_nv.nv.TenNV,
                                                 TenKH = kh.TenKH,
                                                 TongTien = hd_nv.hd.TongTien
                                             })
                                       .OrderByDescending(x => x.NgayLap)
                                       .ToList();

                    dgvHoaDon.DataSource = danhSachHD;

                    // (Code đặt tên cột cho Danh Sách... giữ nguyên)
                    dgvHoaDon.Columns["SoHD"].HeaderText = "Số HĐ";
                    dgvHoaDon.Columns["NgayLap"].HeaderText = "Ngày Lập";
                    dgvHoaDon.Columns["TenNV"].HeaderText = "Nhân Viên";
                    dgvHoaDon.Columns["TenKH"].HeaderText = "Khách Hàng";
                    dgvHoaDon.Columns["TongTien"].HeaderText = "Tổng Tiền";

                    // QUAN TRỌNG: Gỡ cột "Xóa" (nếu có)
                    if (dgvHoaDon.Columns.Contains("btnXoa"))
                    {
                        dgvHoaDon.Columns.Remove("btnXoa");
                    }

                    isMasterView = true; // Đặt cờ: đang xem Danh Sách
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải danh sách hóa đơn: " + ex.Message);
                }
            }
        }

        // 3. HÀM ĐẶT MẶC ĐỊNH (ĐƯỢC TÁCH RA)
        private void SetDefaults()
        {
            dtpNgayLap.Value = DateTime.Now;
            txtTongTien.Text = "0";
            txtTongTien.ReadOnly = true;
            txtTongTien.TextAlign = HorizontalAlignment.Right;
            txtSoHD.Clear();
            cboMaNV.SelectedIndex = -1;
            cboMaKH.SelectedIndex = -1;
        }

        // 4. HÀM TẢI COMBOBOX NHÂN VIÊN (GIỮ NGUYÊN)
        private void LoadComboBoxNhanVien()
        {
            using (var db = new DataContext())
            {
                try
                {
                    var danhSachNV = db.NhanViens.Select(nv => new { nv.MaNV, nv.TenNV }).ToList();
                    cboMaNV.DataSource = danhSachNV;
                    cboMaNV.DisplayMember = "TenNV";
                    cboMaNV.ValueMember = "MaNV";
                }
                catch (Exception ex) { /* Xử lý lỗi */ }
            }
        }

        // 5. HÀM TẢI COMBOBOX KHÁCH HÀNG (GIỮ NGUYÊN)
        private void LoadComboBoxKhachHang()
        {
            using (var db = new DataContext())
            {
                try
                {
                    var danhSachKH = db.KhachHangs.Select(kh => new { kh.MaKH, kh.TenKH }).ToList();
                    cboMaKH.DataSource = danhSachKH;
                    cboMaKH.DisplayMember = "TenKH";
                    cboMaKH.ValueMember = "MaKH";
                }
                catch (Exception ex) { /* Xử lý lỗi */ }
            }
        }

        // 6. SỰ KIỆN CLICK LƯỚI (ĐÃ SỬA)
        private void dgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Chỉ xử lý click khi đang ở chế độ Danh Sách (Master)
            if (isMasterView == false || e.RowIndex < 0)
            {
                return;
            }

            try
            {
                // Lấy SoHD từ LƯỚI DANH SÁCH
                string selectedSoHD = dgvHoaDon.Rows[e.RowIndex].Cells["SoHD"].Value.ToString();

                using (var db = new DataContext())
                {
                    var hoaDon = db.HoaDons.FirstOrDefault(hd => hd.SoHD == selectedSoHD);
                    if (hoaDon == null) return;

                    // Điền thông tin lên control (giữ nguyên)
                    txtSoHD.Text = hoaDon.SoHD;
                    dtpNgayLap.Value = hoaDon.NgayLap;
                    txtTongTien.Text = hoaDon.TongTien.ToString("N0");
                    cboMaNV.SelectedValue = hoaDon.MaNV;
                    cboMaKH.SelectedValue = hoaDon.MaKH;

                    // *** XÓA 2 DÒNG GÂY LỖI ***
                    // dgvHoaDon.DataSource = null; (KHÔNG XÓA)
                    // dgvHoaDon.Rows.Clear(); (KHÔNG XÓA)

                    SetFormReadOnly(true); // Khóa form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin hóa đơn: " + ex.Message);
            }
        }

        // 7. SỰ KIỆN CLICK NÚT "XEM CHI TIẾT" (ĐÃ SỬA)
        private void btnXemChiTiet_Click(object sender, EventArgs e)
        {
            string soHDCanXem = txtSoHD.Text;
            if (string.IsNullOrWhiteSpace(soHDCanXem))
            {
                MessageBox.Show("Bạn chưa chọn hóa đơn nào để xem chi tiết.");
                return;
            }

            using (var db = new DataContext())
            {
                try
                {
                    // (Code Linq truy vấn Chi Tiết... giữ nguyên)
                    var chiTiet = db.ChiTietPhieuNhaps
                                    .Where(ct => ct.SoHD == soHDCanXem)
                                    .Join(db.Saches, // (dùng db.sachs của bạn)
                                          ct => ct.MaSach, s => s.MaSach,
                                          (ct, s) => new {
                                              MaSach = ct.MaSach,
                                              TenSach = s.TenSach,
                                              SoLuong = ct.SoLuong,
                                              DonGia = ct.DonGia,
                                              GiamGia = ct.GiamGia,
                                              ThanhTien = ct.SoLuong * ct.DonGia * (1 - ct.GiamGia / 100)
                                          })
                                    .ToList();

                    // Gán vào lưới (đè lên danh sách)
                    dgvHoaDon.DataSource = chiTiet;
                    isMasterView = false; // Đặt cờ: đang xem Chi Tiết

                    // (Code đặt tên cột chi tiết... giữ nguyên)
                    dgvHoaDon.Columns["MaSach"].HeaderText = "Mã Sách";
                    dgvHoaDon.Columns["TenSach"].HeaderText = "Tên Sách";
                    dgvHoaDon.Columns["ThanhTien"].HeaderText = "Thành Tiền";
                    // ... (thêm các cột khác) ...

                    // QUAN TRỌNG: Thêm cột "Xóa" VÀO LÚC NÀY
                    if (dgvHoaDon.Columns["btnXoa"] == null)
                    {
                        DataGridViewButtonColumn btnXoa = new DataGridViewButtonColumn();
                        btnXoa.Name = "btnXoa";
                        btnXoa.HeaderText = "Xóa";
                        btnXoa.Text = "Xóa";
                        btnXoa.UseColumnTextForButtonValue = true;
                        dgvHoaDon.Columns.Add(btnXoa);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải chi tiết hóa đơn: " + ex.Message);
                }
            }
        }

        // 8. SỰ KIỆN CLICK NÚT "NHẬP LẠI" (TẠO MỚI) (ĐÃ S