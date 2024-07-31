using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using GRHs.Admin;
using GRHs.authentication;
using GRHs.Entities;
namespace GRHs.User
{
    public partial class MesCertifications : UserControl
    {

        private Users _user;
        private UserAccount _userAccount;
        private Employee _employee;

        public MesCertifications(int userID)
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

        private void MesCertifications_Load(object sender, EventArgs e)
        {
            LoadAttestations();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(leavesView);
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

        private void LoadAttestations()
        {
            try
            {
                if (_employee == null)
                {
                    MessageBox.Show("Employee information is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Fetch attestations from the database
                var attestations = _userAccount.DbContext.Attestations
                    .Where(a => a.EmployeeID == _employee.EmployeeID)
                    .ToList();

                // Create a DataTable to display in the DataGridView
                var dataTable = new DataTable();
                dataTable.Columns.Add("Attestation ID");
                dataTable.Columns.Add("Request Date");
                dataTable.Columns.Add("Status");
                dataTable.Columns.Add("Issue Date");

                foreach (var attestation in attestations)
                {
                    var row = dataTable.NewRow();
                    row["Attestation ID"] = attestation.AttestationID;
                    row["Request Date"] = attestation.RequestDate.ToShortDateString();
                    row["Status"] = attestation.Status;
                    row["Issue Date"] = attestation.IssueDate.HasValue ? attestation.IssueDate.Value.ToShortDateString() : "N/A";
                    dataTable.Rows.Add(row);
                }

                // Set the DataSource of the DataGridView
                leavesView.DataSource = dataTable;

                // Update Guna2HtmlLabel visibility and text based on the presence of attestations
                if (attestations.Count == 0)
                {
                    guna2HtmlLabel1.Visible = true;
                    guna2HtmlLabel1.Text = "No certifications found.";
                }
                else
                {
                    guna2HtmlLabel1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading attestations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
