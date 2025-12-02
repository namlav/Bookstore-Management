namespace QLNS
{
    partial class ChucNang
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSach = new System.Windows.Forms.Button();
            this.btnKhach = new System.Windows.Forms.Button();
            this.btnDoanhThu = new System.Windows.Forms.Button();
            this.btnNhanVien = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSach
            // 
            this.btnSach.Location = new System.Drawing.Point(228, 79);
            this.btnSach.Name = "btnSach";
            this.btnSach.Size = new System.Drawing.Size(110, 89);
            this.btnSach.TabIndex = 0;
            this.btnSach.Text = "Quản Lý Sách";
            this.btnSach.UseVisualStyleBackColor = true;
            this.btnSach.Click += new System.EventHandler(this.btnSach_Click);
            // 
            // btnKhach
            // 
            this.btnKhach.Location = new System.Drawing.Point(432, 79);
            this.btnKhach.Name = "btnKhach";
            this.btnKhach.Size = new System.Drawing.Size(110, 89);
            this.btnKhach.TabIndex = 1;
            this.btnKhach.Text = "Quản Lý Khách Hàng";
            this.btnKhach.UseVisualStyleBackColor = true;
            // 
            // btnDoanhThu
            // 
            this.btnDoanhThu.Location = new System.Drawing.Point(432, 221);
            this.btnDoanhThu.Name = "btnDoanhThu";
            this.btnDoanhThu.Size = new System.Drawing.Size(110, 89);
            this.btnDoanhThu.TabIndex = 2;
            this.btnDoanhThu.Text = "Quản Lý Doanh Thu";
            this.btnDoanhThu.UseVisualStyleBackColor = true;
            // 
            // btnNhanVien
            // 
            this.btnNhanVien.Location = new System.Drawing.Point(228, 221);
            this.btnNhanVien.Name = "btnNhanVien";
            this.btnNhanVien.Size = new System.Drawing.Size(110, 89);
            this.btnNhanVien.TabIndex = 3;
            this.btnNhanVien.Text = "Quản Lý Nhân Viên";
            this.btnNhanVien.UseVisualStyleBackColor = true;
            // 
            // ChucNang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnNhanVien);
            this.Controls.Add(this.btnDoanhThu);
            this.Controls.Add(this.btnKhach);
            this.Controls.Add(this.btnSach);
            this.Name = "ChucNang";
            this.Text = "ChucNang";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSach;
        private System.Windows.Forms.Button btnKhach;
        private System.Windows.Forms.Button btnDoanhThu;
        private System.Windows.Forms.Button btnNhanVien;
    }
}