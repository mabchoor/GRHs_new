using GRHs.authentication;
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

namespace GRHs.Admin
{
    public partial class Dashboard : UserControl
    {
        private UserAccount _userAccount;
        public Dashboard()
        {
            InitializeComponent();
            // Initialize UserAccount with your DbContext
            _userAccount = new UserAccount();
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void dashboard_TE_Click(object sender, EventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            dashboard_TE.Text = GetTotalNumberOfEmployees().ToString();
            dashboard_IE.Text = GetNumberOfEmployeesOnLeave().ToString();
            dashboard_AE.Text = GetNumberOfEmployeesNotOnLeave().ToString();
        }
        public int GetTotalNumberOfEmployees()
        {
            // Use LINQ to count the total number of employees
            return _userAccount.DbContext.Employees.Count();
        }

        public int GetNumberOfEmployeesOnLeave()
        {
            var now = DateTime.Now;

            // Query to get the count of employees who are currently on leave
            var numberOfEmployeesOnLeave = _userAccount.DbContext.Leaves
                .Where(l => l.StartDate <= now && l.EndDate >= now)
                .Select(l => l.EmployeeID)
                .Distinct()
                .Count();

            return numberOfEmployeesOnLeave;
        }

        public int GetNumberOfEmployeesNotOnLeave()
        {
            var now = DateTime.Now;

            // Query to get the employee IDs who are currently on leave
            var employeeIdsOnLeave = _userAccount.DbContext.Leaves
                .Where(l => l.StartDate <= now && l.EndDate >= now)
                .Select(l => l.EmployeeID)
                .Distinct()
                .ToList();

            // Query to get the total number of employees who are not on leave
            var numberOfEmployeesNotOnLeave = _userAccount.DbContext.Employees
                .Count(e => !employeeIdsOnLeave.Contains(e.EmployeeID));

            return numberOfEmployeesNotOnLeave;
        }

    }
}
