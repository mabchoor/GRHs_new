using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRHs.Calendar
{
    public partial class CalendarView : UserControl
    {
        public static int _month , _year;
        private List<Tuple<DateTime, DateTime>> _leaveData; // Store the leave data

        // Constructor that accepts leave data
        public CalendarView(List<Tuple<DateTime, DateTime>> leaveData)
        {
            InitializeComponent();
            _month = DateTime.Now.Month;
            _year = DateTime.Now.Year;
            _leaveData = leaveData; // Initialize the leave data
        }

        private void Calendar_Load(object sender, EventArgs e)
        {
            ShowDays(_month, _year);
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            if (_month == 12)
            {
                _month = 1;
                if (_year < 9999) // Ensure year does not exceed 9999
                {
                    _year++;
                }
            }
            else
            {
                _month++;
            }

            ShowDays(_month, _year);
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            if (_month == 1)
            {
                _month = 12;
                if (_year > 1) // Ensure year does not go below 1
                {
                    _year--;
                }
            }
            else
            {
                _month--;
            }

            ShowDays(_month, _year);
        }

        private void ShowDays(int month, int year)
        {
            // Validate month and year
            if (month < 1 || month > 12)
            {
                MessageBox.Show("Invalid month. Month must be between 1 and 12.");
                return;
            }

            if (year < 1 || year > 9999)
            {
                MessageBox.Show("Invalid year. Year must be between 1 and 9999.");
                return;
            }

            // Update the month and year label
            lbMonth.Text = new DateTime(year, month, 1).ToString("MMMM yyyy").ToUpper();

            // Clear previous controls
            panel1.Controls.Clear();

            // Define the number of columns for the TableLayoutPanel
            int columns = 7; // Days of the week
            int rows = 6; // Maximum possible rows in a month

            // Create and configure the TableLayoutPanel
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
            {
                ColumnCount = columns,
                RowCount = rows,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            // Add column headers (optional, for the days of the week)
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < columns; i++)
            {
                tableLayoutPanel.Controls.Add(new Label { Text = dayNames[i], TextAlign = ContentAlignment.MiddleCenter }, i, 0);
            }

            // Calculate start day and number of days in the month
            DateTime startOfTheMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDay = (int)startOfTheMonth.DayOfWeek; // Sunday = 0, Monday = 1, etc.

            // Get today's date
            DateTime today = DateTime.Today;

            // Add empty cells for days before the start of the month
            for (int i = 0; i < startDay; i++)
            {
                tableLayoutPanel.Controls.Add(new Label(), i, 1);
            }

            // Add actual days of the month
            int row = 1;
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDay = new DateTime(year, month, day);
                calenderDay dayControl = new calenderDay(day.ToString());

                // Default color for the day
                dayControl.BackColor = Color.White;

                // Highlight the current date
                if (day == today.Day && month == today.Month && year == today.Year)
                {
                    dayControl.BackColor = Color.LightBlue; // Highlight today's date
                    dayControl.BorderStyle = BorderStyle.FixedSingle; // Border style for today's date
                }

                // Initialize the color priority variables
                bool isCurrentLeave = false;
                bool isPastLeave = false;
                bool isFutureLeave = false;

                // Iterate through all leaves and apply color coding
                foreach (var leave in _leaveData)
                {
                    DateTime leaveStart = leave.Item1; // Access Tuple item by index
                    DateTime leaveEnd = leave.Item2; // Access Tuple item by index

                    if (currentDay >= leaveStart && currentDay <= leaveEnd) // Current leave
                    {
                        isCurrentLeave = true;
                        if (currentDay < today) // Past days within current leave
                        {
                            dayControl.BackColor = Color.LightCoral;
                        }
                        else if (currentDay == today) // Today within current leave
                        {
                            dayControl.BackColor = Color.LightPink; // Today's leave highlight
                        }
                        else // Future days within current leave
                        {
                            dayControl.BackColor = Color.LightGreen;
                        }
                        break; // Stop checking other leaves as current leave takes precedence
                    }
                    else if (currentDay > leaveEnd) // Past leave
                    {
                        isPastLeave = true;
                    }
                    else if (currentDay < leaveStart) // Future leave
                    {
                        isFutureLeave = true;
                    }
                }

                // Apply color for past or future leaves if not a current leave
                if (!isCurrentLeave)
                {
                    if (isPastLeave)
                    {
                        dayControl.BackColor = Color.LightGray;
                    }
                    else if (isFutureLeave)
                    {
                        dayControl.BackColor = Color.LightYellow;
                    }
                }

                tableLayoutPanel.Controls.Add(dayControl, (startDay + day - 1) % columns, row);
                if ((startDay + day - 1) % columns == columns - 1)
                {
                    row++;
                }
            }

            // Add TableLayoutPanel to panel
            panel1.Controls.Add(tableLayoutPanel);
        }



    }
}
