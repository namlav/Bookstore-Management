using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        [StringLength(10)]
        public string SoHD { get; set; }

        [Required]
        public DateTime NgayLap { get; set; }

        [Required]
        [StringLength(10)]
        public string MaNV { get; set; }

        [Required]
        [StringLength(10)]
        public string MaKH { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTien { get; set; } = 0;

        // Thuộc tính điều hướng (Khóa ngoại)
        [ForeignKey("MaNV")]
        public virtual NhanVien NhanVien { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }

        // Liên kết đến bảng chi tiết
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; se