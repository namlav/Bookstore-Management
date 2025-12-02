using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("TacGia")]
    public class TacGia
    {
        [Key]
        [StringLength(10)]
        public string MaTG { get; set; }

        [Required(ErrorMessage = "Tên tác giả là bắt buộc")]
        [StringLength(100)]
        public string TenTG { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }

        [StringLength(15)]
        public string DienThoai { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        // Thuộc tính điều hướng: Một tác giả có thể viết nhiều sách
        public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
    }
}
