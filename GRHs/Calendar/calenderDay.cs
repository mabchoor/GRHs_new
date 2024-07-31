using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRHs.Calendar
{
  
    public partial class calenderDay : UserControl
    {
        string _day ,tade, weekday;

        private void calenderDay_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public calenderDay(string day)
        {
            InitializeComponent();
                        _day = day;
            label1.Text = day;
        }

        private void calenderDay_Load(object sender, EventArgs e)
        {

        }
        
    }
}
