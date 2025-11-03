using DoAn_DAL.Model;
using System.Collections.Generic;
using System.Linq;

namespace DoAn_BUS
{
    public class ChiTietHoaDonBUS
    {
        private Model1 db = new Model1();

        // 🔹 Lấy toàn bộ chi tiết (thường để test)
        public List<ChiTietHoaDon> GetAll()
        {
            return db.ChiTietHoaDon.ToList();
        }

        // 🔹 Lấy danh sách chi tiết theo hóa đơn
        public List<ChiTietHoaDon> GetByHoaDon(int maHD)
        {
            return db.ChiTietHoaDon.Where(ct => ct.MaHD == maHD).ToList();
        }

        // 🔹 Lấy một dòng chi tiết theo MaHD + MaMon
        public ChiTietHoaDon GetDetail(int maHD, int maMon)
        {
            return db.ChiTietHoaDon.Find(maHD, maMon);
        }

        // 🔹 Thêm chi tiết hóa đơn
        public bool Add(ChiTietHoaDon ct)
        {
            // Kiểm tra nếu món đã có trong hoá đơn => tăng số lượng
            var existing = db.ChiTietHoaDon.Find(ct.MaHD, ct.MaMon);
            if (existing != null)
            {
                existing.SoLuong += ct.SoLuong;

                // Cập nhật giá nếu cần (chuyển decimal -> long nếu phát sinh)
                if (ct.DonGia.HasValue)
                    existing.DonGia = ct.DonGia;
            }
            else
            {
                // Đảm bảo DonGia là kiểu long?
                if (ct.DonGia == null)
                    ct.DonGia = 0;

                db.ChiTietHoaDon.Add(ct);
            }

            db.SaveChanges();
            return true;
        }

        // 🔹 Cập nhật chi tiết hóa đơn
        public bool Update(ChiTietHoaDon ct)
        {
            var item = db.ChiTietHoaDon.Find(ct.MaHD, ct.MaMon);
            if (item == null) return false;

            item.SoLuong = ct.SoLuong;
            item.DonGia = ct.DonGia ?? 0; // tránh null

            db.SaveChanges();
            return true;
        }

        // 🔹 Xóa 1 món trong hóa đơn
        public bool Delete(int maHD, int maMon)
        {
            var item = db.ChiTietHoaDon.Find(maHD, maMon);
            if (item == null) return false;

            db.ChiTietHoaDon.Remove(item);
            db.SaveChanges();
            return true;
        }

        // 🔹 Xóa toàn bộ chi tiết của hóa đơn
        public bool DeleteByHoaDon(int maHD)
        {
            var items = db.ChiTietHoaDon.Where(ct => ct.MaHD == maHD).ToList();
            db.ChiTietHoaDon.RemoveRange(items);
            db.SaveChanges();
            return true;
        }
    }
}
