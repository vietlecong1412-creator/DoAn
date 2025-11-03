using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DoAn_DAL.Model
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Ban> Ban { get; set; }
        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDon { get; set; }
        public virtual DbSet<HoaDon> HoaDon { get; set; }
        public virtual DbSet<LoaiMon> LoaiMon { get; set; }
        public virtual DbSet<Mon> Mon { get; set; }
        public virtual DbSet<NhanVien> NhanVien { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoaDon>()
                .HasMany(e => e.ChiTietHoaDon)
                .WithRequired(e => e.HoaDon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoaiMon>()
                .HasMany(e => e.Mon)
                .WithRequired(e => e.LoaiMon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mon>()
                .HasMany(e => e.ChiTietHoaDon)
                .WithRequired(e => e.Mon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhanVien>()
                .Property(e => e.DienThoai)
                .IsUnicode(false);
        }
    }
}
