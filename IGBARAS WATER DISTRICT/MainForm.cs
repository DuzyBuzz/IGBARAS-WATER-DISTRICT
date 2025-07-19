using IGBARAS_WATER_DISTRICT.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class MainForm : Form
    {
        // Dictionary to store the loaded user controls
        private Dictionary<string, UserControl> loadedControls = new Dictionary<string, UserControl>();

        // Keep track of currently displayed control
        private string currentControlName = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadControl(string controlName)
        {
            // 🔁 Avoid reloading the same control
            if (currentControlName == controlName)
                return;

            currentControlName = controlName;

            // 🔍 Hide all currently loaded controls
            foreach (var ctrl in loadedControls.Values)
            {
                ctrl.Visible = false;
            }

            // ✅ If control already loaded, just show it
            if (loadedControls.ContainsKey(controlName))
            {
                loadedControls[controlName].Visible = true;
                return;
            }

            // 🔨 Dynamically create the control
            var type = Type.GetType($"IGBARAS_WATER_DISTRICT.{controlName}Control");

            if (type != null && type.IsSubclassOf(typeof(UserControl)))
            {
                var controlInstance = (UserControl)Activator.CreateInstance(type);
                controlInstance.Dock = DockStyle.Fill;

                // Add to cache and main panel
                loadedControls[controlName] = controlInstance;
                mainPanel.Controls.Add(controlInstance);
                controlInstance.BringToFront();
            }
            else
            {
                MessageBox.Show($"Control '{controlName}' not found.");
            }
        }

        // Button click handlers
        private void dashboardButton_Click(object sender, EventArgs e) => LoadControl("Dashboard");
        private void billingButton_Click(object sender, EventArgs e) => LoadControl("Billing");
        private void settingsButton_Click(object sender, EventArgs e) => LoadControl("Settings");
        private void systemInformationButton_Click(object sender, EventArgs e) => LoadControl("SystemInformation");
        private void accountsButton_Click(object sender, EventArgs e) => LoadControl("RealeaseBilling");
        private void transactionsButton_Click(object sender, EventArgs e) => LoadControl("Transactions");

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadControl("Billing");
            usernameLabel.Text = $"{UserCredentials.Username}";
        }
        private void reloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentControlName))
            {
                MessageBox.Show("No control currently loaded.", "Reload Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🧹 Remove the old control from panel
            if (loadedControls.ContainsKey(currentControlName))
            {
                var oldControl = loadedControls[currentControlName];
                mainPanel.Controls.Remove(oldControl);
                oldControl.Dispose(); // Optional but good practice
                loadedControls.Remove(currentControlName);
            }

            LoadControl("Billing");

        }


        private void logoutButton_Click(object sender, EventArgs e)
        {
            // You can add logout logic here
        }
    }
}
