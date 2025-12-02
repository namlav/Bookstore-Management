using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.Entities
{
    [Table("ChiTietPhieuNhap")]
    public class ChiTietPhieuNhap
    {
        [Key] // Khóa phức hợp 1
        [StringLength(10)]
        public string MaSach { get; set; }

        [Key] // Khóa phức hợp 2
        [StringLength(10)]
        public string SoPN { get; set; }

        [Range(1, int.MaxValue)]
        public int SoLuongNhap { get; set; }

        [Range(0.01, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaNhap { get; set; }

        // Thuộc tính điều hướng (Khóa ngoại)
        [ForeignKey("MaSach")]
        public virtual Sach Sach { get; set; }

        [ForeignKey("SoPN")]
        public virtual 