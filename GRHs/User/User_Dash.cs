
using GRHs.authentication;
using GRHs.Calendar;
using GRHs.Entities;
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
    public partial class User_Dash : UserControl
    {

        private Users _user;
        private UserAccount _userAccount;
        private Employee _employee;

        public User_Dash(int userID)
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

            // Fetch the employee from the database using UserID
            _employee = _userAccount.DbContext.Employees
                .FirstOrDefault(e => e.UserID == userID);

            if (_employee == null)
            {
                MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void User_Dash_Load(object sender, EventArgs e)
        {
            dashboard_AE.Text=GetTotalAttestations().ToString();
            dashboard_IE.Text=GetTotalLeaves().ToString();
            var leaveData = LoadAllLeaves(); // Get the leave data
            CalendarView calendar = new CalendarView(leaveData);
            panel2.Controls.Clear();
            calendar.Dock = DockStyle.Fill; // Optional: Makes the UserControl fill the panel
            panel2.Controls.Add(calendar);
        }

        private void dashboard_AE_Click(object sender, EventArgs e)
        {

        }

        private int GetTotalAttestations()
        {
            try
            {
                // Check if the employee object is initialized
                if (_employee == null)
                {
                    MessageBox.Show("Employee information is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }

                // Get the total number of attestations for the employee
                int totalAttestations = _userAccount.DbContext.Attestations
                    .Count(a => a.EmployeeID == _employee.EmployeeID);

                return totalAttestations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching attestations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private int GetTotalLeaves()
        {
            try
            {
                // Check if the employee object is initialized
                if (_employee == null)
                {
                    MessageBox.Show("Employee information is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }

                // Get the total number of leaves for the employee
                int totalLeaves = _userAccount.DbContext.Leaves
                    .Count(l => l.EmployeeID == _employee.EmployeeID);

                return totalLeaves;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching leaves: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }



        private List<Tuple<DateTime, DateTime>> LoadAllLeaves()
        {
            try
            {
                // Check if the employee object is initialized
                if (_employee == null)
                {
                    MessageBox.Show("Employee information is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Tuple<DateTime, DateTime>>();
                }

                // Fetch leaves for the employee from the database
                var leaves = _userAccount.DbContext.Leaves
                    .Where(l => l.EmployeeID == _employee.EmployeeID)
                    .ToList();

                // Create a list to hold the start and end dates
                List<Tuple<DateTime, DateTime>> leaveDetails = new List<Tuple<DateTime, DateTime>>();

                // Process each leave record
                foreach (var leave in leaves)
                {
                    // Calculate the end date
                    DateTime endDate = leave.StartDate.AddDays(leave.NumberOfDays - 1);

                    // Add the leave details to the list
                    leaveDetails.Add(new Tuple<DateTime, DateTime>(leave.StartDate, endDate));
                }

                return leaveDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching leaves: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Tuple<DateTime, DateTime>>();
            }
        }
    }
}
