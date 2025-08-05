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
        private Dictionary<string, Button> sidebarButtons;
        public MainForm()
        {
            InitializeComponent();

            // Initialize the sidebar button dictionary
            sidebarButtons = new Dictionary<string, Button>
            {
                { "RealeaseBilling", billingButton },
                { "Accounts", accountsButton },
                { "Reports", reportsButton },
                { "BillSettings", billSettingsButton },
                { "Settings", settingsButton },

            };
        }


        private void LoadControl(string controlName)
        {
            // 🚫 Avoid reloading the same control
            if (currentControlName == controlName)
                return;

            currentControlName = controlName;

            // 🧼 Hide all currently loaded controls
            foreach (var ctrl in loadedControls.Values)
                ctrl.Visible = false;

            // 🖍 Highlight the corresponding sidebar button
            HighlightActiveButton(controlName);

            // ✅ If already loaded, just show it
            if (loadedControls.ContainsKey(controlName))
            {
                loadedControls[controlName].Visible = true;
                return;
            }

            // 🛠 Dynamically create the control
            var type = Type.GetType($"IGBARAS_WATER_DISTRICT.{controlName}Control");

            if (type != null && type.IsSubclassOf(typeof(UserControl)))
            {
                var controlInstance = (UserControl)Activator.CreateInstance(type);
                controlInstance.Dock = DockStyle.Fill;

                // Add to panel and dictionary
                loadedControls[controlName] = controlInstance;
                mainPanel.Controls.Add(controlInstance);
                controlInstance.BringToFront();
            }
            else
            {
                MessageBox.Show($"Control '{controlName}' not found.");
            }
        }

        private void HighlightActiveButton(string controlName)
        {
            // Reset all buttons to default
            foreach (var btn in sidebarButtons.Values)
            {
                btn.BackColor = SystemColors.Control;
                btn.ForeColor = Color.Black;
                btn.Font = new Font(btn.Font, FontStyle.Regular);
            }

            // Highlight current button
            if (sidebarButtons.TryGetValue(controlName, out var activeButton))
            {
                activeButton.BackColor = Color.MediumSeaGreen;
                activeButton.ForeColor = Color.White;
                activeButton.Font = new Font(activeButton.Font, FontStyle.Bold);
            }
        }

        // Button click handlers
        private void settingsButton_Click(object sender, EventArgs e) => LoadControl("Settings");
        private void systemInformationButton_Click(object sender, EventArgs e) => LoadControl("SystemInformation");
        private void accountsButton_Click(object sender, EventArgs e) => LoadControl("Accounts");
        private void billingButton_Click(object sender, EventArgs e) => LoadControl("RealeaseBilling");
        private void billSettingsButton_Click(object sender, EventArgs e) => LoadControl("BillSettings");

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadControl("RealeaseBilling");
            usernameLabel.Text = $"{UserCredentials.Fullname}";
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

            LoadControl("RealeaseBilling");

        }


        private void logoutButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout and return to the login screen?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                // Open login form before closing main form
                Login loginForm = new Login();
                loginForm.Show();

                // Close current main form
            }
            // If "No" is selected, do nothing
        }


        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Show confirmation only if it's a user-initiated close (not from Application.Exit)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "You are about to close the application.\n\nDo you want to exit now?",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                // Close all running forms safely
                foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                {
                    form.FormClosing -= MainForm_FormClosing; // Unsubscribe to avoid second trigger
                    form.Close();
                }

                Application.Exit(); // Exit the application
            }
        }


        private void reportsButton_Click(object sender, EventArgs e)
        {
            LoadControl("Reports");
        }


    }
}
