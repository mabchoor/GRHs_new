using ClosedXML.Excel;
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
    public partial class Employees : UserControl
    {
        private UserAccount _userAccount;
        public Employees()
        {
           

            InitializeComponent();
            // Initialize UserAccount with your DbContext
            _userAccount = new UserAccount();
            LoadComboBoxes();

        }

        private void Employees_Load(object sender, EventArgs e)
        {
            rechercher.TextChanged += new EventHandler(rechercher_TextChanged);
            LoadEmployeeData();
         
          

        }
        public void LoadEmployeeData()
        {
            var now = DateTime.Now;

            // Fetch all employees including related entities
            var employees = _userAccount.DbContext.Employees
                .Include(e => e.User)        // Eager load User details
                .Include(e => e.Position)    // Eager load Position details
                .Include(e => e.Department)  // Eager load Department details
                .ToList();

            // Prepare the DataTable for DataGridView
            var dataTable = new DataTable();
            dataTable.Columns.Add("Employee ID", typeof(int)); // Column header title
            dataTable.Columns.Add("Name", typeof(string)); // Column header title
            dataTable.Columns.Add("Position", typeof(string)); // Column header title
            dataTable.Columns.Add("Department", typeof(string)); // Column header title
            dataTable.Columns.Add("Start Date", typeof(DateTime)); // Column header title
            dataTable.Columns.Add("End Date", typeof(DateTime)); // Column header title
            dataTable.Columns.Add("Employee Number", typeof(string)); // Column header title
            dataTable.Columns.Add("CIN", typeof(string)); // Column header title
            dataTable.Columns.Add("On Leave", typeof(string)); // Column header title

            // Populate the DataTable
            foreach (var employee in employees)
            {
                // Use null-coalescing operators to safely handle potential null values
                var employeeName = employee.User?.Name ?? "N/A"; // Handle null User
                var positionName = employee.Position?.PositionName ?? "N/A"; // Handle null Position
                var departmentName = employee.Department?.DepartementName ?? "N/A"; // Handle null Department

                dataTable.Rows.Add(
                    employee.EmployeeID,
                    employeeName,
                    positionName,
                    departmentName,
                    employee.StartDate,
                    employee.EndDate ?? (object)DBNull.Value, // Handle nullable EndDate
                    employee.EmployeeNumber ?? "N/A", // Handle null EmployeeNumber
                    employee.CIN ?? "N/A", // Handle null CIN
                    _userAccount.DbContext.Leaves
                        .Any(l => l.EmployeeID == employee.EmployeeID && l.StartDate <= now && l.EndDate >= now)
                        ? "Yes"
                        : "No"
                );
            }

            // Bind the DataTable to the DataGridView
            EmployeeView.DataSource = dataTable;
        }

        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            // Ensure there's a selected row in the DataGridView
            if (EmployeeView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Confirm deletion
            var result = MessageBox.Show("Are you sure you want to delete the selected employee?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;

            // Get the selected row
            var selectedRow = EmployeeView.SelectedRows[0];

            // Retrieve the EmployeeID from the selected row
            var employeeID = (int)selectedRow.Cells["Employee ID"].Value;

            // Find the employee in the database
            var employee = _userAccount.DbContext.Employees.Find(employeeID);
            if (employee == null)
            {
                MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Remove the employee from the database
            _userAccount.DbContext.Employees.Remove(employee);

            // Save changes to the database
            _userAccount.DbContext.SaveChanges();

            // Reload the DataGridView
            LoadEmployeeData();

            // Optionally, inform the user
            MessageBox.Show("Employee deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_Employee add_Employee = new Add_Employee();
            add_Employee.Show();


        }

        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            // Ensure there's a selected row in the DataGridView
            if (EmployeeView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Confirm deletion
            var result = MessageBox.Show("Are you sure you want to delete the selected employee?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;

            // Get the selected row
            var selectedRow = EmployeeView.SelectedRows[0];

            // Retrieve the EmployeeID from the selected row
            var employeeID = (int)selectedRow.Cells["Employee ID"].Value;

           Update_employee add_Employee = new Update_employee(employeeID);
            add_Employee.Show();
        }

        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(EmployeeView);
        }



        public void ExportDataGridViewToExcel(DataGridView dataGridView)
        {
            // Create a new DataTable and fill it with the DataGridView data
            var dataTable = new DataTable();
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                dataTable.Columns.Add(column.HeaderText);
            }

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataRow dataRow = dataTable.NewRow();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        dataRow[cell.ColumnIndex] = cell.Value?.ToString() ?? ""; // Convert object to string
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            // Create a new Excel workbook and worksheet
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Load data from DataTable to worksheet
                worksheet.Cell(1, 1).InsertTable(dataTable);

                // Save the workbook to a file
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Save an Excel File";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void rechercher_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
       

        private void ComboBox_Departements_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ComboBox_Positions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ComboBox_onleave_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
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
                string onLeaveFilter = (ComboBox_onleave.SelectedItem as LeaveOption)?.Value; // Using LeaveOption value

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
                    (selectedPositionID == -1 || employee.PositionID == selectedPositionID) &&
                    (onLeaveFilter == "All" ||
                     (onLeaveFilter == "Yes" && _userAccount.DbContext.Leaves
                        .Any(l => l.EmployeeID == employee.EmployeeID && l.StartDate <= DateTime.Now && l.EndDate >= DateTime.Now)) ||
                     (onLeaveFilter == "No" && !_userAccount.DbContext.Leaves
                        .Any(l => l.EmployeeID == employee.EmployeeID && l.StartDate <= DateTime.Now && l.EndDate >= DateTime.Now)))
                ).ToList();

                // Prepare the DataTable for DataGridView
                var dataTable = new DataTable();
                dataTable.Columns.Add("Employee ID", typeof(int)); // Column header title
                dataTable.Columns.Add("Name", typeof(string)); // Column header title
                dataTable.Columns.Add("Position", typeof(string)); // Column header title
                dataTable.Columns.Add("Department", typeof(string)); // Column header title
                dataTable.Columns.Add("CIN", typeof(string)); // Column header title
                dataTable.Columns.Add("Phone Number", typeof(string)); // Column header title
                dataTable.Columns.Add("Start Date", typeof(DateTime)); // Column header title
                dataTable.Columns.Add("End Date", typeof(DateTime)); // Column header title
                dataTable.Columns.Add("On Leave", typeof(string)); // Column header title

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
                        employee.StartDate,
                        employee.EndDate ?? (object)DBNull.Value, // Handle nullable EndDate
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

                // Load leave options
                var leaveOptions = new List<LeaveOption>
        {
            new LeaveOption { Value = "All", Name = "All" },
            new LeaveOption { Value = "Yes", Name = "Yes" },
            new LeaveOption { Value = "No", Name = "No" }
        };

                ComboBox_onleave.DataSource = leaveOptions;
                ComboBox_onleave.DisplayMember = "Name";
                ComboBox_onleave.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading departments or positions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
    public class LeaveOption
    {
        public string Value { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
