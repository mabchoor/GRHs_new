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
            if (string.IsNullOrEmpty(pseudo))
            {
                    MessageBox.Show("Password is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
             }

            // Create an instance of UserAccount
            // Assuming _userAccount is properly initialized elsewhere in your code
            var (user, userSession, message) = await _userAccount.LoginAsync(pseudo, password);

            if (user == null || userSession == null)
            {
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Login successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Optionally, you can store the session or proceed to the main application form

                // Check user role and navigate to the appropriate dashboard
                if (user.Role == 0) // Assuming 1 is the role for a regular user
                {
                    // Open User Dashboard
                    var userDashboard = new UserDashboard(); // Replace with your actual user dashboard form
                    this.Hide();
                    userDashboard.Show();

                }
                else if (user.Role == 1) // Assuming 2 is the role for an admin
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
            Register loginForm = new Register();
            loginForm.Show();
            this.Hide();
        }

        private void login_Load(object sender, EventArgs e)
        {

        }
    }
}
