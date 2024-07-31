
using GRHs.authentication;
using GRHs.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRHs.User
{
    public partial class UserDashboard : Form
    {
        private Users _user;
        private UserAccount _userAccount;

        public UserDashboard(int userID)
        {
            InitializeComponent();

            // Initialize UserAccount with your DbContext
            _userAccount = new UserAccount();

            // Fetch the user from the database using userID
            _user = _userAccount.DbContext.Users
             .FirstOrDefault(u => u.UserID == userID);

            if (_user == null)
            {
                MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
          
        }

        private void dashboard_btn_Click(object sender, EventArgs e)
        {
            AddUserControl(new User_Dash(_user.UserID));
        }

        private void AddUserControl(UserControl userControl)
        {
            panel3.Controls.Clear();
            userControl.Dock = DockStyle.Fill; // Optional: Makes the UserControl fill the panel
            panel3.Controls.Add(userControl);
        }

        private void salary_btn_Click(object sender, EventArgs e)
        {
            AddUserControl(new MesCertifications(_user.UserID));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUserControl( new MesLeaves(_user.UserID));
        }

        private void guna2GradientButton4_Click(object sender, EventArgs e)
        {
            //AddUserControl( new Holidays(_user.UserID));
        }
        private int GetTotalAttestations(int userId)
        {
            try
            {
                return _userAccount.DbContext.Attestations
                    .Count(a => a.EmployeeID == userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching attestations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }



        private void label4_Click(object sender, EventArgs e)
        {
            this.Hide();
            var login = new GRHs.authentication.login();
            login.Show();
        }
    }
}
