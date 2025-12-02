using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Database.DTOs
{

    [Table("Sach")]
    public class SachDTO
    {
        [Column("MaSach"), Key]
        public string IDsach { get; set; }

        [Column("TenSach")]
        public string Tensach { get; set; }
        [Column("MaTG")]
        public string IDtacgia { get; set; }

        [Column("NamXuatBan")]
        public int Namxuatban { get; set; }

        [Column("SoLuongTon")]
        public int Soluongton { get; set; }
    }
}
