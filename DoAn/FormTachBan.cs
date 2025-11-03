using DoAn_DAL.Model;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormTachBan : Form
    {
        public int MaBanHienTai { get; set; }
        private Model1 db = new Model1();

        public FormTachBan()
        {
            InitializeComponent();
        }

        private void FormTachBan_Load(object sender, EventArgs e)
        {
            LoadHoaDon();
        }

        private void LoadHoaDon()
        {
            lvHoaDon.Items.Clear();
            lvHoaDon.View = View.Details;
            lvHoaDon.FullRowSelect = true;
            lvHoaDon.CheckBoxes = true;

            lvHoaDon.Columns.Clear();
            lvHoaDon.Columns.Add("Tên món", 150);
            lvHoaDon.Columns.Add("Số lượng", 80);
            lvHoaDon.Columns.Add("Giá", 100);

            // 🔹 Lấy hóa đơn mới nhất của bàn hiện tại (chưa thanh toán)
            var hd = db.HoaDon
                       .Where(h => h.MaBan == MaBanHienTai && h.TrangThai == false)
                       .OrderByDescending(h => h.MaHD)
                       .FirstOrDefault();

            if (hd == null) return;

            // 🔹 Lấy chi tiết hóa đơn + thông tin món
            var chiTiet = db.ChiTietHoaDon
                             .Where(c => c.MaHD == hd.MaHD)
                             .Join(db.Mon,
                                   ct => ct.MaMon,
                                   m => m.MaMon,
                                   (ct, m) => new
                                   {
                                       ChiTiet = ct,
                                       TenMon = m.TenMon,
                                       DonGia = m.DonGia
                                   }).ToList();

            foreach (var item in chiTiet)
            {
                ListViewItem lvi = new ListViewItem(item.TenMon);
                lvi.SubItems.Add(item.ChiTiet.SoLuong.ToString());
                lvi.SubItems.Add(string.Format("{0:N0}", item.DonGia ?? 0));
                lvi.Tag = item.ChiTiet;
                lvHoaDon.Items.Add(lvi);
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBanMoi.Text.Trim(), out int maBanMoi) || maBanMoi <= 0)
            {
                MessageBox.Show("Vui lòng nhập số bàn hợp lệ!");
                return;
            }

            if (lvHoaDon.CheckedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một món để tách!");
                return;
            }

            // 🔹 Lấy hóa đơn cũ (bàn hiện tại)
            var hdCu = db.HoaDon
                         .Where(h => h.MaBan == MaBanHienTai && h.TrangThai == false)
                         .OrderByDescending(h => h.MaHD)
                         .FirstOrDefault();

            if (hdCu == null) return;

            // 🔹 Kiểm tra xem bàn mới có hóa đơn chưa
            var hdMoi = db.HoaDon
                          .Where(h => h.MaBan == maBanMoi && h.TrangThai == false)
                          .OrderByDescending(h => h.MaHD)
                          .FirstOrDefault();

            // 🔹 Nếu chưa có thì tạo hóa đơn mới
            if (hdMoi == null)
            {
                hdMoi = new HoaDon
                {
                    MaBan = maBanMoi,
                    NgayLap = null,
                    TongTien = 0,
                    TrangThai = false
                };
                db.HoaDon.Add(hdMoi);
                db.SaveChanges(); // Lưu để có MaHD
            }

            // 🔹 Thực hiện tách món
            foreach (ListViewItem item in lvHoaDon.CheckedItems)
            {
                if (item.Tag is ChiTietHoaDon cthd)
                {
                    var cthdTrongDb = db.ChiTietHoaDon
                                         .FirstOrDefault(c => c.MaHD == cthd.MaHD && c.MaMon == cthd.MaMon);
                    if (cthdTrongDb == null) continue;

                    // ➕ Thêm món vào hóa đơn mới
                    db.ChiTietHoaDon.Add(new ChiTietHoaDon
                    {
                        MaHD = hdMoi.MaHD,
                        MaMon = cthdTrongDb.MaMon,
                        SoLuong = cthdTrongDb.SoLuong
                    });

                    // ❌ Xóa món khỏi hóa đơn cũ
                    db.ChiTietHoaDon.Remove(cthdTrongDb);
                }
            }

            db.SaveChanges();

            // 🔹 Tính lại tổng tiền cho hóa đơn cũ
            var tongCu = db.ChiTietHoaDon
                           .Where(c => c.MaHD == hdCu.MaHD)
                           .Join(db.Mon, ct => ct.MaMon, m => m.MaMon, (ct, m) => (long)(ct.SoLuong * m.DonGia))
                           .DefaultIfEmpty(0)
                           .Sum();

            // 🔹 Tính lại tổng tiền cho hóa đơn mới
            var tongMoi = db.ChiTietHoaDon
                            .Where(c => c.MaHD == hdMoi.MaHD)
                            .Join(db.Mon, ct => ct.MaMon, m => m.MaMon, (ct, m) => (long)(ct.SoLuong * m.DonGia))
                            .DefaultIfEmpty(0)
                            .Sum();

            hdCu.TongTien = tongCu;
            hdMoi.TongTien = tongMoi;
            db.SaveChanges();

            MessageBox.Show("Tách bàn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
