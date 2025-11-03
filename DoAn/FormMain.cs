using DoAn_BUS;
using DoAn_DAL.Model;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormMain : Form
    {
        int maBanHienTai = 0;
        HoaDonBUS hoaDonBUS = new HoaDonBUS();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            lvHoaDon.View = View.Details;
            lvHoaDon.FullRowSelect = true;
            lvHoaDon.GridLines = true;

            lvHoaDon.Columns.Add("Tên món", 150);
            lvHoaDon.Columns.Add("Số lượng", 80);
            lvHoaDon.Columns.Add("Giá", 100);
            lvHoaDon.Columns.Add("Thành tiền", 120);

            // Gán sự kiện cho bàn
            for (int i = 1; i <= 12; i++)
            {
                Button btn = (Button)this.Controls["btnBan" + i];
                if (btn != null)
                {
                    btn.Tag = i;
                    btn.Click += Ban_Click;
                }
            }
            if (FormLogin.Role == "nhanvien")
            {
                btnDoanhThu.Enabled = false;
            }

            LoadLoaiMon();
            CapNhatMauBan();
        }

        void LoadLoaiMon()
        {
            using (var db = new Model1())
            {
                cboLoaiMon.DataSource = db.LoaiMon.ToList();
                cboLoaiMon.DisplayMember = "TenLoai";
                cboLoaiMon.ValueMember = "MaLoai";
            }
            cboLoaiMon.SelectedIndexChanged += CboLoaiMon_SelectedIndexChanged;
        }

        private void CboLoaiMon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLoaiMon.SelectedValue == null) return;
            int maLoai = (int)cboLoaiMon.SelectedValue;

            using (var db = new Model1())
            {
                cboMon.DataSource = db.Mon.Where(m => m.MaLoai == maLoai).ToList();
                cboMon.DisplayMember = "TenMon";
                cboMon.ValueMember = "MaMon";
            }
        }

        private void Ban_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            maBanHienTai = (int)btn.Tag;
            ShowBill(maBanHienTai);
        }

        void ShowBill(int maBan)
        {
            lvHoaDon.Items.Clear();
            txtTongTien.Text = "0";

            var hd = hoaDonBUS.GetBillByBan(maBan);
            if (hd == null) return;

            using (var db = new Model1())
            {
                var chiTiet = db.ChiTietHoaDon
                                .Where(c => c.MaHD == hd.MaHD)
                                .Join(db.Mon, ct => ct.MaMon, m => m.MaMon, (ct, m) => new
                                {
                                    ct.SoLuong,
                                    m.TenMon,
                                    DonGia = m.DonGia
                                })
                                .ToList();

                long tongTien = 0;
                foreach (var item in chiTiet)
                {
                    long donGia = item.DonGia ?? 0L;
                    long thanhTien = item.SoLuong * donGia;

                    ListViewItem lvi = new ListViewItem(item.TenMon);
                    lvi.SubItems.Add(item.SoLuong.ToString());
                    lvi.SubItems.Add(donGia.ToString("N0"));
                    lvi.SubItems.Add(thanhTien.ToString("N0"));
                    lvHoaDon.Items.Add(lvi);

                    tongTien += thanhTien;
                }

                txtTongTien.Text = tongTien.ToString("N0");
            }
        }



        private void btnThem_Click(object sender, EventArgs e)
        {
            if (maBanHienTai == 0)
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi thêm món!");
                return;
            }
            if (cboMon.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn món!");
                return;
            }

            int maMon = (int)cboMon.SelectedValue;

            hoaDonBUS.ThemMon(maBanHienTai, maMon, 1);
            ShowBill(maBanHienTai);
            CapNhatMauBan();
        }

      
        private void btnGhepBan_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBanNguon.Text, out int maBanNguon) || !int.TryParse(txtBanDich.Text, out int maBanDich))
            {
                MessageBox.Show("Vui lòng nhập số bàn hợp lệ!");
                return;
            }

            if (maBanNguon == maBanDich)
            {
                MessageBox.Show("Không thể ghép cùng một bàn!");
                return;
            }

            var hdNguon = hoaDonBUS.GetBillByBan(maBanNguon);
            if (hdNguon == null)
            {
                MessageBox.Show("Bàn nguồn chưa có hóa đơn!");
                return;
            }

            var hdDich = hoaDonBUS.GetBillByBan(maBanDich);
            if (hdDich == null)
            {
                int maHD = hoaDonBUS.AddHoaDon(maBanDich);
                hdDich = hoaDonBUS.GetById(maHD);
            }

            using (var db = new Model1())
            {
                var chiTietNguon = db.ChiTietHoaDon.Where(c => c.MaHD == hdNguon.MaHD).ToList();

                foreach (var ct in chiTietNguon)
                {
                    var ctDich = db.ChiTietHoaDon.FirstOrDefault(c => c.MaHD == hdDich.MaHD && c.MaMon == ct.MaMon);
                    if (ctDich != null)
                        ctDich.SoLuong += ct.SoLuong;
                    else
                        db.ChiTietHoaDon.Add(new ChiTietHoaDon
                        {
                            MaHD = hdDich.MaHD,
                            MaMon = ct.MaMon,
                            SoLuong = ct.SoLuong
                        });
                }

                db.ChiTietHoaDon.RemoveRange(chiTietNguon);
                db.HoaDon.Remove(db.HoaDon.Find(hdNguon.MaHD));
                db.SaveChanges();

                hoaDonBUS.UpdateTongTien(hdDich.MaHD);
            }

            ShowBill(maBanDich);
            MessageBox.Show("Ghép bàn thành công!");
            CapNhatMauBan();
        }

        private void btnTachBan_Click(object sender, EventArgs e)
        {
            if (maBanHienTai == 0)
            {
                MessageBox.Show("Vui lòng chọn bàn!");
                return;
            }

            FormTachBan f = new FormTachBan();
            f.MaBanHienTai = maBanHienTai;
            f.ShowDialog();
            ShowBill(maBanHienTai);
            CapNhatMauBan();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn thoát?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Exit();
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            new FormNhanVien().Show();
        }

        private void btnThucDon_Click(object sender, EventArgs e)
        {
            new FormThucDon().Show();
        }

        private void btnDoanhThu_Click(object sender, EventArgs e)
        {
            new FormDoanhThu().Show();
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            if (maBanHienTai <= 0)
            {
                MessageBox.Show("Vui lòng chọn bàn!");
                return;
            }

            var hd = hoaDonBUS.GetBillByBan(maBanHienTai);
            if (hd == null)
            {
                MessageBox.Show("Bàn này chưa có hóa đơn!");
                return;
            }

            // Xác nhận thanh toán
            if (MessageBox.Show("Xác nhận thanh toán cho bàn " + maBanHienTai + "?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            // Hỏi xem có dùng voucher không
            DialogResult dr = MessageBox.Show("Khách hàng có muốn sử dụng voucher/khuyến mãi không?", "Voucher", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                // Mở form khuyến mãi
                FormKhuyenMai fkm = new FormKhuyenMai();
                fkm.MaBan = maBanHienTai; // nếu FormKhuyenMai có thuộc tính MaBan
                fkm.ShowDialog();
            }
            else
            {
                // Cập nhật hóa đơn đã thanh toán
                hoaDonBUS.ThanhToan(hd.MaHD);

                // Mở form in hóa đơn
                FormHoaDon fhd = new FormHoaDon(maBanHienTai);
                fhd.ShowDialog();

                MessageBox.Show("Thanh toán và in hóa đơn thành công!");
            }

            // Làm mới giao diện
            lvHoaDon.Items.Clear();
            txtTongTien.Text = "0";
            maBanHienTai = 0;
            CapNhatMauBan();
        }

        void CapNhatMauBan()
        {
            using (var db = new Model1())
            {
                for (int i = 1; i <= 12; i++)
                {
                    Button btn = (Button)this.Controls["btnBan" + i];
                    if (btn != null)
                    {
                        var hoaDon = db.HoaDon.FirstOrDefault(h => h.MaBan == i && h.TrangThai == false);

                        if (hoaDon != null)
                            btn.BackColor = System.Drawing.Color.SaddleBrown; // Bàn đang có hóa đơn
                        else
                            btn.BackColor = System.Drawing.Color.Peru; // Bàn trống
                    }
                }
            }
        }
    }
}
