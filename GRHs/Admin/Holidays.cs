using ClosedXML.Excel;
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
    public partial class Holidays : UserControl
    {
        private readonly UserAccount _userAccount;
        public Holidays()
        {

            InitializeComponent();
            _userAccount = new UserAccount();
        }

        private void Holidays_Load(object sender, EventArgs e)
        {
            LoadHolidays();
        }

        private void LoadHolidays()
        {
            try
            {
                var query = _userAccount.DbContext.Holidays.AsQueryable();

                // Check if rechercher.Text is not empty
                if (!string.IsNullOrEmpty(rechercher.Text))
                {
                    // Filter holidays by name
                    query = query.Where(h => h.Name.Contains(rechercher.Text));
                }

                // Fetch the holidays from the database
                var holidays = query.ToList();

                // Create a DataTable to hold the data
                var dataTable = new DataTable();
                dataTable.Columns.Add("Holiday ID");
                dataTable.Columns.Add("Name");
                dataTable.Columns.Add("Days Number");
                dataTable.Columns.Add("Date");

                // Populate the DataTable with data
                foreach (var holiday in holidays)
                {
                    var row = dataTable.NewRow();
                    row["Holiday ID"] = holiday.HolidayID;
                    row["Name"] = holiday.Name;
                    row["Days Number"] = holiday.Daysnumber;
                    row["Date"] = holiday.Date.ToShortDateString();
                    dataTable.Rows.Add(row);
                }

                // Bind the DataTable to the DataGridView
                HolidaysView.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading holidays: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddHoliday(string name, int daysNumber, DateTime date)
        {
            try
            {
                var newHoliday = new Holiday
                {
                    Name = name,
                    Daysnumber = daysNumber,
                    Date = date
                };

                // Ensure the DbContext is not null
                if (_userAccount?.DbContext == null)
                {
                    throw new Exception("Database context is not initialized.");
                }

                // Ensure the DbSet<Holidays> is not null
                if (_userAccount.DbContext.Holidays == null)
                {
                    throw new Exception("Holidays DbSet is not initialized.");
                }

                _userAccount.DbContext.Holidays.Add(newHoliday);
                _userAccount.DbContext.SaveChanges();

                LoadHolidays();
                MessageBox.Show("Holiday added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the holiday: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void UpdateSelectedHoliday()
        {
            try
            {
                // Ensure a row is selected
                if (HolidaysView.SelectedRows.Count > 0)
                {
                    // Retrieve the selected row
                    DataGridViewRow selectedRow = HolidaysView.SelectedRows[0];

                    // Extract the holiday ID from the selected row
                    int holidayID = Convert.ToInt32(selectedRow.Cells["Holiday ID"].Value);

                    // Extract other values from the input fields
                    string name = nameup.Text;
                    int daysNumber;
                    if (!int.TryParse(daysNumberup.Text, out daysNumber))
                    {
                        MessageBox.Show("Please enter a valid number for days.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    DateTime date = datestart.Value;

                    // Fetch the holiday from the database
                    var holiday = _userAccount.DbContext.Holidays.FirstOrDefault(h => h.HolidayID == holidayID);

                    if (holiday == null)
                    {
                        MessageBox.Show("Holiday not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Update holiday details
                    holiday.Name = name;
                    holiday.Daysnumber = daysNumber;
                    holiday.Date = date;

                    // Save changes to the database
                    _userAccount.DbContext.SaveChanges();

                    // Reload holidays to reflect the update
                    LoadHolidays();
                    MessageBox.Show("Holiday updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select a holiday to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the holiday: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteHoliday()
        {
            try
            {
                // Ensure a row is selected
                if (HolidaysView.SelectedRows.Count > 0)
                {
                    // Retrieve the selected row
                    DataGridViewRow selectedRow = HolidaysView.SelectedRows[0];

                    // Extract the holiday ID from the selected row
                    int holidayID = Convert.ToInt32(selectedRow.Cells["Holiday ID"].Value);

                    // Fetch the holiday from the database
                    var holiday = _userAccount.DbContext.Holidays.FirstOrDefault(h => h.HolidayID == holidayID);

                    if (holiday == null)
                    {
                        MessageBox.Show("Holiday not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Delete the holiday
                    _userAccount.DbContext.Holidays.Remove(holiday);
                    _userAccount.DbContext.SaveChanges();

                    // Reload holidays to reflect the deletion
                    LoadHolidays();
                    MessageBox.Show("Holiday deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select a holiday to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the holiday: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearHolidayInputs()
        {
            
            nameup.Text = string.Empty;
            daysNumberup.Text = string.Empty;
            datestart.Value = DateTime.Today;
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

        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            UpdateSelectedHoliday();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Add_name.Text) &&
      int.TryParse(Add_days.Text, out int daysNumber) &&
      Add_date.Value != null)
            {
                DateTime date = Add_date.Value;

                AddHoliday(Add_name.Text, daysNumber, date);
            }
            else
            {
                MessageBox.Show("Please enter valid details in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HolidaysView_SizeChanged(object sender, EventArgs e)
        {

        }

        private void HolidaysView_SelectionChanged(object sender, EventArgs e)
        {
            if (HolidaysView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = HolidaysView.SelectedRows[0];
                nameup.Text = selectedRow.Cells["Name"].Value.ToString();
                daysNumberup.Text = selectedRow.Cells["Days Number"].Value.ToString();
                datestart.Value = DateTime.Parse(selectedRow.Cells["Date"].Value.ToString());
            }
        }

        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            DeleteHoliday();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(HolidaysView);
        }

        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            ClearHolidayInputs();
        }
    }
}
