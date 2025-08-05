using IGBARAS_WATER_DISTRICT.Helpers;
using Org.BouncyCastle.Asn1.Cmp;
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
    public partial class BillSettingsControl : UserControl
    {
        public BillSettingsControl()
        {
            InitializeComponent();
        }

        private void BillSettingsControl_Load(object sender, EventArgs e)
        {
            TableLoaderHelper.LoadTableToGrid(serviceDataGridView, "Tb_Service");
            TableLoaderHelper.LoadTableToGrid(settingsDataGidView, "Tb_Settings");
            TableLoaderHelper.LoadTableToGrid(zoneDataGridView, "Tb_Zone");
            TableLoaderHelper.LoadTableToGrid(discountDataGridView, "Tb_Discount");
            settingsDataGidView.AllowUserToAddRows = false;
            settingsDataGidView.RowHeadersVisible = false;
        }
        private void undoServiceButton_Click(object sender, EventArgs e)
        {
            TableLoaderHelper.LoadTableToGrid(serviceDataGridView, "Tb_Service");
        }
        private void applyServiceButton_Click(object sender, EventArgs e)
        {
            TableUpdaterHelper.UpdateTableFromGrid(serviceDataGridView, "Tb_Service", "ServiceID");
        }

        private void settingsApplyButton_Click(object sender, EventArgs e)
        {
            TableUpdaterHelper.UpdateTableFromGrid(settingsDataGidView, "Tb_Settings", "SettingID");

        }

        private void settingsUndoButton_Click(object sender, EventArgs e)
        {
            TableLoaderHelper.LoadTableToGrid(settingsDataGidView, "Tb_Settings");
        }

        private void discountApplyButton_Click(object sender, EventArgs e)
        {
            TableUpdaterHelper.UpdateTableFromGrid(discountDataGridView, "Tb_Discount", "DiscountID");
        }

        private void discountUndoButton_Click(object sender, EventArgs e)
        {
            TableLoaderHelper.LoadTableToGrid(discountDataGridView, "Tb_Discount");

        }

        private void zoneUndoButton_Click(object sender, EventArgs e)
        {
            TableLoaderHelper.LoadTableToGrid(zoneDataGridView, "Tb_Zone");

        }

        private void zoneApplyButton_Click(object sender, EventArgs e)
        {
            TableUpdaterHelper.UpdateTableFromGrid(zoneDataGridView, "Tb_Zone", "ZoneID");
        }
    }
}
