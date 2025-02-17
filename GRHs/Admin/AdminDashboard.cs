﻿using GRHs.Data.UserSession;
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
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
            AddUserControl(new Dashboard());
        }

        private void dashboard_btn_Click(object sender, EventArgs e)
        {
            AddUserControl(new Dashboard());
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void AddUserControl(UserControl userControl)
        {
            panel3.Controls.Clear();
            userControl.Dock = DockStyle.Fill; // Optional: Makes the UserControl fill the panel
            panel3.Controls.Add(userControl);
        }

        private void addEmployee_btn_Click(object sender, EventArgs e)
        {
            AddUserControl(new Employees());
        }

        private void salary_btn_Click(object sender, EventArgs e)
        {
            AddUserControl(new Certificates());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUserControl(new Leaves());
        }

        private void dashboard_btn_Click_1(object sender, EventArgs e)
        {
            AddUserControl(new Dashboard());
        }

        private void addEmployee_btn_Click_1(object sender, EventArgs e)
        {
            AddUserControl(new Employees());
        }

        private void salary_btn_Click_1(object sender, EventArgs e)
        {
            AddUserControl(new Certificates());
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            AddUserControl(new Leaves());
        }

        private void guna2GradientButton4_Click(object sender, EventArgs e)
        {
            AddUserControl(new Holidays());
        }

        private void label4_Click(object sender, EventArgs e)
        {
            this.Hide();
            var login = new GRHs.authentication.login();
            login.Show();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
