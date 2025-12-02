using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("PhieuNhap")]
    public class PhieuNhap
    {
        [Key]
        [StringLength(10)]
        public string SoPN { get; set; }

        [Required]
        public DateTime NgayNhap { get; set; }

        [Required]
        [StringLength(10)]
        public string MaNV { get; set; }

        // Thuộc tính điều hướng (Khóa ngoại)
        [ForeignKey("MaNV")]
        public virtual NhanVien NhanVien { get; set; }

        // Liên kết đến bảng chi tiết
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    }
}
