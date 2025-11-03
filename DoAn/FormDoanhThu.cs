using DoAn_DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormDoanhThu : Form
    {
        private Model1 db = new Model1();

        public FormDoanhThu()
        {
            InitializeComponent();
        }

        private void FormDoanhThu_Load(object sender, EventArgs e)
        {
            // Load tháng
            for (int i = 1; i <= 12; i++) cboThang.Items.Add(i);
            cboThang.SelectedIndex = DateTime.Now.Month - 1;

            // Thiết lập DataGridView
            dgvHoaDon.AutoGenerateColumns = false;
            dgvHoaDon.Columns.Clear();

            // Cột Số HĐ
            var col1 = new DataGridViewTextBoxColumn();
            col1.HeaderText = "Số HĐ";
            col1.DataPropertyName = "MaHD"; // trùng với tên thuộc tính trong nguồn dữ liệu
            col1.Width = 80;
            col1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoaDon.Columns.Add(col1);

            // Cột Bàn
            var col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "Bàn";
            col2.DataPropertyName = "MaBan";
            col2.Width = 80;
            col2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoaDon.Columns.Add(col2);

            // Cột Ngày
            var col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "Ngày";
            col3.DataPropertyName = "Ngay";
            col3.Width = 100;
            col3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoaDon.Columns.Add(col3);

            // Cột Giờ
            var col4 = new DataGridViewTextBoxColumn();
            col4.HeaderText = "Giờ";
            col4.DataPropertyName = "Gio";
            col4.Width = 100;
            col4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoaDon.Columns.Add(col4);

            // Cột Tổng tiền
            var col5 = new DataGridViewTextBoxColumn();
            col5.HeaderText = "Tổng tiền";
            col5.DataPropertyName = "TongTien";
            col5.Width = 120;
            col5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            col5.DefaultCellStyle.Format = "N0"; // định dạng số
            dgvHoaDon.Columns.Add(col5);
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (cboThang.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn tháng!");
                return;
            }

            if (!int.TryParse(txtNam.Text.Trim(), out int nam))
            {
                MessageBox.Show("Vui lòng nhập năm hợp lệ!");
                return;
            }

            int thang = (int)cboThang.SelectedItem;

            // Lấy tất cả hóa đơn trong tháng và năm đã chọn (chỉ lấy raw DateTime)
            var hdListRaw = db.HoaDon
                              .Where(h => h.NgayLap.HasValue &&
                                          h.NgayLap.Value.Month == thang &&
                                          h.NgayLap.Value.Year == nam)
                              .ToList(); // gọi ToList() trước khi format

            // Chuyển sang anonymous object với format ngày/giờ
            var hdList = hdListRaw
                         .Select(h => new
                         {
                             h.MaHD,
                             h.MaBan,
                             Ngay = h.NgayLap.Value.ToString("dd/MM/yyyy"),
                             Gio = h.NgayLap.Value.ToString("HH:mm"),
                             TongTien = h.TongTien ?? 0m
                         })
                         .ToList();

            dgvHoaDon.DataSource = hdList;

            if (dgvHoaDon.Columns["TongTien"] != null)
                dgvHoaDon.Columns["TongTien"].DefaultCellStyle.Format = "N0";

            // Tính tổng doanh thu
            decimal tong = hdList.Sum(h => h.TongTien);
            txtTongTien.Text = tong.ToString("N0") + " đ";
        }
    }
}
