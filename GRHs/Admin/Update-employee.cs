using GRHs.authentication;
using GRHs.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GRHs.Admin
{
    public partial class Update_employee : Form
    {
        private UserAccount _userAccount;
        private int _employeeId;
        public Update_employee(int employeeId)
        {

            InitializeComponent();
            _userAccount = new UserAccount();
            _employeeId = employeeId;
        }

        private void Update_employee_Load(object sender, EventArgs e)
        {
            try
            {
                var departments = _userAccount.DbContext.Departements
            .Select(d => new Departement
            {
                DepartementID = d.DepartementID,
                DepartementName = d.DepartementName
            })
            .ToList();

                // Create and configure a BindingSource
                var bindingSource = new BindingSource();
                bindingSource.DataSource = departments;

                // Bind the BindingSource to the ComboBox
                addEmployee_Departement.DataSource = bindingSource;
                addEmployee_Departement.DisplayMember = "DepartementName";
                addEmployee_Departement.ValueMember = "DepartementID";
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during data fetching
                MessageBox.Show($"An error occurred while loading departments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // Fetch departments from the database
                var position = _userAccount.DbContext.Positions
                    .Select(d => new { d.PositionID, d.PositionName })
                    .ToList();

                // Bind the departments to the ComboBox
                addEmployee_position.DataSource = position;
                addEmployee_position.DisplayMember = "PositionName";
                addEmployee_position.ValueMember = "PositionID";
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during data fetching
                MessageBox.Show($"An error occurred while loading departments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                var employee = _userAccount.DbContext.Employees
                .Include(emp => emp.User)
                .Include(emp => emp.Position)
                .Include(emp => emp.Department)
                .FirstOrDefault(emp => emp.EmployeeID == _employeeId);

                if (employee != null)
                {
                    // Populate fields with existing data
                    username.Text = employee.User?.Username ?? string.Empty;
                    email.Text = employee.User?.Email ?? string.Empty;
                    empNumber.Text = employee.EmployeeNumber;
                    addEmployee_id.Text = employee.CIN;
                    StartTime.Value = employee.StartDate;
                    EndDate.Enabled = false; // Handle nullable EndDate
                    addEmployee_fullName.Text = employee.User?.Name ?? string.Empty;
                    

                    // Set the selected values for ComboBoxes
                    addEmployee_position.SelectedValue = employee.PositionID;
                    addEmployee_Departement.SelectedValue = employee.DepartmentID;
                    //passwd.Text = employee.User?.Password ?? string.Empty;
                    //cpasswd.Text = employee.User?.Password ?? string.Empty;
                    addEmployee_picture.ImageLocation = employee.User?.Image ?? string.Empty;
                }
                else
                {
                    MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading employee data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateEmployee()
        {
            try
            {
                // Fetch the employee record to update
                var employee = _userAccount.DbContext.Employees
                    .FirstOrDefault(e => e.EmployeeID == _employeeId);

                if (employee != null)
                {
                    // Update employee properties
                    employee.EmployeeNumber = empNumber.Text;
                    employee.CIN = addEmployee_id.Text;
                    employee.StartDate = StartTime.Value;
                    employee.EndDate = EndDate.Value; // Use null if not set


                    // Update related User
                    var user = _userAccount.DbContext.Users
                        .FirstOrDefault(u => u.UserID == employee.UserID);

                    if (user != null)
                    {
                        user.Username = username.Text;
                        user.Email = email.Text;
                        // Hash and update password if needed
                       user.Password = HashPassword(passwd.Text); // Handle password change if needed
                    }

                    // Update related Position and Department
                    employee.PositionID = (int)addEmployee_position.SelectedValue;
                    employee.DepartmentID = (int)addEmployee_Departement.SelectedValue;

                    // Save changes to the database
                    _userAccount.DbContext.SaveChanges();



                    // Prepare email content
                    string subject = "Your Account Credentials";
                    string body = $@"
            <p>Dear {addEmployee_fullName.Text},</p>
            <p>Your account has been successfully updated. Here are your new login details:</p>
            <p><strong>Username:</strong> {username.Text}</p>
            <p><strong>Password:</strong> {passwd.Text}</p>
            <p>Please keep these details safe and secure. If you have any questions or need assistance, do not hesitate to contact us.</p>
            <br />
            <p>Best regards,</p>
            <p><strong>GRHs</strong></p>
            <p><em>This is an automated message from GRHs. Please do not reply to this email.</em></p>
            <p><a href='mailto:no-reply@grhs.com'>no-reply@grhs.com</a></p>";



                    MessageBox.Show("Employee updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Send email
                    EmailService.SendEmail(email.Text, subject, body);




                }
                else
                {
                    MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to hash a password
        private string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            // Combine salt and hashed password
            return Convert.ToBase64String(salt) + ":" + hashed;
        }

        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            UpdateEmployee();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void login_showPass_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void login_showPass_CheckedChanged_1(object sender, EventArgs e)
        {
            passwd.PasswordChar = login_showPass.Checked ? '\0' : '*';
            cpasswd.PasswordChar = login_showPass.Checked ? '\0' : '*';
        }
    }
}
