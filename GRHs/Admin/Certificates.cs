using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
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
using System.Web.Configuration;
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
            ApplyFilters();
        }
        private void ApplyFilters()
        {
            try
            {
                // Get the search text
                string searchText = rechercher.Text.Trim().ToLower();

                // Get selected values from ComboBoxes safely
                int selectedDepartmentID = -1;
                int selectedPositionID = -1;

                if (ComboBox_Departements.SelectedValue != null)
                {
                    int.TryParse(ComboBox_Departements.SelectedValue.ToString(), out selectedDepartmentID);
                }

                if (ComboBox_Positions.SelectedValue != null)
                {
                    int.TryParse(ComboBox_Positions.SelectedValue.ToString(), out selectedPositionID);
                }

                // Fetch all employees including related entities
                var employees = _userAccount.DbContext.Employees
                    .Include(emp => emp.User)        // Eager load User details
                    .Include(emp => emp.Position)    // Eager load Position details
                    .Include(emp => emp.Department)  // Eager load Department details
                    .ToList();

                // Filter employees based on search text, department, and position
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
                dataTable.Columns.Add("Start Date", typeof(DateTime)); // Column header title

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
                        employee.StartDate
                    );
                }

                // Bind the DataTable to the DataGridView
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
                // Load departments
                var departments = _userAccount.DbContext.Departements
                    .Select(d => new Departement
                    {
                        DepartementID = d.DepartementID,
                        DepartementName = d.DepartementName
                    })
                    .ToList();

                // Add "All" option
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

                // Add "All" option
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

        private void EmployeeView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void EmployeeView_SelectionChanged(object sender, EventArgs e)
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
                StartTime.Value = DateTime.Parse(date);
                // Populate the text boxes and combo boxes
                addEmployee_fullName.Text = name;
                addEmployee_id.Text = cin;
                phone.Text = phoneNumber;

                // Set the combo box selections
                departementtxt.Text = department;

                positiontxt.Text = position;
            }
        }

        private void button1_Click(object sender, EventArgs e)
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

                // Populate the text boxes and combo boxes
                addEmployee_fullName.Text = name;
                addEmployee_id.Text = cin;
                phone.Text = phoneNumber;

                // Set the combo box selections
                departementtxt.Text = department;

                positiontxt.Text = position;
            }
        }

        private void salary_clearBtn_Click(object sender, EventArgs e)
        {

            addEmployee_fullName.Text = "";
            addEmployee_id.Text = "";
            phone.Text = "";

            StartTime.Value = DateTime.Now;
            departementtxt.Text = "";

            positiontxt.Text = "";
        }

        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            int employeeID = GetSelectedEmployeeID(); // Method to get the selected employee ID
            if (employeeID == -1)
            {
                MessageBox.Show("Please select an employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var employeeDetails = _userAccount.DbContext.Employees
                .Where(emp => emp.EmployeeID == employeeID)
                .Select(emp => new
                {
                    EmployeeName = emp.User.Name,
                    EmployeeEmail = emp.User.Email
                })
                .FirstOrDefault();

            if (employeeDetails == null)
            {
                MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isGenerated = GenerateAttestation();
            if (isGenerated)
            {
                InsertAttestation(employeeID);

                // Prepare email content
                string subject = "Certification Generated - Immediate Action Required";
                string body = $@"
                    <p>Dear {employeeDetails.EmployeeName},</p>
                    <p>We are pleased to inform you that your certification has been successfully generated.</p>
                    <p>Please visit the Human Resources department as soon as possible to collect your certification and for any additional information you may need.</p>
                    <p>If you have any questions or need further assistance, feel free to contact HR.</p>
                    <br />
                    ";

                // Send email
                EmailService.SendEmail(employeeDetails.EmployeeEmail, subject, body);
            }

        }

        private bool GenerateAttestation()
        {
            try
            {
                string[] wordsToReplace = { "<name>", "<cin>", "<posistion>", "<starttime>", "<date>" };
                string[] newValues =
                {
            addEmployee_fullName.Text ?? string.Empty,
            addEmployee_id.Text ?? string.Empty,
            positiontxt.Text ?? string.Empty,
            StartTime.Text ?? string.Empty,
            DateTime.Now.ToString("yyyy-MM-dd") // Format DateTime to a string
        };

                var generator = new GenerateCertification(wordsToReplace, newValues, addEmployee_id.Text);
                generator.ReplaceWordsInWordFile();
                return true; // Return true if generation is successful
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating attestation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void InsertAttestation(int employeeID)
        {
            try
            {
                var employee = _userAccount.DbContext.Employees
                    .Include(e => e.User)
                    .Include(e => e.Position)
                    .Include(e => e.Department)
                    .FirstOrDefault(e => e.EmployeeID == employeeID);

                if (employee == null)
                {
                    MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var attestation = new Attestation
                {
                    EmployeeID = employee.EmployeeID,
                    RequestDate = DateTime.Now,
                    Status = "Validated", // Assuming attestation is validated
                    IssueDate = DateTime.Now
                };

                // Insert the attestation into the database
                _userAccount.DbContext.Attestations.Add(attestation);
                _userAccount.DbContext.SaveChanges();

                MessageBox.Show("Attestation generated and inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while inserting the attestation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetSelectedEmployeeID()
        {
            if (EmployeeView.SelectedRows.Count > 0)
            {
                return (int)EmployeeView.SelectedRows[0].Cells["Employee ID"].Value;
            }
            return -1;
        }
    }


}
