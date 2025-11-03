namespace DoAn_DAL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Mon")]
    public partial class Mon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mon()
        {
            ChiTietHoaDon = new HashSet<ChiTietHoaDon>();
        }

        [Key]
        public int MaMon { get; set; }

        [Required]
        [StringLength(150)]
        public string TenMon { get; set; }

        public long? DonGia { get; set; }

        public int MaLoai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDon { get; set; }

        public virtual LoaiMon LoaiMon { get; set; }
    }
}
