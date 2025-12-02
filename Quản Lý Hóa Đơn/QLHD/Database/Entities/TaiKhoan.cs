using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(200)]
        public string MatKhau { get; set; } // Nên lưu dạng hash

        [StringLength(20)]
        public string VaiTro { get; set; } // Ràng buộc CHECK IN nên xử lý ở logic nghiệp vụ hoặc Fluent API

        [StringLength(10)]
        public string MaNV { get; set; } // Nullable FK

        [StringLength(10)]
        public string MaKH { get; set; } // Nullable FK

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Thuộc tính điều hướng (Khóa ngoại)
        [ForeignKey("MaNV")]
        public virtual NhanVien NhanVien { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }
    }
}
