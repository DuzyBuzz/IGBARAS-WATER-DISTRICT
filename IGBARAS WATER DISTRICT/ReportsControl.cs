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
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class ReportsControl : UserControl
    {

        public ReportsControl()
        {
            InitializeComponent();
        }

        private DataTable dailyBillingReportDataTable = new DataTable();

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

            AppendColoredLine(box, $"💰 Total Paid: ₱{totalPaid:N2}", Color.ForestGreen);
            AppendColoredLine(box, $"🧾 Total Bills: {totalBills}", Color.MediumBlue);
            AppendColoredLine(box, $"🔴 Outstanding: ₱{totalBalance:N2}", Color.DarkRed);


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

            AppendColoredLine(box, $"💵 Total Collected: ₱{totalCollected:N2}", Color.ForestGreen);
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

            AppendColoredLine(box, $"💸 Total Penalty: ₱{totalPenalty:N2}", Color.DarkOrange);
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

            AppendColoredLine(box, $"💰 Total Paid: ₱{totalPaid:N2}", Color.ForestGreen);
            AppendColoredLine(box, $"🔴 Still Due: ₱{totalBalance:N2}", Color.DarkRed);
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

            AppendColoredLine(box, $"🔴 Accounts: {count}", Color.DarkRed);
            AppendColoredLine(box, $"💸 Total Balance: ₱{totalBalance:N2}", Color.OrangeRed);


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

            AppendColoredLine(box, $"💸 Total Outstanding: ₱{totalOutstanding:N2}", Color.DarkRed);
            AppendColoredLine(box, $"🔴 Affected Accounts: {count}", Color.MediumBlue);


        }

        private void exportReportsButton_Click(object sender, EventArgs e)
        {
            // 🔹 Prepare list to hold selected DataTables
            List<DataTable> selectedReports = new List<DataTable>();

            // 🔹 Check each checkbox and add corresponding report
            if (billingSummaryDailyCheckBox.Checked)
            {
                string dailyQuery = @"SELECT DATE(datebilled) AS BillingDate,
                                     COUNT(*) AS TotalBills,
                                     SUM(totalbillcharge) AS TotalAmount
                              FROM tb_bill
                              GROUP BY DATE(datebilled)";
                selectedReports.Add(ReportHelper.GetDataTable(dailyQuery, "Daily Billing Report"));
            }

            if (billingSummaryMonthlyCheckBox.Checked)
            {
                string monthlyQuery = @"SELECT CONCAT(YEAR(datebilled), '-', LPAD(MONTH(datebilled), 2, '0')) AS BillingMonth,
                                       COUNT(*) AS TotalBills,
                                       SUM(totalbillcharge) AS TotalAmount
                                FROM tb_bill
                                GROUP BY YEAR(datebilled), MONTH(datebilled)";
                selectedReports.Add(ReportHelper.GetDataTable(monthlyQuery, "Monthly Billing Report"));
            }

            if (billingSummaryYearlyCheckBox.Checked)
            {
                string yearlyQuery = @"SELECT YEAR(datebilled) AS BillingYear,
                                      COUNT(*) AS TotalBills,
                                      SUM(totalbillcharge) AS TotalAmount
                               FROM tb_bill
                               GROUP BY YEAR(datebilled)";
                selectedReports.Add(ReportHelper.GetDataTable(yearlyQuery, "Yearly Billing Report"));
            }

            // ❌ If no reports selected, alert the user
            if (selectedReports.Count == 0)
            {
                MessageBox.Show("⚠️ Please select at least one report to export.", "No Reports", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 📁 Choose where to save
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = $"BillingReports_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // ✅ Export all selected reports
                    ReportHelper.ExportToExcel(sfd.FileName, selectedReports.ToArray());

                    MessageBox.Show("✅ Report(s) exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void ChecUnCheck()
        {
            if (allCheckBox.Checked)
            {
                billingSummaryDailyCheckBox.Checked = true;
                billingSummaryDailyCheckBox.Checked = true;
                billingSummaryMonthlyCheckBox.Checked = true;
                billingSummaryYearlyCheckBox.Checked = true;
                collectionSummaryDailyCheckBox.Checked = true;
                collectionSummaryMonthlyCheckBox.Checked = true;
                collectionSummaryYearlyCheckBox.Checked = true;
                penaltyRevenueCheckBox.Checked = true;
                partiallyPaidCheckBox.Checked = true;
                disconnectionCheckBox.Checked = true;
                outstandingBalancesCheckBox.Checked = true;
            }
            else
            {
                billingSummaryDailyCheckBox.Checked = false;
                billingSummaryDailyCheckBox.Checked = false;
                billingSummaryMonthlyCheckBox.Checked = false;
                billingSummaryYearlyCheckBox.Checked = false;
                collectionSummaryDailyCheckBox.Checked = false;
                collectionSummaryMonthlyCheckBox.Checked = false;
                collectionSummaryYearlyCheckBox.Checked = false;
                penaltyRevenueCheckBox.Checked = false;
                partiallyPaidCheckBox.Checked = false;
                disconnectionCheckBox.Checked = false;
                outstandingBalancesCheckBox.Checked = false;
                allCheckBox.Checked = IsAllChecked();
            }
        }

        private bool IsAllChecked()
        {
            return billingSummaryDailyCheckBox.Checked &&
                   billingSummaryMonthlyCheckBox.Checked &&
                   billingSummaryYearlyCheckBox.Checked &&
                   collectionSummaryDailyCheckBox.Checked &&
                   collectionSummaryMonthlyCheckBox.Checked &&
                   collectionSummaryYearlyCheckBox.Checked &&
                   penaltyRevenueCheckBox.Checked &&
                   partiallyPaidCheckBox.Checked &&
                   disconnectionCheckBox.Checked &&
                   outstandingBalancesCheckBox.Checked;
        }
        private void allCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ChecUnCheck();
        }

        private void disconnectionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private void outstandingBalancesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private void penaltyRevenueCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private void partiallyPaidCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private void billingSummaryYearlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private void collectionSummaryYearlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private void collectionSummaryDailyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private void billingSummaryDailyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            allCheckBox.Checked = IsAllChecked();
        }

        private async void refreshReportsButton_Click(object sender, EventArgs e)
        {
            refreshReportsButton.Enabled = false;
            refreshReportsButton.Text = "⏳ Refreshing...";

            await LoadReportSummariesAsync();

            refreshReportsButton.Text = "🔁 Refresh Reports";
            refreshReportsButton.Enabled = true;
        }
    }
}
