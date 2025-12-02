using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNS.Database.Entities
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key]
        [Column(Order = 1)] // (Dùng Column Order 1 cho EF6)
        [StringLength(10)]
        public string MaSach { get; set; } // <-- Tên phải là 'MaSach'

        [Key]
        [Column(Order = 2)] // (Dùng Column Order 2 cho EF6)
        [StringLength(10)]
        public string SoHD { get; set; }   // <-- Tên phải là 'SoHD'

        // ... Các thuộc tính khác (SoLuong, DonGia...)
        [Range(1, int.MaxValue)]
        public int SoLuong { get; set; }

        [Range(0.01, (double)decimal.MaxValue)]
        public decimal DonGia { get; set; }

        [Range(0, 100)]
        public decimal GiamGia { get; set; } = 0;

        // ... Các thuộc tính điều hướng ...
        [ForeignKey("MaSach")]
        public virtual Sach Sach { get; set; }

        [ForeignKey("SoHD")]
        public virtual HoaDon HoaDon { get; set; }
    }
}