using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace QLNS.Database.Entities
{
    [Table("Sach")]
    public class Sach
    {
        [Key]
        [StringLength(10)]
        public string MaSach { get; set; }

        [Required]
        [StringLength(200)]
        public string TenSach { get; set; }

        [Required]
        [StringLength(10)]
        public string MaTG { get; set; }

        [Required]
        [StringLength(10)]
        public string MaTL { get; set; }

        [Required]
        [StringLength(10)]
        public string MaNXB { get; set; }

        [Range(1900, 9999)] // 9999 là giá trị giả định, bạn có thể thay bằng năm hiện tại + 1
        public int NamXuatBan { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaBia { get; set; }

        [Range(0, int.MaxValue)]
        public int SoLuongTon { get; set; } = 0;

        // Thuộc tính điều hướng (Khóa ngoại)
        [ForeignKey("MaTG")]
        public virtual TacGia TacGia { get; set; }

        [ForeignKey("MaTL")]
        public virtual TheLoai TheLoai { get; set; }

        [ForeignKey("MaNXB")]
        public virtual NhaXuatBan NhaXuatBan { get; set; }

        // Liên kết đến các bảng chi tiết
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; se