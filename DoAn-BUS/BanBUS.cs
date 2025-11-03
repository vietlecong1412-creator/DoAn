using DoAn_DAL.Model;
using System.Collections.Generic;
using System.Linq;

namespace DoAn_BUS
{
    public class BanBUS
    {
        private Model1 db = new Model1();

        // Lấy tất cả bàn
        public List<Ban> GetAll()
        {
            return db.Ban.ToList();
        }

        // Lấy bàn theo mã
        public Ban GetById(int maBan)
        {
            return db.Ban.Find(maBan);
        }

        // Thêm bàn mới
        public bool AddBan(Ban ban)
        {
            try
            {
                db.Ban.Add(ban);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật bàn
        public bool UpdateBan(Ban ban)
        {
            var item = db.Ban.Find(ban.MaBan);
            if (item == null) return false;

            item.TenBan = ban.TenBan;
            item.TrangThai = ban.TrangThai;

            db.SaveChanges();
            return true;
        }

        // Xóa bàn (không được xóa nếu có hóa đơn chưa thanh toán)
        public bool DeleteBan(int maBan)
        {
            var ban = db.Ban.Find(maBan);
            if (ban == null) return false;

            // Kiểm tra bàn có hóa đơn chưa thanh toán hay không
            var hd = db.HoaDon.FirstOrDefault(h => h.MaBan == maBan);
            if (hd != null) return false; // Có hóa đơn → Không xóa bàn

            db.Ban.Remove(ban);
            db.SaveChanges();
            return true;
        }

        // ✅ Đổi trạng thái bàn (Trống / Có khách)
        public bool SetTrangThai(int maBan, string trangThai)
        {
            var ban = db.Ban.Find(maBan);
            if (ban == null) return false;

            ban.TrangThai = trangThai;
            db.SaveChanges();
            return true;
        }
    }
}
