using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormThucDon : Form
    {
        public FormThucDon()
        {
            InitializeComponent();
        }

        private void FormThucDon_Load(object sender, EventArgs e)
        {
            ReloadMenu();

            if (FormLogin.Role == "nhanvien")
                btnDieuChinh.Enabled = false;
        }

        public void ReloadMenu()
        {
            LoadListView(lvCaPhe, 1);
            LoadListView(lvTraSua, 2);
            LoadListView(lvNuocEp, 3);
            LoadListView(lvTraTraiCay, 4);
            LoadListView(lvSinhTo, 5);
            LoadListView(lvKhac, 6);
        }

        public void LoadListView(ListView lv, int maLoai)
        {
            lv.Items.Clear();
            lv.Columns.Clear();

            lv.View = View.Details;
            lv.Columns.Add("Tên món", 150);
            lv.Columns.Add("Giá", 80);

            DataTable dt = GetMonTheoLoai(maLoai);

            foreach (DataRow row in dt.Rows)
            {
                ListViewItem item = new ListViewItem(row["TenMon"].ToString());
                item.SubItems.Add(string.Format("{0:N0} đ", row["DonGia"]));
                lv.Items.Add(item);
            }
        }

        public DataTable GetMonTheoLoai(int maLoai)
        {
            string connStr = @"Data Source=localhost;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT TenMon, DonGia FROM Mon WHERE MaLoai = @MaLoai AND DaXoa = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@MaLoai", maLoai);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private void btnDieuChinh_Click(object sender, EventArgs e)
        {
            using (FormQuanLyThucDon f = new FormQuanLyThucDon())
            {
                f.ShowDialog(); // Khi tắt form quản lý thực đơn

                // Load lại từng ListView
                LoadListView(lvCaPhe, 1);
                LoadListView(lvTraSua, 2);
                LoadListView(lvNuocEp, 3);
                LoadListView(lvTraTraiCay, 4);
                LoadListView(lvSinhTo, 5);
                LoadListView(lvKhac, 6);
            }
        }
    }
}
