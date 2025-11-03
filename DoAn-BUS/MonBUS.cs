using DoAn_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAn_BUS
{
    public class MonBUS
    {
        private Model1 db = new Model1();

        // 🔹 Lấy tất cả món ăn
        public List<Mon> GetAll()
        {
            return db.Mon.ToList();
        }

        // 🔹 Lấy danh sách món theo mã loại
        public List<Mon> GetByLoai(int maLoai)
        {
            return db.Mon.Where(m => m.MaLoai == maLoai).ToList();
        }

        // 🔹 Lấy món theo mã
        public Mon GetById(int maMon)
        {
            return db.Mon.Find(maMon);
        }

        // 🔹 Thêm món mới
        public bool Add(Mon mon)
        {
            try
            {
                // Kiểm tra trùng tên món
                if (db.Mon.Any(m => m.TenMon == mon.TenMon))
                    return false;

                db.Mon.Add(mon);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 🔹 Cập nhật món
        public bool Update(Mon mon)
        {
            try
            {
                var item = db.Mon.Find(mon.MaMon);
                if (item == null) return false;

                item.TenMon = mon.TenMon;
                item.DonGia = mon.DonGia;
                item.MaLoai = mon.MaLoai;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 🔹 Xóa món
        public bool Delete(int maMon)
        {
            try
            {
                var item = db.Mon.Find(maMon);
                if (item == null) return false;

                db.Mon.Remove(item);
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
