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
            this.Load += AccountsControl_Load;
        }

        private async void AccountsControl_Load(object sender, EventArgs e)
        {
            try
            {
                LoadZoneComboBox();
                PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "Fullname or Account Number.");
                AutoCompleteHelper.FillTextBoxWithColumns("v_concessionaire_detail", new[] { "accountno", "name" }, searchAccountNumberTextBox);
                FormatDataGridView(accountDataGridView);
                await LoadAccountDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAccountDataAsync()
        {
            using (var loadingForm = new LoadingForm())
            {
                await DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_concessionaire_detail", loadingForm);
            }
            await LoadAgingOfAccountsAsync();
        }

        private void FormatDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Arial", 10);
            dgv.EnableHeadersVisualStyles = false;
        }

        private void searchAccountNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string keyword = searchAccountNumberTextBox.Text.Trim().Replace("'", "''");
                if (accountDataGridView.DataSource is DataTable dt)
                {
                    dt.DefaultView.RowFilter = $"accountno LIKE '%{keyword}%' OR name LIKE '%{keyword}%'";
                }
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool hasText = !string.IsNullOrEmpty(searchAccountNumberTextBox.Text);
            clearButton.ForeColor = hasText ? Color.Crimson : Color.Gray;
            clearButton.Enabled = hasText;
        }

        private void accountDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string accountno = accountDataGridView.Rows[e.RowIndex].Cells["accountno"].Value?.ToString();
            if (string.IsNullOrEmpty(accountno)) return;

            using (var editForm = new EditWaterMeterForm(accountno))
            {
                editForm.ShowDialog();
            }
            _ = LoadAccountDataAsync();
        }

        private async Task LoadAgingOfAccountsAsync()
        {
            try
            {
                string query = @"
SELECT
    b.name AS FullName,
    c.address,
    c.businessname,
    c.meterno,
    SUM(b.balance) AS TotalBalance,
    SUM(CASE WHEN DATEDIFF(CURDATE(), b.duedate) <= 30 THEN b.balance ELSE 0 END) AS Days_0_30,
    SUM(CASE WHEN DATEDIFF(CURDATE(), b.duedate) BETWEEN 31 AND 60 THEN b.balance ELSE 0 END) AS Days_31_60,
    SUM(CASE WHEN DATEDIFF(CURDATE(), b.duedate) BETWEEN 61 AND 90 THEN b.balance ELSE 0 END) AS Days_61_90,
    SUM(CASE WHEN DATEDIFF(CURDATE(), b.duedate) > 90 THEN b.balance ELSE 0 END) AS Days_91_Up
FROM tb_bill b
JOIN tb_concessionaire c ON b.accountno = c.accountno
WHERE b.balance > 0
GROUP BY b.name, c.address, c.businessname, c.meterno
ORDER BY TotalBalance DESC;

                ";

                using var con = new MySqlConnection(DbConfig.ConnectionString);
                await con.OpenAsync();

                using var cmd = new MySqlCommand(query, con);
                using var adapter = new MySqlDataAdapter(cmd);

                DataTable agingTable = new DataTable();
                adapter.Fill(agingTable);

                agingOfAccountDGV.DataSource = agingTable;

                // Column Header Customization
                var colMap = new Dictionary<string, string>
                {
                    ["accountno"] = "Account No",
                    ["FullName"] = "Customer Name",
                    ["address"] = "Address",
                    ["meterno"] = "Meter No.",
                    ["businessname"] = "Business Name",
                    ["TotalBalance"] = "Total Balance",
                    ["Days_0_30"] = "0–30 Days",
                    ["Days_31_60"] = "31–60 Days",
                    ["Days_61_90"] = "61–90 Days",
                    ["Days_91_Up"] = "91+ Days"
                };
                foreach (var kvp in colMap)
                {
                    if (agingOfAccountDGV.Columns.Contains(kvp.Key))
                        agingOfAccountDGV.Columns[kvp.Key].HeaderText = kvp.Value;
                }

                // Format money columns
                string[] moneyCols = { "TotalBalance", "Days_0_30", "Days_31_60", "Days_61_90", "Days_91_Up" };
                foreach (string colName in moneyCols)
                {
                    if (agingOfAccountDGV.Columns.Contains(colName))
                    {
                        var col = agingOfAccountDGV.Columns[colName];
                        col.DefaultCellStyle.Format = "N2";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        col.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }

                // Add ₱ prefix via CellFormatting (only add handler once)
                if (!_cellFormattingAdded)
                {
                    agingOfAccountDGV.CellFormatting += AgingOfAccountDGV_CellFormatting;
                    _cellFormattingAdded = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading aging of accounts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool _cellFormattingAdded = false;
        private void AgingOfAccountDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string[] moneyCols = { "TotalBalance", "Days_0_30", "Days_31_60", "Days_61_90", "Days_91_Up" };
            var dgv = sender as DataGridView;
            string colName = dgv.Columns[e.ColumnIndex].Name;
            if (moneyCols.Contains(colName) && e.Value != null && double.TryParse(e.Value.ToString(), out double val))
            {
                e.Value = $"₱{val:N2}";
                e.FormattingApplied = true;
            }
        }

        private void dailyCollectionExportButton_Click(object sender, EventArgs e)
        {
            DGVExcelExporter.ExportToExcel(agingOfAccountDGV, "Aging_of_Accounts_Report");
        }

        private void dailyCollectionPrintButton_Click(object sender, EventArgs e)
        {
            PrinterService.PrintDataGridView(agingOfAccountDGV, "Aging of Accounts Report");
        }

        private void LoadZoneComboBox()
        {
            int districtNo = 1; // Replace with actual district if needed
            var zoneList = ZoneHelper.GetZoneCodeHelper(districtNo);

            zoneComboBox.DataSource = zoneList;
            zoneComboBox.DisplayMember = "ZoneCode";
            zoneComboBox.ValueMember = "ZoneCode";

            if (zoneComboBox.Items.Count > 0)
                zoneComboBox.SelectedIndex = 0;
        }

        private void zoneComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string zoneCode = zoneComboBox.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(zoneCode)) return;

            if (accountDataGridView.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = $"accountno LIKE '{zoneCode}-%'";
                dt.DefaultView.Sort = "accountno ASC";
            }
        }
    }
}
