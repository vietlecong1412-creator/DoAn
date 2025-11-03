using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoAn_BUS;
using DoAn_DAL.Model;

namespace DoAn
{
    public partial class FormNhanVien : Form
    {
        NhanVienBUS nvBUS = new NhanVienBUS();

        public FormNhanVien()
        {
            InitializeComponent();
        }

        private void FormNhanVien_Load(object sender, EventArgs e)
        {
            LoadNhanVien();

            if (FormLogin.Role == "nhanvien")
            {
                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void LoadNhanVien()
        {
            var data = nvBUS.GetAll()
        .Select(n => new
        {
            n.MaNV,
            n.HoTen,
            n.DienThoai,
            n.DiaChi,
            n.NgaySinh
        }).ToList();

            dgvNhanVien.DataSource = data;

        }

        private void dgvNhanVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtMaNV.Text = dgvNhanVien.CurrentRow.Cells["MaNV"].Value.ToString();
                txtHoTen.Text = dgvNhanVien.CurrentRow.Cells["HoTen"].Value.ToString();
                txtDienThoai.Text = dgvNhanVien.CurrentRow.Cells["DienThoai"].Value.ToString();
                txtDiaChi.Text = dgvNhanVien.CurrentRow.Cells["DiaChi"].Value.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(dgvNhanVien.CurrentRow.Cells["NgaySinh"].Value);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtHoTen.Text == "" || txtDienThoai.Text == "" || txtDiaChi.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            NhanVien nv = new NhanVien
            {
                HoTen = txtHoTen.Text,
                DienThoai = txtDienThoai.Text,
                DiaChi = txtDiaChi.Text,
                NgaySinh = dtpNgaySinh.Value
            };

            nvBUS.AddNhanVien(nv);
            LoadNhanVien();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtMaNV.Text == "")
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!");
                return;
            }

            NhanVien nv = new NhanVien
            {
                MaNV = int.Parse(txtMaNV.Text),
                HoTen = txtHoTen.Text,
                DienThoai = txtDienThoai.Text,
                DiaChi = txtDiaChi.Text,
                NgaySinh = dtpNgaySinh.Value
            };

            nvBUS.UpdateNhanVien(nv);
            LoadNhanVien();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtMaNV.Text == "")
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!");
                return;
            }

            int maNV = int.Parse(txtMaNV.Text);
            nvBUS.DeleteNhanVien(maNV);
            LoadNhanVien();
        }
    }
}
