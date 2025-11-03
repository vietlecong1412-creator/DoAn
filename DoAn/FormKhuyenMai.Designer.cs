namespace DoAn
{
    partial class FormKhuyenMai
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboGiam = new System.Windows.Forms.ComboBox();
            this.btnInHoaDon = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Chọn phần trăm giảm:";
            // 
            // cboGiam
            // 
            this.cboGiam.FormattingEnabled = true;
            this.cboGiam.Location = new System.Drawing.Point(154, 21);
            this.cboGiam.Name = "cboGiam";
            this.cboGiam.Size = new System.Drawing.Size(159, 24);
            this.cboGiam.TabIndex = 3;
            // 
            // btnInHoaDon
            // 
            this.btnInHoaDon.Location = new System.Drawing.Point(108, 64);
            this.btnInHoaDon.Name = "btnInHoaDon";
            this.btnInHoaDon.Size = new System.Drawing.Size(104, 45);
            this.btnInHoaDon.TabIndex = 5;
            this.btnInHoaDon.Text = "In hóa đơn";
            this.btnInHoaDon.UseVisualStyleBackColor = true;
            this.btnInHoaDon.Click += new System.EventHandler(this.btnInHoaDon_Click);
            // 
            // FormKhuyenMai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 134);
            this.Controls.Add(this.btnInHoaDon);
            this.Controls.Add(this.cboGiam);
            this.Controls.Add(this.label1);
            this.Name = "FormKhuyenMai";
            this.Text = "Voucher";
            this.Load += new System.EventHandler(this.FormKhuyenMai_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboGiam;
        private System.Windows.Forms.Button btnInHoaDon;
    }
}