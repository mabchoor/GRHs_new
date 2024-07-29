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

namespace GRHs.Admin
{
    public partial class Certificates : UserControl
    {
        private UserAccount _userAccount;
        public Certificates()
        {
            InitializeComponent();
            _userAccount = new UserAccount();
            LoadComboBoxes();
        }

        private void Certificates_Load(object sender, EventArgs e)
        {

        }
        private void ApplyFilters()
        {
            try
            {
                // Get the search text
                string searchText = rechercher.Text.Trim().ToLower();

                // Get selected values from ComboBoxes
                int selectedDepartmentID = (int)ComboBox_Departements.SelectedValue;
                int selectedPositionID = (int)ComboBox_Positions.SelectedValue;
              

                // Fetch all employees including related entities
                var employees = _userAccount.DbContext.Employees
                    .Include(emp => emp.User)        // Eager load User details
                    .Include(emp => emp.Position)    // Eager load Position details
                    .Include(emp => emp.Department)  // Eager load Department details
                    .ToList();

                // Filter employees based on search text, department, position, and leave status
                var filteredEmployees = employees.Where(employee =>
                    (string.IsNullOrEmpty(searchText) ||
                     (employee.User?.Name ?? "").ToLower().Contains(searchText) ||
                     (employee.Position?.PositionName ?? "").ToLower().Contains(searchText) ||
                     (employee.Department?.DepartementName ?? "").ToLower().Contains(searchText) ||
                     (employee.CIN ?? "").ToLower().Contains(searchText) ||
                     (employee.EmployeeNumber ?? "").ToLower().Contains(searchText)) &&
                    (selectedDepartmentID == -1 || employee.DepartmentID == selectedDepartmentID) &&
                    (selectedPositionID == -1 || employee.PositionID == selectedPositionID)
                ).ToList();

                // Prepare the DataTable for DataGridView
                var dataTable = new DataTable();
                dataTable.Columns.Add("Employee ID", typeof(int)); // Column header title
                dataTable.Columns.Add("Name", typeof(string)); // Column header title
                dataTable.Columns.Add("Position", typeof(string)); // Column header title
                dataTable.Columns.Add("Department", typeof(string)); // Column header title
                dataTable.Columns.Add("CIN", typeof(string)); // Column header title
                dataTable.Columns.Add("Phone Number", typeof(string)); // Column header title
               
               
               

                // Populate the DataTable
                foreach (var employee in filteredEmployees)
                {
                    // Use null-coalescing operators to safely handle potential null values
                    var employeeName = employee.User?.Name ?? "N/A"; // Handle null User
                    var positionName = employee.Position?.PositionName ?? "N/A"; // Handle null Position
                    var departmentName = employee.Department?.DepartementName ?? "N/A"; // Handle null Department
                    var cin = employee.CIN ?? "N/A"; // Handle null CIN
                    var phoneNumber = employee.EmployeeNumber ?? "N/A"; // Handle null PhoneNumber

                    dataTable.Rows.Add(
                        employee.EmployeeID,
                        employeeName,
                        positionName,
                        departmentName,
                        cin,
                        phoneNumber,
                        _userAccount.DbContext.Leaves
                            .Any(l => l.EmployeeID == employee.EmployeeID && l.StartDate <= DateTime.Now && l.EndDate >= DateTime.Now)
                            ? "Yes"
                            : "No"
                    );
                }

                // Bind the DataTable to the DataGridView
                EmployeeView.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while applying filters: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadComboBoxes()
        {
            try
            {
                // Load departments
                var departments = _userAccount.DbContext.Departements
                    .Select(d => new Departement
                    {
                        DepartementID = d.DepartementID,
                        DepartementName = d.DepartementName
                    })
                    .ToList();

                departments.Insert(0, new Departement { DepartementID = -1, DepartementName = "All" });

                var departmentBindingSource = new BindingSource();
                departmentBindingSource.DataSource = departments;
                ComboBox_Departements.DataSource = departmentBindingSource;
                ComboBox_Departements.DisplayMember = "DepartementName";
                ComboBox_Departements.ValueMember = "DepartementID";

                // Load positions
                var positions = _userAccount.DbContext.Positions
                    .Select(p => new Position
                    {
                        PositionID = p.PositionID,
                        PositionName = p.PositionName
                    })
                    .ToList();

                positions.Insert(0, new Position { PositionID = -1, PositionName = "All" });

                var positionBindingSource = new BindingSource();
                positionBindingSource.DataSource = positions;
                ComboBox_Positions.DataSource = positionBindingSource;
                ComboBox_Positions.DisplayMember = "PositionName";
                ComboBox_Positions.ValueMember = "PositionID";

                

             
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading departments or positions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBox_Departements_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void rechercher_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ComboBox_Positions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
    }


}
