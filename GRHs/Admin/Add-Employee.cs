using GRHs.authentication;
using GRHs.Data.UserSession;
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
    public partial class Add_Employee : Form
    {
        private UserAccount _userAccount;
        public Add_Employee()
        {
            InitializeComponent();
            // Initialize UserAccount with your DbContext
            _userAccount = new UserAccount();
        }
        string imagePath = "";
        private void addEmployee_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    addEmployee_picture.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Collect User Information
                string userName = username.Text; // Replace with your actual input field
                string userEmail = email.Text; // Replace with your actual input field
                string userPassword = passwd.Text; // Replace with your actual input field
                string confirmPassword = cpasswd.Text; // Replace with your actual input field for confirmation password
                int userRole = 0; // Replace with your actual input field (assume role is an integer)

                // Validate User Information
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Please fill in all user details.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if passwords match
                if (userPassword != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hash the password (if needed)
                string hashedPassword = HashPassword(userPassword); // Implement HashPassword as per your hashing logic

                // Create a new User object
                var newUser = new Users
                {
                    Username = userName,
                    Email = userEmail,
                    Password = hashedPassword,
                    Image = imagePath, // Ensure imagePath is defined if needed
                    Role = userRole // Assume role is an integer
                };

                // Add User to the Database
                _userAccount.DbContext.Users.Add(newUser);
                _userAccount.DbContext.SaveChanges();

                // Collect Employee Information
                int userID = newUser.UserID; // Get the newly created UserID

                // Ensure SelectedValue is not null and cast it safely
                int employeePositionID = Convert.ToInt32(addEmployee_position.SelectedValue); // Convert safely to int
                int employeeDepartmentID = Convert.ToInt32(addEmployee_Departement.SelectedValue); // Convert safely to int
                DateTime employeeStartDate = StartTime.Value; // Replace with your actual input field
                DateTime? employeeEndDate = null;
                string CIN = addEmployee_id.Text; // Replace with your actual input field
                string employeeNumber = empNumber.Text; // Replace with your actual input field

                // Create a new Employee object
                var newEmployee = new Employee
                {
                    UserID = userID,
                    PositionID = employeePositionID, // Assume PositionID is an integer
                    DepartmentID = employeeDepartmentID, // Assume DepartmentID is an integer
                    CIN = CIN,
                    EmployeeNumber = employeeNumber,
                    StartDate = employeeStartDate,
                    EndDate = employeeEndDate
                };

                // Add Employee to the Database
                _userAccount.DbContext.Employees.Add(newEmployee);
                _userAccount.DbContext.SaveChanges();

                // Optionally, inform the user
                MessageBox.Show("Employee added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Hide();
        }

        // Sample hashing function (you should replace this with your actual implementation)
        private string HashPassword(string password)
        {
            // Implement your password hashing logic here
            return password; // Placeholder
        }

        private void addEmployee_Departement_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Add_Employee_Load(object sender, EventArgs e)
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
        }
    }

}