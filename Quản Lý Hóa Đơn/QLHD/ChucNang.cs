using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLNS
{
    public partial class ChucNang : Form
    {
        public ChucNang()
        {
            InitializeComponent();
        }

        private void btnSach_Click(object sender, EventArgs e)
        {
            QuanLySach fSach = new QuanLySach();
            fSach.ShowDialog();
        }
    }
}
