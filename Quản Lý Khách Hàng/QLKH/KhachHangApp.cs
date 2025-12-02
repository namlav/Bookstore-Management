using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QLNS
{
    public partial class KhachHangApp : Form
    {
        private KhachHangService khService = new KhachHangService();

        public KhachHangApp()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                List<KhachHang> khachHangs = khService.GetAllKhachHangs();
                dgvKhachHang.DataSource = khachHangs;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL hoặc lỗi SQL: Vui lòng kiểm tra Connection String trong App.config. Chi tiết: {ex.Message}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dgvKhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ClearInputFields()
        {
            txtMaKH.Text = "";
            txtTenKH.Text = "";
            txtDienThoai.Text = "";
            txtEmail.Text = "";
            txtDiaChi.Text = "";
            txtMaKH.Enabled = true; 
            txtSearch.Text = "";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                KhachHang newKH = new KhachHang
                {
                    MaKH = txtMaKH.Text.Trim(),
                    TenKH = txtTenKH.Text.Trim(),
                    DienThoai = txtDienThoai.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim()
                };

                if (khService.AddKhachHang(newKH))
                {
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputFields();
                    LoadData();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex) when (ex.Number == 2627) // Lỗi Primary Key/Unique Constraint
            {
                MessageBox.Show("Mã KH hoặc Email/Điện thoại đã tồn tại. Vui lòng kiểm tra lại.", "Lỗi Trùng Lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm khách hàng thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtMaKH.Enabled == true)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa từ danh sách.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                KhachHang updatedKH = new KhachHang
                {
                    MaKH = txtMaKH.Text,
                    TenKH = txtTenKH.Text.Trim(),
                    DienThoai = txtDienThoai.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim()
                };

                if (khService.UpdateKhachHang(updatedKH))
                {
                    MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputFields();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu nào được cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cập nhật khách hàng thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaKH.Text) || txtMaKH.Enabled == true)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa KH có Mã: {txtMaKH.Text}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (khService.DeleteKhachHang(txtMaKH.Text))
                    {
                        MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Xóa khách hàng thất bại. Không tìm thấy Mã KH này.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Xóa khách hàng thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            try
            {
                List<KhachHang> searchResults = khService.SearchKhachHang(keyword);
                dgvKhachHang.DataSource = searchResults;

                if (searchResults.Count == 0 && !string.IsNullOrEmpty(keyword))
                {
                    MessageBox.Show("Không tìm thấy khách hàng nào với từ khóa: " + keyword, "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            LoadData();
        }

        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKhachHang.Rows[e.RowIndex];
                txtMaKH.Text = row.Cells["MaKH"].Value?.ToString();
                txtTenKH.Text = row.Cells["TenKH"].Value?.ToString();
                txtDienThoai.Text = row.Cells["DienThoai"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();
                txtMaKH.Enabled = false;
            }
        }
    }
}
