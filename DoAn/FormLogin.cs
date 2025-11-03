using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAn
{
    public partial class FormLogin : Form
    {
        public static string Role = "";

        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string user = txtTaiKhoan.Text.Trim();
            string pass = txtMatKhau.Text.Trim();

            if (user == "")
            {
                MessageBox.Show("Vui lòng nhập tài khoản!", "Thông báo");
                txtTaiKhoan.Focus();
                return;
            }

            if (pass == "")
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo");
                txtMatKhau.Focus();
                return;
            }

            // Tài khoản mẫu (có thể thay bằng DB sau này)
            Dictionary<string, string> accounts = new Dictionary<string, string>()
            {
                { "admin", "123" },
                { "nhanvien", "111" }
            };

            // Kiểm tra tài khoản & phân quyền
            if (accounts.ContainsKey(user) && accounts[user] == pass)
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo");

                // Gán Role
                if (user == "admin")
                    Role = "admin";
                else
                    Role = "nhanvien";

                FormMain frm = new FormMain();
                this.Hide();
                frm.Show();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi");
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtMatKhau.PasswordChar = '*';
            this.AcceptButton = btnDangNhap;
        }
    }
}
