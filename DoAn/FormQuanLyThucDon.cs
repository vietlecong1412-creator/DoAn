using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormQuanLyThucDon : Form
    {
        public FormQuanLyThucDon()
        {
            InitializeComponent();
        }

        private void FormQuanLyThucDon_Load(object sender, EventArgs e)
        {
            LoadLoaiMon();
            LoadDanhSachMon();
        }

        private void LoadLoaiMon()
        {
            string connStr = @"Data Source=localhost;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT MaLoai, TenLoai FROM LoaiMon";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbLoai.DataSource = dt;
                cbLoai.DisplayMember = "TenLoai";
                cbLoai.ValueMember = "MaLoai";
            }
        }

        private void LoadDanhSachMon()
        {
            string connStr = @"Data Source=localhost;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT m.MaMon, m.TenMon, l.TenLoai, m.DonGia 
                 FROM Mon m 
                 INNER JOIN LoaiMon l ON m.MaLoai = l.MaLoai
                 WHERE m.DaXoa = 0"; // chỉ hiển thị món chưa bị ẩn

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvMenu.DataSource = dt;
                dgvMenu.Columns["MaMon"].HeaderText = "Mã món";
                dgvMenu.Columns["TenMon"].HeaderText = "Tên món";
                dgvMenu.Columns["TenLoai"].HeaderText = "Loại món";
                dgvMenu.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvMenu.Columns["DonGia"].DefaultCellStyle.Format = "N0"; // định dạng số
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtTenMon.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập tên món!");
                return;
            }

            decimal donGia = 0;
            decimal.TryParse(txtDonGia.Text.Trim(), out donGia);

            string connStr = @"Data Source=localhost;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string insert = "INSERT INTO Mon (TenMon, MaLoai, DonGia) VALUES (@TenMon, @MaLoai, @DonGia)";
                SqlCommand cmd = new SqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("@TenMon", txtTenMon.Text);
                cmd.Parameters.AddWithValue("@MaLoai", cbLoai.SelectedValue);
                cmd.Parameters.AddWithValue("@DonGia", donGia);
                cmd.ExecuteNonQuery();
            }

            LoadDanhSachMon();
            txtTenMon.Clear();
            txtDonGia.Clear();
        }

        private void dgvMenu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtTenMon.Text = dgvMenu.Rows[e.RowIndex].Cells["TenMon"].Value.ToString();
                cbLoai.Text = dgvMenu.Rows[e.RowIndex].Cells["TenLoai"].Value.ToString();
                txtDonGia.Text = dgvMenu.Rows[e.RowIndex].Cells["DonGia"].Value.ToString();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Hãy nhập tên món cần sửa!");
                return;
            }

            decimal donGia = 0;
            decimal.TryParse(txtDonGia.Text.Trim(), out donGia);

            string connStr = @"Data Source=localhost;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string update = "UPDATE Mon SET MaLoai=@MaLoai, DonGia=@DonGia WHERE TenMon=@TenMon";
                SqlCommand cmd = new SqlCommand(update, conn);
                cmd.Parameters.AddWithValue("@TenMon", txtTenMon.Text);
                cmd.Parameters.AddWithValue("@MaLoai", cbLoai.SelectedValue);
                cmd.Parameters.AddWithValue("@DonGia", donGia);
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    MessageBox.Show("Không tìm thấy món để sửa!");
                }
                else
                {
                    MessageBox.Show("Cập nhật món thành công!");
                }
            }

            LoadDanhSachMon();
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            string tenMon = txtTenMon.Text.Trim();
            if (string.IsNullOrEmpty(tenMon))
            {
                MessageBox.Show("Hãy nhập tên món cần xóa!");
                return;
            }

            string connStr = @"Data Source=localhost;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Cập nhật DaXoa = 1 bất kể món đang tồn tại trong hóa đơn hay không
                string updateQuery = "UPDATE Mon SET DaXoa = 1 WHERE TenMon = @TenMon";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@TenMon", tenMon);
                int rows = updateCmd.ExecuteNonQuery();

                if (rows > 0)
                    MessageBox.Show("Món đã được ẩn thành công!");
                else
                    MessageBox.Show("Không tìm thấy món để ẩn!");
            }

            // Tải lại danh sách món, sẽ tự động ẩn món có DaXoa = 1
            LoadDanhSachMon();
            txtTenMon.Clear();
            txtDonGia.Clear();
        }

    }
}
