using DoAn_BUS;
using DoAn_DAL.Model;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormKhuyenMai : Form
    {
        public int MaBan { get; set; }
        HoaDonBUS hoaDonBUS = new HoaDonBUS();

        public FormKhuyenMai()
        {
            InitializeComponent();
        }

        private void FormKhuyenMai_Load(object sender, EventArgs e)
        {
            cboGiam.Items.Clear();
            cboGiam.Items.Add("0%");
            cboGiam.Items.Add("10%");
            cboGiam.Items.Add("20%");
            cboGiam.Items.Add("30%");
            cboGiam.Items.Add("40%");
            cboGiam.Items.Add("50%");
            cboGiam.SelectedIndex = 0;
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            if (MaBan <= 0)
            {
                MessageBox.Show("Không xác định được bàn!");
                return;
            }

            var hd = hoaDonBUS.GetBillByBan(MaBan);
            if (hd == null)
            {
                MessageBox.Show("Không tìm thấy hóa đơn cho bàn này!");
                return;
            }

            // 🔹 Lấy phần trăm giảm
            int phanTramGiam = int.Parse(cboGiam.SelectedItem.ToString().Replace("%", "").Trim());

            // 🔹 Tính tổng để hiển thị (chỉ để báo)
            long tongTien = hd.TongTien ?? 0L;
            long tienGiam = tongTien * phanTramGiam / 100;
            long tongSauGiam = tongTien - tienGiam;

            MessageBox.Show(
                $"Áp dụng giảm {phanTramGiam}% thành công!\nTổng sau giảm: {tongSauGiam:N0} đ",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information
            );

            // 🔹 Mở FormHoaDon và truyền phần trăm giảm
            FormHoaDon fhd = new FormHoaDon(MaBan)
            {
                PhanTramGiam = phanTramGiam
            };
            fhd.ShowDialog();

            this.Close();
        }
    }
}
