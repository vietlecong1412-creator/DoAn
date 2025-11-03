using DoAn_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAn_BUS
{
    public class LoaiMonBUS
    {
        private Model1 db = new Model1();

        // 🔹 Lấy danh sách tất cả loại món
        public List<LoaiMon> GetAll()
        {
            return db.LoaiMon.ToList();
        }

        // 🔹 Tìm theo mã loại
        public LoaiMon GetById(int maLoai)
        {
            return db.LoaiMon.Find(maLoai);
        }

        // 🔹 Thêm loại món mới
        public bool Add(LoaiMon loai)
        {
            try
            {
                // Kiểm tra trùng tên loại
                if (db.LoaiMon.Any(l => l.TenLoai == loai.TenLoai))
                    return false;

                db.LoaiMon.Add(loai);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 🔹 Cập nhật loại món
        public bool Update(LoaiMon loai)
        {
            try
            {
                var item = db.LoaiMon.Find(loai.MaLoai);
                if (item == null) return false;

                // Kiểm tra trùng tên (trừ chính nó)
                if (db.LoaiMon.Any(l => l.TenLoai == loai.TenLoai && l.MaLoai != loai.MaLoai))
                    return false;

                item.TenLoai = loai.TenLoai;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 🔹 Xóa loại món
        public bool Delete(int maLoai)
        {
            try
            {
                var item = db.LoaiMon.Find(maLoai);
                if (item == null) return false;

                // Nếu loại món đang được dùng trong bảng Mon thì không xóa
                if (db.Mon.Any(m => m.MaLoai == maLoai))
                    return false;

                db.LoaiMon.Remove(item);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
