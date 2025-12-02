using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("DoanhThuThang")]
    public class DoanhThuThang
    {
        [Key] // Khóa phức hợp 1
        public int Nam { get; set; }

        [Key] // Khóa phức hợp 2
        [Range(1, 12)]
        public int Thang { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public de