using DoAn_DAL.Model;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormHoaDon : Form
    {
        private int maBan; // Bàn hiện tại
        private Model1 db = new Model1();

        // 🔹 Thuộc tính nhận từ FormKhuyenMai
        public int PhanTramGiam { get; set; } = 0;

        public FormHoaDon(int maBan)
        {
            InitializeComponent();
            this.maBan = maBan;
        }

        private void FormHoaDon_Load(object sender, EventArgs e)
        {
            var hd = db.HoaDon
                .Where(h => h.MaBan == maBan)
                .OrderByDescending(h => h.MaHD)
                .FirstOrDefault();

            if (hd == null)
            {
                MessageBox.Show("Bàn này chưa có hóa đơn!");
                this.Close();
                return;
            }

            // Cập nhật các label thông tin
            lblSoHoaDon.Text = hd.MaHD.ToString();
            lblBan.Text = hd.MaBan.ToString();

            DateTime now = DateTime.Now;
            lblNgay.Text = now.ToString("dd/MM/yyyy");
            lblGio.Text = now.ToString("HH:mm");

            // 🔹 Lấy chi tiết món ăn
            var chiTiet = db.ChiTietHoaDon
                .Where(c => c.MaHD == hd.MaHD)
                .ToList();

            // Thiết lập ListView
            lvHoaDon.Items.Clear();
            lvHoaDon.View = View.Details;
            lvHoaDon.FullRowSelect = true;
            lvHoaDon.GridLines = true;
            lvHoaDon.Columns.Clear();
            lvHoaDon.Columns.Add("Tên món", 150);
            lvHoaDon.Columns.Add("Số lượng", 60);
            lvHoaDon.Columns.Add("Đơn giá", 60);
            lvHoaDon.Columns.Add("Thành tiền", 100);

            long tongTien = 0;

            foreach (var item in chiTiet)
            {
                long donGia = item.Mon.DonGia ?? 0L;
                long thanhTien = item.SoLuong * donGia;

                ListViewItem lvi = new ListViewItem(item.Mon.TenMon);
                lvi.SubItems.Add(item.SoLuong.ToString());
                lvi.SubItems.Add(donGia.ToString("N0"));
                lvi.SubItems.Add(thanhTien.ToString("N0"));
                lvHoaDon.Items.Add(lvi);

                tongTien += thanhTien;
            }

            // 🔹 Tính giảm giá
            long tienGiam = tongTien * PhanTramGiam / 100;
            long tongSauGiam = tongTien - tienGiam;

            // 🔹 Hiển thị kết quả
            lblGiamGia.Text = $"{PhanTramGiam}%";
            lblTongTien.Text = $"{tongSauGiam:N0} đ";
        }

        private void btnXuatHoaDon_Click(object sender, EventArgs e)
        {
            if (lvHoaDon.Items.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFile.FileName = $"HoaDon_Ban{maBan}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveFile.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (var sw = new System.IO.StreamWriter(saveFile.FileName, false, Encoding.UTF8))
                {
                    // Thông tin quán
                    sw.WriteLine($"Tên quán: {lblTenQuan.Text}");
                    sw.WriteLine($"Địa chỉ: {lblDiaChi.Text}");
                    sw.WriteLine();

                    // Thông tin hóa đơn
                    sw.WriteLine($"Số hóa đơn: {lblSoHoaDon.Text}");
                    sw.WriteLine($"Bàn: {lblBan.Text}");
                    sw.WriteLine($"Ngày: {lblNgay.Text}");
                    sw.WriteLine($"Giờ: {lblGio.Text}");
                    sw.WriteLine();

                    // Header cột
                    sw.WriteLine(string.Format("{0,-20} {1,10} {2,15} {3,15}", "Tên món", "Số lượng", "Đơn giá", "Thành tiền"));

                    // Dữ liệu từng món
                    foreach (ListViewItem item in lvHoaDon.Items)
                    {
                        string tenMon = item.SubItems[0].Text;
                        string soLuong = item.SubItems[1].Text;
                        string donGia = item.SubItems[2].Text.Replace(",", "");
                        string thanhTien = item.SubItems[3].Text.Replace(",", "");

                        sw.WriteLine(string.Format("{0,-20} {1,10} {2,15} {3,15}", tenMon, soLuong, donGia, thanhTien));
                    }

                    // Tổng kết
                    sw.WriteLine();
                    sw.WriteLine($"Giảm giá: {lblGiamGia.Text}");
                    sw.WriteLine($"Tổng tiền sau giảm: {lblTongTien.Text}");
                }

                MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất hóa đơn: " + ex.Message);
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                // 🔹 Lấy hóa đơn mới nhất của bàn
                var hd = db.HoaDon
                    .Where(h => h.MaBan == maBan)
                    .OrderByDescending(h => h.MaHD)
                    .FirstOrDefault();

                if (hd == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn để lưu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 🔹 Tính lại tổng tiền gốc
                long tongTien = db.ChiTietHoaDon
                    .Where(c => c.MaHD == hd.MaHD)
                    .Join(db.Mon,
                          ct => ct.MaMon,
                          m => m.MaMon,
                          (ct, m) => (ct.SoLuong * (m.DonGia ?? 0L)))
                    .Sum();

                // 🔹 Tính giảm giá và tổng sau giảm
                long tienGiam = tongTien * PhanTramGiam / 100;
                long tongSauGiam = tongTien - tienGiam;

                // 🔹 Cập nhật hóa đơn trong database
                hd.TongTien = tongSauGiam;
                hd.NgayLap = DateTime.Now;
                hd.TrangThai = true; // Đánh dấu đã thanh toán

                // Nếu bạn có cột "GiamGia" trong bảng Hóa đơn thì thêm dòng này:
                // hd.GiamGia = PhanTramGiam;

                db.SaveChanges();

                MessageBox.Show($"✅ Hóa đơn đã được lưu thành công!\nTổng tiền sau giảm: {tongSauGiam:N0} đ",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
