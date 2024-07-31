using GRHs.Admin;
using GRHs.Data;
using GRHs.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRHs.authentication
{
    public partial class login : Form
    {
        private UserAccount _userAccount;
        public login()
        {
            InitializeComponent();
            // Initialize UserAccount with your DbContext
            _userAccount = new UserAccount();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void login_showPass_CheckedChanged(object sender, EventArgs e)
        {
            login_password.PasswordChar = login_showPass.Checked ? '\0' : '*';
        }

        private async void login_btn_Click(object sender, EventArgs e)
        {
            string pseudo = login_username.Text; // Assume this textbox can contain either username or email
            string password = login_password.Text;

            if (string.IsNullOrEmpty(pseudo))
            {
                MessageBox.Show("Username or email is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure _userAccount is properly initialized
            if (_userAccount == null)
            {
                _userAccount = new UserAccount(); // Replace this with your actual initialization if necessary
            }

            // Call the asynchronous login method
            var (user, userSession, message) = await _userAccount.LoginAsync(pseudo, password);

            if (user == null || userSession == null)
            {
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Check user role and navigate to the appropriate dashboard
                if (user.Role == 0) // Assuming 0 is the role for a regular user
                {
                    // Open User Dashboard
                    var userDashboard = new UserDashboard(user.UserID); // Replace with your actual user dashboard form
                    this.Hide();
                    userDashboard.Show();
                }
                else if (user.Role == 1) // Assuming 1 is the role for an admin
                {
                    // Open Admin Dashboard
                    var adminDashboard = new AdminDashboard(); // Replace with your actual admin dashboard form
                    this.Hide();
                    adminDashboard.Show();
                }
                else
                {
                    MessageBox.Show("Unknown role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void login_signupBtn_Click(object sender, EventArgs e)
        {
         
        }

        private void login_Load(object sender, EventArgs e)
        {
           
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {
            Register loginForm = new Register();
            loginForm.Show();
            this.Hide();
        }
    }
}
