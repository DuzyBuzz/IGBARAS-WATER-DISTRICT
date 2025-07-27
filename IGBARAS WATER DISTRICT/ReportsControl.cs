using ClosedXML.Excel;
using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class ReportsControl : UserControl
    {
        public ReportsControl() => InitializeComponent();

        private async void ReportsControl_Load(object sender, EventArgs e)
        {
            InitializeDateTimePickers();
            FormatAllGrids();
            await LoadAllReportsAsync();

        }

        #region Report Loading

        private async Task LoadAllReportsAsync()
        {
            await LoadReportAsync("daily", "billing", dailyBillDateTimePicker.Value, dailyBillingDGV);
            await LoadReportAsync("monthly", "billing", monthBillDateTimePicker.Value, monthlyBillingDGV);
            await LoadReportAsync("daily", "collection", dailyCollectionDateTimePicker.Value, dailyCollectionDGV);
            await LoadReportAsync("monthly", "collection", monthlyCollectionDateTimePicker.Value, monthlyCollectionDGV);
        }

        private async Task LoadReportAsync(
            string reportType,        // "daily" or "monthly"
            string reportCategory,    // "billing" or "collection"
            DateTime selectedDate,
            DataGridView dgv)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(DbConfig.ConnectionString))
                {
                    await con.OpenAsync();

                    string query = "";

                    // Billing Reports
                    if (reportCategory == "billing")
                    {
                        query = @"
SELECT 
    billcode AS 'Bill Code',
    accountno AS 'Account No.',
    name AS 'Name',
    address AS 'Address',
    meterno AS 'Meter No.',
    previousreading AS 'Previous Reading',
    presentreading AS 'Present Reading',
    meterconsumed AS 'Consumption',
    billcharge AS 'Bill Charge',
    arrearsamount AS 'Arrears',
    balance AS 'Balance',
    totalbillcharge AS 'Total Bill',
    datebilled AS 'Billing Date'
FROM tb_bill
WHERE {0}
ORDER BY datebilled DESC;";
                        query = string.Format(query,
                            reportType == "daily"
                                ? "DATE(datebilled) = @selectedDate"
                                : "MONTH(datebilled) = @month AND YEAR(datebilled) = @year");
                    }
                    // Collection Reports
                    else if (reportCategory == "collection")
                    {
                        query = @"
SELECT 
    p.ornumber AS 'OR No.',
    p.paymentdate AS 'Payment Date',
    p.accountno AS 'Account No.',
    CONCAT(c.lastname, ', ', c.firstname, ' ', COALESCE(c.mi, '')) AS 'Name',
    p.bill AS 'Bill',
    p.service_connection_fee AS 'Service Fee',
    p.total_other_charges AS 'Other Charges',
    p.total_discount AS 'Discount',
    p.tax AS 'Tax',
    p.penalty AS 'Penalty',
    b.adjustdebit AS 'Adjust Debit',
    b.adjustcredit AS 'Adjust Credit',
    b.totalbillcharge AS 'Total Bill Charge',
    b.amountpaid AS 'Amount Paid',
    b.fromreadingdate AS 'From Date',
    b.toreadingdate AS 'To Date',
    
    CASE 
        WHEN b.paid = 1 THEN 'Paid'
        WHEN b.partiallypaid = 1 THEN 'Partially Paid'
        ELSE 'Unpaid'
    END AS 'Status'

FROM tb_payment p
LEFT JOIN tb_bill b ON b.billcode = p.billcode
LEFT JOIN tb_concessionaire c ON c.accountno = p.accountno
WHERE {0}
ORDER BY p.paymentdate DESC;

";

                        // Replace {0} with filter condition for daily or monthly
                        query = string.Format(query,
                            reportType == "daily"
                                ? "DATE(p.paymentdate) = @selectedDate"
                                : "MONTH(p.paymentdate) = @month AND YEAR(p.paymentdate) = @year"
                        );



                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        if (reportType == "daily")
                        {
                            cmd.Parameters.AddWithValue("@selectedDate", selectedDate.Date);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@month", selectedDate.Month);
                            cmd.Parameters.AddWithValue("@year", selectedDate.Year);
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgv.Invoke((MethodInvoker)(() =>
                            {
                                dgv.DataSource = dt;
                                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                                string[] currencyColumns = new[]
                                {
                            "Bill Charge", "Penalty", "Arrears", "Total Bill", "Amount Paid", "Balance",
                            "Bill Amount", "Total Paid", "Cash Paid", "Service Fee", "Other Charges",
                            "Discounts", "Tax",  "Total Bill Charge", "Cash Tendered"
                        };

                                foreach (DataGridViewColumn col in dgv.Columns)
                                {
                                    if (currencyColumns.Contains(col.HeaderText))
                                    {
                                        col.DefaultCellStyle.Format = "₱#,##0.00";
                                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    }
                                }
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Failed to load {reportType} {reportCategory} report:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        #endregion

        #region UI Setup

        private void InitializeDateTimePickers()
        {
            monthBillDateTimePicker.Format = DateTimePickerFormat.Custom;
            monthBillDateTimePicker.CustomFormat = "MMMM yyyy";
            monthBillDateTimePicker.ShowUpDown = true;

            dailyBillDateTimePicker.Format = DateTimePickerFormat.Custom;
            dailyBillDateTimePicker.CustomFormat = "MMMM d, yyyy";
            dailyBillDateTimePicker.ShowUpDown = true;

            monthlyCollectionDateTimePicker.Format = DateTimePickerFormat.Custom;
            monthlyCollectionDateTimePicker.CustomFormat = "MMMM yyyy";
            monthlyCollectionDateTimePicker.ShowUpDown = true;


            dailyCollectionDateTimePicker.Format = DateTimePickerFormat.Custom;
            dailyCollectionDateTimePicker.CustomFormat = "MMMM d, yyyy";
            dailyCollectionDateTimePicker.ShowUpDown = true;
        }

        private void FormatAllGrids()
        {
            FormatDataGridView(dailyBillingDGV);
            FormatDataGridView(monthlyBillingDGV);
            FormatDataGridView(dailyCollectionDGV);
            FormatDataGridView(monthlyCollectionDGV);
        }

        private void FormatDataGridView(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Arial", 9);
            dgv.EnableHeadersVisualStyles = false;

        }

        #endregion

        #region Billing & Collection Summaries (Grouped)


        #endregion

        #region Export Reports


        private void printDGVReportButton_Click(object sender, EventArgs e)
        {
            var printableGrids = new List<DataGridView>
            {
                dailyBillingDGV,
                monthlyBillingDGV,
                dailyCollectionDGV,
                monthlyCollectionDGV
            }.FindAll(dgv => dgv.Visible && dgv.Rows.Count > 0);

            if (printableGrids.Count == 0)
            {
                MessageBox.Show("No reports available to print.", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var dgv in printableGrids)
            {
                var printer = new Helpers.DGVPrinter
                {
                    Title = "Republic of the Philippines\nIGBARAS WATER DISTRICT (ILOILO)",
                    SubTitle = GetSectionTitle(dgv.Name),
                    Footer = $"Printed on {DateTime.Now:MMMM dd, yyyy hh:mm tt}",
                    TitleFont = new Font("Arial", 11, FontStyle.Bold),
                    SubTitleFont = new Font("Arial", 10),
                    PrintHeader = true,
                    PrintFooter = true,
                    PageNumbers = true
                };

                printer.PrintPreviewDataGridView(dgv);
            }
        }

        private string GetSectionTitle(string dgvName) => dgvName switch
        {
            "dailyBillingDGV" => "Daily Billing Report",
            "monthlyBillingDGV" => "Monthly Billing Report",
            "dailyCollectionDGV" => "Daily Collection Report",
            "monthlyCollectionDGV" => "Monthly Collection Report",
            _ => "Water District Report"
        };

        #endregion

        private void dailyBillPrintButton_Click(object sender, EventArgs e)
        {
            PrinterService.PrintDataGridView(dailyBillingDGV, "Daily Billing Report");
            FormatAllGrids();
        }

        private async void dailyBillDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            await LoadReportAsync("daily", "billing", dailyBillDateTimePicker.Value, dailyBillingDGV);
        }

        private async void monthBillDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            await LoadReportAsync("monthly", "billing", monthBillDateTimePicker.Value, monthlyBillingDGV);
        }

        private void monthlyBillPrintButton_Click(object sender, EventArgs e)
        {

        }

        private void dailyCollectionPrintButton_Click(object sender, EventArgs e)
        {

        }

        private void monthlyCollectionPrintButton_Click(object sender, EventArgs e)
        {

        }

        private async void refreshReportsButton_Click(object sender, EventArgs e)
        {
            await LoadAllReportsAsync();
        }

        private void monthlyBillPrintButton_Click_1(object sender, EventArgs e)
        {
            PrinterService.PrintDataGridView(monthlyBillingDGV, "Monthly Billing Report");
            FormatAllGrids();
        }

        private void dailyCollectionPrintButton_Click_1(object sender, EventArgs e)
        {
            PrinterService.PrintDataGridView(dailyCollectionDGV, "Daily Collection Report");
            FormatAllGrids();
        }

        private void monthlyCollectionPrintButton_Click_1(object sender, EventArgs e)
        {
            PrinterService.PrintDataGridView(dailyCollectionDGV, "Monthly Collection Report");
            FormatAllGrids();
        }

        private async void monthlyCollectionDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            await LoadAllReportsAsync();
        }

        private async void dailyCollectionDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            await LoadAllReportsAsync();
        }

        private void exportReportsButton_Click(object sender, EventArgs e)
        {

        }
        private void dailyBillExportButton_Click(object sender, EventArgs e)
        {
            string date = dailyBillDateTimePicker.Value.ToString("MMMM_d_yyyy");
            DGVExcelExporter.ExportToExcel(dailyBillingDGV, $"Daily_Billing_Report_{date}");
        }

        private void monthlyBillExportButton_Click(object sender, EventArgs e)
        {
            string date = monthBillDateTimePicker.Value.ToString("MMMM_yyyy"); // Only month and year
            DGVExcelExporter.ExportToExcel(monthlyBillingDGV, $"Monthly_Billing_Report_{date}");
        }

        private void dailyCollectionExportButton_Click(object sender, EventArgs e)
        {
            string date = dailyCollectionDateTimePicker.Value.ToString("MMMM_d_yyyy");
            DGVExcelExporter.ExportToExcel(dailyCollectionDGV, $"Daily_Collection_Report_{date}");
        }

        private void monthlyCollectionExportButton_Click(object sender, EventArgs e)
        {
            string date = monthlyCollectionDateTimePicker.Value.ToString("MMMM_yyyy");
            DGVExcelExporter.ExportToExcel(monthlyCollectionDGV, $"Monthly_Collection_Report_{date}");
        }

        private void exportAllToExcelButton_Click(object sender, EventArgs e)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                // Format dates
                string dailyBillDate = dailyBillDateTimePicker.Value.ToString("MMMM_d_yyyy");
                string monthlyBillDate = monthBillDateTimePicker.Value.ToString("MMMM_yyyy");
                string dailyCollectionDate = dailyCollectionDateTimePicker.Value.ToString("MMMM_d_yyyy");
                string monthlyCollectionDate = monthlyCollectionDateTimePicker.Value.ToString("MMMM_yyyy");

                // Add each DataGridView to a separate worksheet
                AddDataGridViewToSheet(workbook, dailyBillingDGV, $"Daily Billing {dailyBillDate}");
                AddDataGridViewToSheet(workbook, monthlyBillingDGV, $"Monthly Billing {monthlyBillDate}");
                AddDataGridViewToSheet(workbook, dailyCollectionDGV, $"Daily Collection {dailyCollectionDate}");
                AddDataGridViewToSheet(workbook, monthlyCollectionDGV, $"Monthly Collection {monthlyCollectionDate}");

                // Fix: Access AccountsControl instance to retrieve agingOfAccountDGV
                var accountsControl = this.Parent?.Controls.OfType<AccountsControl>().FirstOrDefault();
                if (accountsControl != null)
                {
                    AddDataGridViewToSheet(workbook, accountsControl.agingOfAccountDGV, "Aging of Accounts");
                }

                // Save the file
                string fileName = $"All_Reports_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                    saveFileDialog.FileName = fileName;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Export successful!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void AddDataGridViewToSheet(XLWorkbook workbook, DataGridView dgv, string sheetName)
        {
            var worksheet = workbook.Worksheets.Add(sheetName);

            // Add column headers
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = dgv.Columns[i].HeaderText;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            // Add rows
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    if (dgv.Rows[i].Cells[j].Value != null)
                    {
                        worksheet.Cell(i + 2, j + 1).Value = dgv.Rows[i].Cells[j].Value.ToString();
                    }
                }
            }

            // Auto-adjust columns
            worksheet.Columns().AdjustToContents();
        }


    }



}
