using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("TheLoai")]
    public class TheLoai
    {
        [Key]
        [StringLength(10)]
        public string MaTL { get; set; }

        [Required]
        [StringLength(100)]
        public string TenTL { get; set; }

        [StringLength(200)]
        public string MoTa { get; set; }

        // Thuộc tính điều hướng: Một thể loại có nhiều sách
        public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
    }
}
