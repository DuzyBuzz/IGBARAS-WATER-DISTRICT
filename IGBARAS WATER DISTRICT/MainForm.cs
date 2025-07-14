using IGBARAS_WATER_DISTRICT.Helpers;
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

namespace IGBARAS_WATER_DISTRICT
{
    public partial class MainForm : Form

    {

        public MainForm()
        {
            InitializeComponent();
        }
        private void LoadControl(string controlName)
        {
            mainPanel.Controls.Clear();

            var type = Type.GetType($"IGBARAS_WATER_DISTRICT.{controlName}Control");

            if (type != null && type.IsSubclassOf(typeof(UserControl)))
            {
                var controlInstance = (UserControl)Activator.CreateInstance(type);
                controlInstance.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(controlInstance);
            }
            else
            {
                MessageBox.Show($"Control '{controlName}' not found.");
            }
        }
        private void dashboardButton_Click(object sender, EventArgs e)
        {
            LoadControl("Dashboard");
        }

        private void billingButton_Click(object sender, EventArgs e)
        {
            LoadControl("Billing");
        }


        private void settingsButton_Click(object sender, EventArgs e)
        {
            LoadControl("Settings");
        }

        private void systemInformationButton_Click(object sender, EventArgs e)
        {
            LoadControl("SystemInformation");
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadControl("Dashboard");
            usernameLabel.Text = $"{UserCredentials.Username}";
        }

        private void accountsButton_Click(object sender, EventArgs e)
        {
            LoadControl("Accounts");
        }

        private void transactionsButton_Click(object sender, EventArgs e)
        {
            LoadControl("Transactions");
        }
    }
}
