using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class AccountsControl : UserControl
    {
        public AccountsControl()
        {
            InitializeComponent();
        }

        private async void AccountsControl_Load(object sender, EventArgs e)
        {
            PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "🔎 Fullname or Account Number.");
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountsDataGridView, "Tb_Concessionaire", loadingForm);

                await Task.WhenAll(task1);
            }
            AutoCompleteHelper.FillTextBoxWithColumns("Tb_Concessionaire", new string[] { "AccountNo", "ConcessionaireName" }, searchAccountNumberTextBox);
            LoadZoneComboBox();

        }

        private void LoadZoneComboBox()
        {
            int districtNo = 1; // Replace with actual district if needed

            var zoneList = ZoneHelper.GetZoneCodeHelper(districtNo);

            zoneComboBox.DataSource = zoneList;
            zoneComboBox.DisplayMember = "ZoneCode"; // Shown: "01", "02", "11"
            zoneComboBox.ValueMember = "ZoneCode";   // Internal value: same as displayed

            if (zoneComboBox.Items.Count > 0)
                zoneComboBox.SelectedIndex = 0;
        }
        private void accountApplyButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to apply the changes to the concessionaire details?\n\n" +
                "This action will save the modifications to the database and may affect related records.",
                "Confirm Save",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                TableUpdaterHelper.UpdateTableFromGrid(accountsDataGridView, "Tb_Concessionaire", "ConcessionaireID");
            }
        }


        private async void accountUndoButton_Click(object sender, EventArgs e)
        {
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountsDataGridView, "Tb_Concessionaire", loadingForm);

                await Task.WhenAll(task1);
            }
        }

        private void searchAccountNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                string keyword = searchAccountNumberTextBox.Text.Trim();

                if (string.IsNullOrEmpty(keyword)) return;

                // Prevent special character issues
                keyword = keyword.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");

                if (accountsDataGridView.DataSource is DataTable dt)
                {
                    // Ensure exact column names are used from your MDB table
                    if (dt.Columns.Contains("AccountNo") && dt.Columns.Contains("ConcessionaireName"))
                    {
                        dt.DefaultView.RowFilter =
                            $"Convert(AccountNo, 'System.String') LIKE '%{keyword}%' OR Convert(ConcessionaireName, 'System.String') LIKE '%{keyword}%'";
                    }
                    else
                    {
                        MessageBox.Show("Ensure your MDB columns are named exactly 'AccountNo' and 'ConcessionaireName'.", "Column Name Error");
                    }
                }
            }
        }

        private void accountsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Only format once per row (check if you're on the first column, or skip if you like)
            if (e.RowIndex >= 0 && accountsDataGridView.Rows[e.RowIndex].Cells["Status"].Value != null)
            {
                string status = accountsDataGridView.Rows[e.RowIndex].Cells["Status"].Value.ToString().Trim().ToLower();

                if (status == "disconnected")
                {
                    accountsDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 204, 204);
                    accountsDataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black; // Optional
                }
                else if (status == "active")
                {
                    accountsDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 204, 255, 204);
                    accountsDataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black; // Optional
                }
                else
                {
                    // Reset for other statuses
                    accountsDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    accountsDataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void zoneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected zone code from ComboBox
            string zoneCode = zoneComboBox.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(zoneCode))
                return;

            if (accountsDataGridView.DataSource is DataTable dt)
            {
                // Filter rows where accountno starts with the selected zoneCode (e.g., "04-")
                dt.DefaultView.RowFilter = $"accountno LIKE '{zoneCode}-%'";

                // Sort rows in ascending order by accountno
                dt.DefaultView.Sort = "accountno ASC";
            }
        }

        private async void clearButton_Click(object sender, EventArgs e)
        {
            zoneComboBox.SelectedIndex = 0; // Clear the selection
            searchAccountNumberTextBox.Text = "";
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountsDataGridView, "Tb_Concessionaire", loadingForm);

                await Task.WhenAll(task1);
            }
            LoadZoneComboBox();
        }
    }
}
