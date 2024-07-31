using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using GRHs.authentication;
using GRHs.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace GRHs.Admin
{
    public partial class Leaves : UserControl
    {
        private readonly UserAccount _userAccount;

        public Leaves()
        {
            InitializeComponent();
            _userAccount = new UserAccount();
            LoadComboBoxes();
            LoadLeaveTypes();
            loadLeavesType2();
        }

        private void Leaves_Load(object sender, EventArgs e)
        {
            ApplyFilters();
            guna2Panel2.Visible = false;
            guna2Panel1.Visible = true;
            LoadLeaves();
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = rechercher.Text.Trim().ToLower();

                int selectedDepartmentID = ComboBox_Departements.SelectedValue != null
                    ? Convert.ToInt32(ComboBox_Departements.SelectedValue)
                    : -1;

                int selectedPositionID = ComboBox_Positions.SelectedValue != null
                    ? Convert.ToInt32(ComboBox_Positions.SelectedValue)
                    : -1;

                var employees = _userAccount.DbContext.Employees
                    .Include(emp => emp.User)
                    .Include(emp => emp.Position)
                    .Include(emp => emp.Department)
                    .ToList();

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

                var dataTable = new DataTable();
                dataTable.Columns.Add("Employee ID", typeof(int));
                dataTable.Columns.Add("Name", typeof(string));
                dataTable.Columns.Add("Position", typeof(string));
                dataTable.Columns.Add("Department", typeof(string));
                dataTable.Columns.Add("CIN", typeof(string));
                dataTable.Columns.Add("Phone Number", typeof(string));
                dataTable.Columns.Add("Start Date", typeof(DateTime));

                foreach (var employee in filteredEmployees)
                {
                    var employeeName = employee.User?.Name ?? "N/A";
                    var positionName = employee.Position?.PositionName ?? "N/A";
                    var departmentName = employee.Department?.DepartementName ?? "N/A";
                    var cin = employee.CIN ?? "N/A";
                    var phoneNumber = employee.EmployeeNumber ?? "N/A";

                    dataTable.Rows.Add(
                        employee.EmployeeID,
                        employeeName,
                        positionName,
                        departmentName,
                        cin,
                        phoneNumber,
                        employee.StartDate
                    );
                }

                EmployeeView.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while applying filters: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                var departments = _userAccount.DbContext.Departements
                    .Select(d => new { d.DepartementID, d.DepartementName })
                    .ToList();
                departments.Insert(0, new { DepartementID = -1, DepartementName = "All" });

                ComboBox_Departements.DataSource = new BindingSource { DataSource = departments };
                ComboBox_Departements.DisplayMember = "DepartementName";
                ComboBox_Departements.ValueMember = "DepartementID";

                var positions = _userAccount.DbContext.Positions
                    .Select(p => new { p.PositionID, p.PositionName })
                    .ToList();
                positions.Insert(0, new { PositionID = -1, PositionName = "All" });

                ComboBox_Positions.DataSource = new BindingSource { DataSource = positions };
                ComboBox_Positions.DisplayMember = "PositionName";
                ComboBox_Positions.ValueMember = "PositionID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading departments or positions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EmployeeView_SelectionChanged(object sender, EventArgs e)
        {
         
        }


        private void salary_clearBtn_Click(object sender, EventArgs e)
        {
            addEmployee_fullName.Clear();
            addEmployee_cin.Clear();
            StartTime.Value = DateTime.Now;
            
            DaysNumber.Clear();
            positiontxt.Clear();
        }
        private void ClearLeaveFields()
        {
            // Reset all relevant input fields
            StartTime.Value = DateTime.Now;
            //dTime.Value = DateTime.Now; // Ensure you have an EndTime picker
            typeleave.Text = "";
            DaysNumber.Text = "";
            positiontxt.Text = ""; 
            addEmployee_fullName.Text = "";
            addEmployee_cin.Text = "";
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

        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {

            try
            {
                // Ensure a row is selected
                if (EmployeeView.SelectedRows.Count > 0)
                {
                    // Retrieve the selected row
                    DataGridViewRow selectedRow = EmployeeView.SelectedRows[0];

                    // Extract the employee ID from the selected row
                    int employeeID = Convert.ToInt32(selectedRow.Cells["Employee ID"].Value);

                    // Retrieve and parse other input values
                    if (DateTime.TryParse(StartTime.Text, out DateTime startDate) &&
                        int.TryParse(DaysNumber.Text, out int numberOfDays) &&
                        Enum.TryParse(typeleave.SelectedValue?.ToString(), out LeaveType leaveType) &&
                        !string.IsNullOrEmpty(typeleave.Text))
                    {
                        // Calculate end date
                        DateTime initialEndDate = startDate.AddDays(numberOfDays);

                        // Check if the employee exists
                        var employee = _userAccount.DbContext.Employees
                            .SingleOrDefault(emp => emp.EmployeeID == employeeID);

                        if (employee != null)
                        {
                            // Fetch existing leaves for the employee within the new leave period
                            var existingLeaves = _userAccount.DbContext.Leaves
                                .Where(l => l.EmployeeID == employeeID &&
                                            ((l.StartDate <= initialEndDate && l.EndDate >= startDate)))
                                .ToList();

                            if (existingLeaves.Any())
                            {
                                // Overlapping leave detected
                                MessageBox.Show("The employee already has a leave scheduled during this period.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                // Fetch holidays within the leave period
                                var holidays = _userAccount.DbContext.Holidays
                                    .Where(h => h.Date >= startDate && h.Date <= initialEndDate)
                                    .ToList();

                                // Calculate the total number of holiday days within the leave period
                                int holidayDays = holidays.Sum(h => h.Daysnumber);

                                // Adjust the end date by adding the total number of holiday days
                                DateTime adjustedEndDate = initialEndDate.AddDays(holidayDays);

                                // Create a new Leave record
                                var newLeave = new Leave
                                {
                                    EmployeeID = employeeID,
                                    StartDate = startDate,
                                    EndDate = adjustedEndDate, // Adjusted end date
                                    Type = leaveType,
                                    Reason = typeleave.Text, // Assuming there's a textbox for reason
                                    NumberOfDays = numberOfDays, // Keep the original number of days
                                    Status = LeaveStatus.Pending // Default status
                                };

                                // Add the new leave record to the database
                                _userAccount.DbContext.Leaves.Add(newLeave);
                                _userAccount.DbContext.SaveChanges();

                                MessageBox.Show("Leave added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Optionally, clear the fields or refresh the view
                                salary_clearBtn_Click(sender, e); // Call the method to clear fields
                            }
                        }
                        else
                        {
                            MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please ensure all input values are valid.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please select an employee from the DataGridView.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding leave: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2GroupBox2_Click(object sender, EventArgs e)
        {
            // Handle group box click if needed
        }

        private void EmployeeView_SelectionChanged_1(object sender, EventArgs e)
        {
            if (EmployeeView.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = EmployeeView.SelectedRows[0];

                // Extract the data from the selected row
                int employeeID = (int)selectedRow.Cells["Employee ID"].Value;
                string name = (string)selectedRow.Cells["Name"].Value;
                string position = (string)selectedRow.Cells["Position"].Value;
                string department = (string)selectedRow.Cells["Department"].Value;
                string cin = (string)selectedRow.Cells["CIN"].Value;
                string phoneNumber = (string)selectedRow.Cells["Phone Number"].Value;
                string date = (string)selectedRow.Cells["Start Date"].Value.ToString();
                //StartTime.Value = DateTime.Parse(date);
                // Populate the text boxes and combo boxes
                addEmployee_fullName.Text = name;
                addEmployee_cin.Text = cin;

                positiontxt.Text = position;
            }
        }

     
        private void LoadLeaveTypes()
        {
            try
            {
                // Get the list of LeaveType values
                var leaveTypes = Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>().ToList();

                // Create a list to bind to the ComboBox
                var leaveTypeList = leaveTypes.Select(type => new { Value = type, Name = type.ToString() }).ToList();

                // Bind the list to the ComboBox
                typeleave.DataSource = leaveTypeList;
                typeleave.DisplayMember = "Name";
                typeleave.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading leave types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loadLeavesType2()
        {
            try
            {
                // Get the list of LeaveType values
                var leaveTypes = Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>().ToList();

                // Create a list to bind to the ComboBox
                var leaveTypeList = leaveTypes.Select(type => new { Value = type, Name = type.ToString() }).ToList();

                // Bind the list to the ComboBox
                typeleaveup.DataSource = leaveTypeList;
                typeleaveup.DisplayMember = "Name";
                typeleaveup.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading leave types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            guna2Panel2.Visible= false;
            guna2Panel1.Visible = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            guna2Panel1.Visible = false;
            guna2Panel2.Visible = true;
            LoadLeaves();
        }

        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a row is selected
                if (leavsView.SelectedRows.Count > 0)
                {
                    // Retrieve the selected row
                    DataGridViewRow selectedRow = leavsView.SelectedRows[0];

                    // Extract the leave ID from the selected row
                    int leaveID = Convert.ToInt32(selectedRow.Cells["Leave ID"].Value);

                    // Retrieve and parse other input values
                    if (DateTime.TryParse(datestart.Text, out DateTime newStartDate) &&
                        int.TryParse(DaysNumberupdate.Text, out int newNumberOfDays) &&
                        Enum.TryParse(typeleaveup.SelectedValue?.ToString(), out LeaveType newLeaveType))
                    {
                        // Update the leave
                        UpdateLeave(leaveID, newStartDate, newLeaveType, newNumberOfDays);

                        // Optionally, refresh the view
                        LoadLeaves();
                    }
                    else
                    {
                        MessageBox.Show("Invalid input. Please check your entries.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a leave from the DataGridView.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the leave: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadLeaves()
        {
            try
            {
                var leaves = _userAccount.DbContext.Leaves
                    .Include(l => l.Employee)
                    .ThenInclude(e => e.User)
                    .Include(l => l.Employee.Position)
                    .Include(l => l.Employee.Department)
                    .ToList();

                var dataTable = new DataTable();
                dataTable.Columns.Add("Leave ID", typeof(int));
                dataTable.Columns.Add("Employee Name", typeof(string));
                dataTable.Columns.Add("Employee Phone", typeof(string));
                dataTable.Columns.Add("Employee CIN", typeof(string));
                dataTable.Columns.Add("Start Date", typeof(DateTime));
                dataTable.Columns.Add("End Date", typeof(DateTime));
                dataTable.Columns.Add("Number of Days", typeof(int));
                dataTable.Columns.Add("Leave Type", typeof(string));

                foreach (var leave in leaves)
                {
                    var employee = leave.Employee;
                    var user = employee.User;
                    var position = employee.Position;
                    var department = employee.Department;

                    dataTable.Rows.Add(
                        leave.LeaveID,
                        user?.Name ?? "N/A",
                        employee?.EmployeeNumber ?? "N/A",
                        employee?.CIN ?? "N/A",
                        leave.StartDate,
                        leave.EndDate,
                        leave.NumberOfDays,
                        leave.Type.ToString()
                    );
                }

                leavsView.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading leaves: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void leavsView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void leavsView_SelectionChanged(object sender, EventArgs e)
        {
            if (leavsView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = leavsView.SelectedRows[0];

                int leaveID = (int)selectedRow.Cells["Leave ID"].Value;
                string employeeName = (string)selectedRow.Cells["Employee Name"].Value;
                string employeePhone = (string)selectedRow.Cells["Employee Phone"].Value;
                string employeeCIN = (string)selectedRow.Cells["Employee CIN"].Value;
                DateTime startDate = (DateTime)selectedRow.Cells["Start Date"].Value;
                DateTime endDate = (DateTime)selectedRow.Cells["End Date"].Value;
                int numberOfDays = (int)selectedRow.Cells["Number of Days"].Value;
                string leaveType = (string)selectedRow.Cells["Leave Type"].Value;

                // Assuming you have corresponding controls to display this information
               
                name.Text = employeeName;
                cin.Text = employeeCIN;
                datestart.Value = startDate;
                DaysNumberupdate.Text = numberOfDays.ToString();
                // Set the SelectedValue to the parsed LeaveType
                typeleaveup.SelectedValue = Enum.Parse(typeof(LeaveType), leaveType);
                positionUpdate.Text= employeePhone;
            }
        }

        private void UpdateLeave(int leaveID, DateTime newStartDate, LeaveType newLeaveType, int newNumberOfDays)
        {
            try
            {
                // Find the existing leave record
                var leave = _userAccount.DbContext.Leaves.SingleOrDefault(l => l.LeaveID == leaveID);

                if (leave != null)
                {
                    // Calculate initial end date
                    DateTime initialEndDate = newStartDate.AddDays(newNumberOfDays);

                    // Fetch holidays within the leave period
                    var holidays = _userAccount.DbContext.Holidays
                        .Where(h => h.Date >= newStartDate && h.Date <= initialEndDate)
                        .ToList();

                    // Calculate the total number of holiday days within the leave period
                    int holidayDays = holidays.Sum(h => h.Daysnumber);

                    // Adjust the end date by adding the total number of holiday days
                    DateTime adjustedEndDate = initialEndDate.AddDays(holidayDays);

                    // Update leave details
                    leave.StartDate = newStartDate;
                    leave.EndDate = adjustedEndDate; // Adjusted end date
                    leave.Type = newLeaveType;
                    leave.NumberOfDays = newNumberOfDays;

                    // Save changes to the database
                    _userAccount.DbContext.SaveChanges();

                    MessageBox.Show("Leave updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Leave not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the leave: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ExportDataGridViewToExcel(DataGridView dataGridView)
        {
            try
            {
                // Create a new DataTable and fill it with the DataGridView data
                var dataTable = new DataTable();

                // Add columns to DataTable
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    dataTable.Columns.Add(column.HeaderText);
                }

                // Add rows to DataTable
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            dataRow[cell.ColumnIndex] = cell.Value ?? ""; // Convert object to string and handle null values
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }

                if (dataTable.Rows.Count > 0) // Check if there are rows to export
                {
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
                                MessageBox.Show("Data exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No data to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteLeave(int leaveID)
        {
            try
            {
                // Find the leave record in the database
                var leave = _userAccount.DbContext.Leaves.SingleOrDefault(l => l.LeaveID == leaveID);

                if (leave != null)
                {
                    // Remove the leave record from the DbSet
                    _userAccount.DbContext.Leaves.Remove(leave);

                    // Save changes to the database
                    _userAccount.DbContext.SaveChanges();

                    // Inform the user of success
                    MessageBox.Show("Leave deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the view to reflect changes
                    LoadLeaves();
                }
                else
                {
                    MessageBox.Show("Leave not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the leave: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputFields()
        {
            try
            {
                // Clear textboxes
                cin.Text = string.Empty;
                name.Text = string.Empty;
                positionUpdate.Text = string.Empty;
                DaysNumberupdate.Text = string.Empty;

                // Reset ComboBox selection
                typeleaveup.SelectedIndex = -1;

                // Set the date picker to the current date or a default value
                datestart.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while clearing input fields: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(leavsView);
        }

        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a row is selected
                if (leavsView.SelectedRows.Count > 0)
                {
                    // Retrieve the selected row
                    DataGridViewRow selectedRow = leavsView.SelectedRows[0];

                    // Extract the leave ID from the selected row
                    int leaveID = Convert.ToInt32(selectedRow.Cells["Leave ID"].Value);

                    // Delete the leave record
                    DeleteLeave(leaveID);
                }
                else
                {
                    MessageBox.Show("Please select a leave from the DataGridView.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the leave: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            // Clear input fields
            ClearInputFields();
        }

        private void salary_clearBtn_Click_1(object sender, EventArgs e)
        {
            StartTime.Value = DateTime.Now;
            // Populate the text boxes and combo boxes
            addEmployee_fullName.Text = "";
            addEmployee_cin.Text = "";
            DaysNumber.Text = "";
            positiontxt.Text = "";
            typeleave.SelectedIndex = -1;
        }
    }
}
