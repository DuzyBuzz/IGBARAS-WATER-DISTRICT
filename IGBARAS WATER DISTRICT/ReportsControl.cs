using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class ReportsControl : UserControl
    {
        public ReportsControl()
        {
            InitializeComponent();
        }

        private DataTable billingDataTable = new DataTable();

        private async void ReportsControl_Load(object sender, EventArgs e)
        {
            await LoadReportSummariesAsync();
        }

        private async Task LoadReportSummariesAsync()
        {
            // Billing Summaries
            await SetBillingSummaryAsync("Daily", billingSummaryDailyRichTextBox);
            await SetBillingSummaryAsync("Monthly", billingSummaryMonthlyRichTextBox);
            await SetBillingSummaryAsync("Yearly", billingSummaryYearlyRichTextBox);

            // Collection Summaries
            await SetCollectionSummaryAsync("Daily", collectionSummaryDailyRichTextBox);
            await SetCollectionSummaryAsync("Monthly", collectionSummaryMonthlyRichTextBox);
            await SetCollectionSummaryAsync("Yearly", collectionSummaryYearlyRichTextBox);

            // Penalty Revenue
            await SetPenaltyRevenueSummaryAsync(penaltyRevenueRichTextBox);

            // Partially Paid Bills
            await SetPartiallyPaidSummaryAsync(partiallyPaidRichTextBox);

            // Disconnection Candidates
            await SetDisconnectionSummaryAsync(disconnectionRichTextBox);

            // Outstanding Balances
            await SetOutstandingSummaryAsync(outstandingBalancesRichTextBox);
        }

        private void AppendColoredLine(RichTextBox box, string text, Color color, bool bold = false)
        {
            int start = box.TextLength;
            box.AppendText(text + Environment.NewLine);
            box.Select(start, text.Length);
            box.SelectionColor = color;

            if (bold)
                box.SelectionFont = new Font(box.Font, FontStyle.Bold);

            box.SelectionLength = 0;
            box.SelectionColor = box.ForeColor;
            box.SelectionFont = box.Font;
        }

        private async Task SetBillingSummaryAsync(string group, RichTextBox box)
        {
            box.Clear();

            var table = await ReportQueries.GetBillingSummaryAsync(group);
            if (table.Rows.Count == 0)
            {
                box.Text = "No billing data available.";
                return;
            }

            int totalBills = 0;
            double totalPaid = 0, totalBalance = 0;

            foreach (DataRow row in table.Rows)
            {
                totalBills += Convert.ToInt32(row["Number of Bills"]);
                totalPaid += Convert.ToDouble(row["Total Paid"]);
                totalBalance += Convert.ToDouble(row["Total Balance"]);
            }

            AppendColoredLine(box, $"📄 Billing Summary ({group})", Color.Black, true);
            AppendColoredLine(box, $"💰 Total Paid: {totalPaid:C}", Color.ForestGreen);
            AppendColoredLine(box, $"🧾 Total Bills: {totalBills}", Color.MediumBlue);
            AppendColoredLine(box, $"🔴 Outstanding: {totalBalance:C}", Color.DarkRed);


        }

        private async Task SetCollectionSummaryAsync(string group, RichTextBox box)
        {
            box.Clear();

            var table = await ReportQueries.GetCollectionSummaryAsync(group);
            if (table.Rows.Count == 0)
            {
                box.Text = "No collection data available.";
                return;
            }

            int totalPayments = 0;
            double totalCollected = 0;

            foreach (DataRow row in table.Rows)
            {
                totalPayments += Convert.ToInt32(row["Number of Payments"]);
                totalCollected += Convert.ToDouble(row["Total Collected"]);
            }

            AppendColoredLine(box, $"📄 Collection Summary ({group})", Color.Black, true);
            AppendColoredLine(box, $"💵 Total Collected: {totalCollected:C}", Color.ForestGreen);
            AppendColoredLine(box, $"🧾 Payments Made: {totalPayments}", Color.MediumBlue);


        }

        private async Task SetPenaltyRevenueSummaryAsync(RichTextBox box)
        {
            box.Clear();

            var table = await ReportQueries.GetPenaltyRevenueByMonthAsync();
            if (table.Rows.Count == 0)
            {
                box.Text = "No penalty data found.";
                return;
            }

            double totalPenalty = 0;
            int penaltyBills = 0;

            foreach (DataRow row in table.Rows)
            {
                penaltyBills += Convert.ToInt32(row["Bills With Penalty"]);
                totalPenalty += Convert.ToDouble(row["Total Penalty Revenue"]);
            }

            AppendColoredLine(box, "📄 Penalty Revenue (Monthly)", Color.Black, true);
            AppendColoredLine(box, $"💸 Total Penalty: {totalPenalty:C}", Color.DarkOrange);
            AppendColoredLine(box, $"📑 Bills With Penalty: {penaltyBills}", Color.MediumBlue);


        }

        private async Task SetPartiallyPaidSummaryAsync(RichTextBox box)
        {
            box.Clear();

            var table = await ReportQueries.GetPartiallyPaidBillsAsync();
            if (table.Rows.Count == 0)
            {
                box.Text = "No partially paid bills.";
                return;
            }

            double totalPaid = 0, totalBalance = 0;
            int count = table.Rows.Count;

            foreach (DataRow row in table.Rows)
            {
                totalPaid += Convert.ToDouble(row["amountpaid"]);
                totalBalance += Convert.ToDouble(row["balance"]);
            }

            AppendColoredLine(box, "📄 Partially Paid Bills", Color.Black, true);
            AppendColoredLine(box, $"💰 Total Paid: {totalPaid:C}", Color.ForestGreen);
            AppendColoredLine(box, $"🔴 Still Due: {totalBalance:C}", Color.DarkRed);
            AppendColoredLine(box, $"🧾 Partial Bills: {count}", Color.MediumBlue);


        }

        private async Task SetDisconnectionSummaryAsync(RichTextBox box)
        {
            box.Clear();

            var table = await ReportQueries.GetDisconnectionCandidatesAsync();
            if (table.Rows.Count == 0)
            {
                box.Text = "No disconnection candidates.";
                return;
            }

            int count = table.Rows.Count;
            double totalBalance = 0;

            foreach (DataRow row in table.Rows)
                totalBalance += Convert.ToDouble(row["balance"]);

            AppendColoredLine(box, "📄 Disconnection Candidates", Color.Black, true);
            AppendColoredLine(box, $"🔴 Accounts: {count}", Color.DarkRed);
            AppendColoredLine(box, $"💸 Total Balance: {totalBalance:C}", Color.OrangeRed);


        }

        private async Task SetOutstandingSummaryAsync(RichTextBox box)
        {
            box.Clear();

            var table = await ReportQueries.GetOutstandingBalancesAsync();
            if (table.Rows.Count == 0)
            {
                box.Text = "No outstanding balances.";
                return;
            }

            int count = table.Rows.Count;
            double totalOutstanding = 0;

            foreach (DataRow row in table.Rows)
                totalOutstanding += Convert.ToDouble(row["Total Outstanding Balance"]);

            AppendColoredLine(box, "📄 Outstanding Balances", Color.Black, true);
            AppendColoredLine(box, $"💸 Total Outstanding: {totalOutstanding:C}", Color.DarkRed);
            AppendColoredLine(box, $"🔴 Affected Accounts: {count}", Color.MediumBlue);


        }

        private async void exportReportsButton_Click(object sender, EventArgs e)
        {
            DataSet selectedReports = new DataSet();

            // Billing
            if (billingSummaryDailyCheckBox.Checked)
            {
                var table = await ReportQueries.GetBillingSummaryAsync("Daily");
                table.TableName = "Billing Summary (Daily)";
                selectedReports.Tables.Add(table);
            }

            if (billingSummaryMonthlyCheckBox.Checked)
            {
                var table = await ReportQueries.GetBillingSummaryAsync("Monthly");
                table.TableName = "Billing Summary (Monthly)";
                selectedReports.Tables.Add(table);
            }

            if (billingSummaryYearlyCheckBox.Checked)
            {
                var table = await ReportQueries.GetBillingSummaryAsync("Yearly");
                table.TableName = "Billing Summary (Yearly)";
                selectedReports.Tables.Add(table);
            }

            // Collection
            if (collectionSummaryDailyCheckBox.Checked)
            {
                var table = await ReportQueries.GetCollectionSummaryAsync("Daily");
                table.TableName = "Collection Summary (Daily)";
                selectedReports.Tables.Add(table);
            }

            if (collectionSummaryMonthlyCheckBox.Checked)
            {
                var table = await ReportQueries.GetCollectionSummaryAsync("Monthly");
                table.TableName = "Collection Summary (Monthly)";
                selectedReports.Tables.Add(table);
            }

            if (collectionSummaryYearlyCheckBox.Checked)
            {
                var table = await ReportQueries.GetCollectionSummaryAsync("Yearly");
                table.TableName = "Collection Summary (Yearly)";
                selectedReports.Tables.Add(table);
            }

            // Others
            if (penaltyRevenueCheckBox.Checked)
            {
                var table = await ReportQueries.GetPenaltyRevenueByMonthAsync();
                table.TableName = "Penalty Revenue";
                selectedReports.Tables.Add(table);
            }

            if (partiallyPaidCheckBox.Checked)
            {
                var table = await ReportQueries.GetPartiallyPaidBillsAsync();
                table.TableName = "Partially Paid Bills";
                selectedReports.Tables.Add(table);
            }

            if (disconnectionCheckBox.Checked)
            {
                var table = await ReportQueries.GetDisconnectionCandidatesAsync();
                table.TableName = "Disconnection Candidates";
                selectedReports.Tables.Add(table);
            }

            if (outstandingBalancesCheckBox.Checked)
            {
                var table = await ReportQueries.GetOutstandingBalancesAsync();
                table.TableName = "Outstanding Balances";
                selectedReports.Tables.Add(table);
            }

            // Check if empty
            if (selectedReports.Tables.Count == 0)
            {
                MessageBox.Show("Please select at least one report to export.", "No Reports Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save dialog
            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"WaterReports_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await ExcelExportHelper.ExportReportsToExcelAsync(selectedReports, saveFileDialog.FileName);
                    MessageBox.Show("✅ Reports successfully exported!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


    }
}
