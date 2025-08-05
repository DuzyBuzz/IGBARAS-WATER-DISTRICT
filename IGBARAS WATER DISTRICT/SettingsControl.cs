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

namespace IGBARAS_WATER_DISTRICT
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        private void SettingsControl_Load(object sender, EventArgs e)
        {
            PlaceholderHelper.AddPlaceholder(currentPasswordTextBox, "Current Password.");
            PlaceholderHelper.AddPlaceholder(newPasswordTextBox, "New Password.");
            PlaceholderHelper.AddPlaceholder(confirmPasswordTextBox, "Confirm Password");
            fullnameTextBox.Text = $"{UserCredentials.Fullname}";
            userNameTextBox.Text = $"{UserCredentials.Username}";

        }
    }
}
