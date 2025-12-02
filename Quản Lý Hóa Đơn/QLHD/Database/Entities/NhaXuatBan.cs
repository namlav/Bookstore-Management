using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("NhaXuatBan")]
    public class NhaXuatBan
    {
        [Key]
        [StringLength(10)]
        public string MaNXB { get; set; }

        [Required]
        [StringLength(200)]
        public string TenNXB { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }

        [StringLength(15)]
        public string DienThoai { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        // Thuộc tính điều hướng: Một NXB có nhiều sách
        public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
    }
}
