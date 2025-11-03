using DoAn_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAn_BUS
{
    public class HoaDonBUS
    {
        private Model1 db = new Model1();

        // ✅ Lấy tất cả hóa đơn
        public List<HoaDon> GetAll()
        {
            return db.HoaDon.ToList();
        }

        // ✅ Lấy hóa đơn theo mã
        public HoaDon GetById(int maHD)
        {
            return db.HoaDon.Find(maHD);
        }

        // ✅ Lấy hóa đơn chưa thanh toán theo bàn (mới nhất)
        public HoaDon GetBillByBan(int maBan)
        {
            return db.HoaDon
                     .Where(h => h.MaBan == maBan && h.TrangThai == false)
                     .OrderByDescending(h => h.MaHD)
                     .FirstOrDefault();
        }

        // ✅ Thêm hóa đơn mới
        public int AddHoaDon(int maBan)
        {
            HoaDon hd = new HoaDon
            {
                NgayLap = null,
                TongTien = 0,
                MaBan = maBan,
                TrangThai = false
            };

            db.HoaDon.Add(hd);
            db.SaveChanges();
            return hd.MaHD;
        }

        // ✅ Cập nhật tổng tiền hóa đơn (dùng kiểu long)
        public bool UpdateTongTien(int maHD)
        {
            var hd = db.HoaDon.Find(maHD);
            if (hd == null) return false;

            var chiTiet = db.ChiTietHoaDon
                            .Where(ct => ct.MaHD == maHD)
                            .Select(ct => new
                            {
                                ct.SoLuong,
                                DonGia = ct.DonGia ?? 0
                            }).ToList();

            long tong = chiTiet.Sum(x => (long)x.SoLuong * x.DonGia);
            hd.TongTien = tong;

            db.SaveChanges();
            return true;
        }

        // ✅ Thanh toán hóa đơn
        public bool ThanhToan(int maHD)
        {
            var hd = db.HoaDon
                       .Include("ChiTietHoaDon")
                       .FirstOrDefault(h => h.MaHD == maHD);
            if (hd == null) return false;

            UpdateTongTien(maHD);

            hd.TrangThai = true;
            hd.NgayLap = DateTime.Now;
            db.SaveChanges();

            return true;
        }

        // ✅ Xóa hóa đơn
        public bool DeleteHoaDon(int maHD)
        {
            var hd = db.HoaDon.Find(maHD);
            if (hd == null) return false;

            db.ChiTietHoaDon.RemoveRange(hd.ChiTietHoaDon);
            db.HoaDon.Remove(hd);
            db.SaveChanges();
            return true;
        }

        // ✅ Thêm món vào hóa đơn
        public void ThemMon(int maBan, int maMon, int soLuong = 1)
        {
            var hd = GetBillByBan(maBan);
            if (hd == null)
            {
                int maHD = AddHoaDon(maBan);
                hd = db.HoaDon.Find(maHD);
            }

            var ct = db.ChiTietHoaDon.FirstOrDefault(x => x.MaHD == hd.MaHD && x.MaMon == maMon);
            if (ct != null)
            {
                ct.SoLuong += soLuong;
            }
            else
            {
                var mon = db.Mon.Find(maMon);
                ct = new ChiTietHoaDon
                {
                    MaHD = hd.MaHD,
                    MaMon = maMon,
                    SoLuong = soLuong,
                    DonGia = mon?.DonGia ?? 0 // ✅ DonGia đã là long
                };
                db.ChiTietHoaDon.Add(ct);
            }

            db.SaveChanges();
            UpdateTongTien(hd.MaHD);
        }
    }
}
