using GRHs.Data;
using GRHs.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRHs.authentication
{
    public partial class Register : Form
    {
        private UserAccount _userAccount;
        public Register()
        {
            InitializeComponent();
            // Initialize UserAccount with your DbContext
           _userAccount = new UserAccount();
        }

        private void signup_showPass_CheckedChanged(object sender, EventArgs e)
        {
            signup_Password.PasswordChar = signup_showPass.Checked ? '\0' : '*';
        }

        private void signup_loginBtn_Click(object sender, EventArgs e)
        {
           
        }

        private async  void signup_btn_Click(object sender, EventArgs e)
        {
            // Retrieve user input from the form
            string username = signup_username.Text;
            string password = signup_Password.Text;
            string email = signup_email.Text;
            string name = signup_name.Text;


            // Validate user input
            string validationMessage = ValidateUserInput(username, password, email, name);
            if (validationMessage != null)
            {
                // Display validation message and return
                MessageBox.Show(validationMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Call the RegisterAsync method with default values for imagePath and role
            var (message, success) = await _userAccount.RegisterAsync(username, password, email, name);

            if (success)
            {
                // Registration successful
                MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Optionally, proceed with the created user session
                // For example, redirect to a login page or main form
            }
            else
            {
                // Registration failed
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void signup_showPass_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void signup_Password_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void signup_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void signup_email_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void signup_name_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        private string ValidateUserInput(string username, string password, string email, string name)
        {
            // Check if fields are empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                return "All fields are required.";
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                return "Invalid email format.";
            }

            // Validate password
            if (!IsValidPassword(password))
            {
                return "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one digit.";
            }

            // All validations passed
            return null;
        }

        private bool IsValidEmail(string email)
        {
            // Simple regex for validating email format
            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            // Check for minimum length, at least one uppercase letter, one lowercase letter, and one digit
            return password.Length >= 8 &&
                   Regex.IsMatch(password, @"[A-Z]") &&
                   Regex.IsMatch(password, @"[a-z]") &&
                   Regex.IsMatch(password, @"\d");
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
