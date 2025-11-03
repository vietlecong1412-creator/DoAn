using DoAn_DAL.Model;
using System.Collections.Generic;
using System.Linq;

namespace DoAn_BUS
{
    public class NhanVienBUS
    {
        private Model1 db = new Model1();

        // Lấy tất cả nhân viên
        public List<NhanVien> GetAll()
        {
            return db.NhanVien.ToList();
        }

        // Tìm nhân viên theo mã
        public NhanVien GetById(int maNV)
        {
            return db.NhanVien.Find(maNV);
        }

        // Thêm nhân viên
        public bool AddNhanVien(NhanVien nv)
        {
            db.NhanVien.Add(nv);
            db.SaveChanges();
            return true;
        }

        // Cập nhật nhân viên
        public bool UpdateNhanVien(NhanVien nv)
        {
            var nhanVien = db.NhanVien.Find(nv.MaNV);
            if (nhanVien == null) return false;

            nhanVien.HoTen = nv.HoTen;
            nhanVien.NgaySinh = nv.NgaySinh;
            nhanVien.DiaChi = nv.DiaChi;
            nhanVien.DienThoai = nv.DienThoai;

            db.SaveChanges();
            return true;
        }

        // Xóa nhân viên
        public bool DeleteNhanVien(int maNV)
        {
            var nhanVien = db.NhanVien.Find(maNV);
            if (nhanVien == null) return false;

            db.NhanVien.Remove(nhanVien);
            db.SaveChanges();
            return true;
        }
    }
}
