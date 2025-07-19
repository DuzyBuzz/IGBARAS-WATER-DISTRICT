using IGBARAS_WATER_DISTRICT.Helpers;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class RealeaseBillingControl : UserControl
    {
        private string[] billData;
        private string[] selectedBillingData;

        public RealeaseBillingControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// this is the event handler for the print save button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printSaveButton_Click(object sender, EventArgs e)
        {
            if (selectedBillingData == null || selectedBillingData.Length < 17)
            {
                MessageBox.Show("No selected billing data. Please select an account first.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!CheckBillingDate())
            {
                MessageBox.Show("Invalid billing date. Cannot proceed with saving or printing.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                InsertToBillingTable(selectedBillingData);





                MessageBox.Show("Billing record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                billingPrintDocument.DefaultPageSettings.Landscape = true;

                foreach (PaperSize ps in billingPrintDocument.PrinterSettings.PaperSizes)
                {
                    if (ps.Kind == PaperKind.Legal)
                    {
                        billingPrintDocument.DefaultPageSettings.PaperSize = ps;
                        break;
                    }
                }

                billingPrintDocument.DefaultPageSettings.Margins = new Margins(30, 30, 30, 30); // 0.3 inch margins

                billingPrintDocument.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ An error occurred while saving or printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void GetDueDays()
        {
            string dueDateDaysStr = GetBillSettingsHelper.GetValue("duedateduration");

            if (int.TryParse(dueDateDaysStr, out int dueDays))
            {
                DateTime dueDate = DateTime.Today.AddDays(dueDays);
                string formattedDueDate = dueDate.ToString("MMMM d, yyyy");

                dueDateLabel.Text = formattedDueDate;
            }
            else
            {
                MessageBox.Show("⚠️ Invalid or missing due date setting.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private Bitmap CapturePanel(Control panel)
        {
            // Create a bitmap with the size of the panel
            Bitmap bmp = new Bitmap(panel.Width, panel.Height);
            panel.DrawToBitmap(bmp, new Rectangle(0, 0, panel.Width, panel.Height));
            return bmp;
        }
        private void billingPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Set label if needed
            copyTypeLabel.Text = "Concessionaire's Copy";

            // Capture the billing panel as image
            Bitmap panelImage = CapturePanel(billingPanel);

            // Use the printable area
            Rectangle marginBounds = e.MarginBounds;

            // Calculate scaling factor to fit within bounds
            float scale = Math.Min(
                (float)marginBounds.Width / panelImage.Width,
                (float)marginBounds.Height / panelImage.Height);

            int scaledWidth = (int)(panelImage.Width * scale);
            int scaledHeight = (int)(panelImage.Height * scale);

            // Compute center position within margin bounds
            int x = marginBounds.Left + (marginBounds.Width - scaledWidth) / 2;
            int y = marginBounds.Top + (marginBounds.Height - scaledHeight) / 2;

            // Use high-quality rendering
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            // Draw the scaled image at the center
            e.Graphics.DrawImage(panelImage, new Rectangle(x, y, scaledWidth, scaledHeight));

            panelImage.Dispose();

            e.HasMorePages = false;
        }
        /// <summary>

        /// <summary>
        /// end of the print save button click event.
        /// </summary>

        private bool CheckBillingDate()
        {
            // Get the account number from textbox
            string accountNo = accountNumberTextBox.Text.Trim();

            // Get current month and year
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            // SQL query to check if a bill already exists for this account in the current month and year
            string query = @"
        SELECT COUNT(*) FROM tb_bill 
        WHERE accountno = @accountno 
          AND month = @month 
          AND year = @year";

            try
            {
                // Create and open the MySQL connection
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // Create command object with query and connection
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Pass parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@accountno", accountNo);
                        cmd.Parameters.AddWithValue("@month", currentMonth);
                        cmd.Parameters.AddWithValue("@year", currentYear);

                        // Execute query and get the result
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        // If a bill exists, show message and return false (don't save)
                        if (count > 0)
                        {
                            MessageBox.Show("This customer is already billed for this month.", "Duplicate Billing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        // No existing bill, proceed with saving
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking billing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private string GetFullBillCode()
        {
            // "001" + "-" + invoiceLabel.Text (which is like "0000123")
            string zoneCode = GetZonePrefixFromAccountNo(accountNumberTextBox.Text);
            return $"{zoneCode}-{invoiceLabel.Text}";
        }
        private string RouteNumber()
        {
            string zoneCode = GetZonePrefixFromAccountNo(accountNumberTextBox.Text);

            // Convert "001" to 1
            int zoneNumeric = int.Parse(zoneCode); // removes leading zeros

            return $"{zoneNumeric}"; // will return "1"

        }
        private int GetDiscount()
        {


            if (!string.IsNullOrEmpty(discountedLabel.Text))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        private void SetDateNow()
        {
            // Set the current date and time to the label
            dateBilledLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            dateIssuedLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            toReadingDateLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
        }
        private void DisableButton()
        {
            if (string.IsNullOrEmpty(chargeLabel.Text))
            {
                printSaveButton.Enabled = false;
            }
            else
            {
                printSaveButton.Enabled = true;
            }
        }
        private void ClearButtonDisable()
        {
            if (!string.IsNullOrEmpty(searchAccountNumberTextBox.Text))
            {
                clearButton.ForeColor = Color.Crimson;
                clearButton.Enabled = true;
            }
            else
            {
                clearButton.ForeColor = Color.Gray;
                clearButton.Enabled = false;
            }
        }
        /// <summary>
        /// This takes the first 2 digits of the account number (e.g., "01", "02") and turns it into a zone prefix like "001", "002".
        /// </summary>
        public string GetZonePrefixFromAccountNo(string accountNo)
        {
            if (string.IsNullOrWhiteSpace(accountNo) || accountNo.Length < 2)
                return "001"; // default to Zone 1 if invalid

            string zone = accountNo.Substring(0, 2); // get '01', '02', etc.

            if (int.TryParse(zone, out int zoneNum))
                return zoneNum.ToString("D3"); // convert to "001", "002", etc.

            return "001"; // fallback if parsing fails
        }
        /// <summary>
        ///  This connects to the database and checks which zone number matches the zoneCode like "001", "002".
        ///    If not found, it defaults to Zone 1.
        ///    this help know the start range for invoice number per zone 
        ///    for example zone 1 starts at 1 - 200 the invoice will be 001-0000001 to 001-0000200 then next month it will be 001-0000201
        /// </summary>
        public int GetZoneNoFromDB(string zoneCode)
        {
            string query = "SELECT zoneno FROM tb_zone WHERE zonecode = @zonecode LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@zonecode", zoneCode);
                    object result = cmd.ExecuteScalar();

                    return result != null && result != DBNull.Value
                        ? Convert.ToInt32(result)
                        : 1; // default to Zone 1 if not found
                }
            }
        }

        /// <summary>
        ///  gets the zoneno to calculate the starting number for the next bill code. zone 1-1, zone 2 -201, etc.
        ///     adds a months offset to the starting number based on the month of the billMonth parameter. jan = 0, feb = 400, mar = 800, etc.
        ///     
        /// </summary>
        /// <summary>
        /// Generates the next bill code in the format "ZZZ-0000001" where:
        /// ZZZ = zone code (3 digits)
        /// 0000001 = padded bill number (7 digits)
        /// </summary>
        /// <summary>
        /// Returns the next bill number (int) for the given zone and billing month
        /// </summary>

        private int GetLastBillNumberUsed(string zoneCode, DateTime billingDate)
        {
            string yearMonth = billingDate.ToString("yyyyMM");
            int lastNumber = 0;

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                string query = @"SELECT MAX(CAST(SUBSTRING_INDEX(billcode, '-', -1) AS UNSIGNED)) 
                         FROM tb_bill 
                         WHERE zonecode = @zone AND DATE_FORMAT(billingmonth, '%Y%m') = @yearMonth";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@zone", zoneCode);
                    cmd.Parameters.AddWithValue("@yearMonth", yearMonth);

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        int.TryParse(result.ToString(), out lastNumber);
                    }
                }
            }

            return lastNumber;
        }


        private string FormatBillCode(string zoneCode, int billNumber)
        {
            return $"{zoneCode}-{billNumber.ToString("D7")}";
        }




        private void InsertToBillingTable(string[] data)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
            INSERT INTO tb_bill (
                billcode, billnumber, accountno, routeno, concessionairecode, name, address,
                districtno, zonecode, servicecode, servicetype, meterno, taxexempt, dueexempt,
                withholdingtax, wtpercent, wtamount, seniorcitizen, scpercent,
                fromreadingdate, toreadingdate, previousreading, presentreading, meterconsumed,
                charge, taxpercent, taxamount, senioramount, month, year,
                totaladditionalcharge, totalbillcharge, duedate, datebilled,
                arrearsamount, billcharge, balance, paid, firstbill, penaltyamount,
                arrears, duegraceperiod, amountpaid, adjustdebit, adjustcredit,
                partiallypaid, othermeterconsumed, presentmeterconsumed, electriccharge,
                uploaded, disconnectiondate
            ) VALUES (
                @billcode, @billnumber, @accountno, @routeno, @concessionairecode, @name, @address,
                @districtno, @zonecode, @servicecode, @servicetype, @meterno, @taxexempt, @dueexempt,
                @withholdingtax, @wtpercent, @wtamount, @seniorcitizen, @scpercent,
                @fromreadingdate, @toreadingdate, @previousreading, @presentreading, @meterconsumed,
                @charge, @taxpercent, @taxamount, @senioramount, @month, @year,
                @totaladditionalcharge, @totalbillcharge, @duedate, @datebilled,
                @arrearsamount, @billcharge, @balance, @paid, @firstbill, @penaltyamount,
                @arrears, @duegraceperiod, @amountpaid, @adjustdebit, @adjustcredit,
                @partiallypaid, @othermeterconsumed, @presentmeterconsumed, @electriccharge,
                @uploaded, @disconnectiondate
            )";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Clean and parse percentage strings from labels
                        string taxPercentStr = taxExemptedLabel.Text.Trim().Replace("%", "");
                        string discountPercentStr = discountedAmountLabel.Text.Trim().Replace("%", "");

                        if (!int.TryParse(taxPercentStr, out int taxPercent))
                        {
                            MessageBox.Show("Invalid tax percent format.", "Input Error");
                            return;
                        }

                        // Format date labels safely
                        string formattedFromDate = DateTime.TryParse(fromReadingDateLabel.Text, out var fromDate)
                            ? fromDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                        string formattedToDate = DateTime.TryParse(toReadingDateLabel.Text, out var toDate)
                            ? toDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                        string formattedDueDate = DateTime.TryParse(dueDateLabel.Text, out var dueDate)
                            ? dueDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                        string formattedDateBilled = DateTime.Now.ToString("yyyy-MM-dd");

                        // Required parameters from string[] data
                        cmd.Parameters.AddWithValue("@accountno", data[0]);
                        cmd.Parameters.AddWithValue("@name", data[1]);
                        cmd.Parameters.AddWithValue("@address", data[2]);
                        cmd.Parameters.AddWithValue("@concessionairecode", data[3]);
                        cmd.Parameters.AddWithValue("@zonecode", data[4]);
                        cmd.Parameters.AddWithValue("@servicecode", data[5]);
                        cmd.Parameters.AddWithValue("@servicetype", data[6]);
                        cmd.Parameters.AddWithValue("@meterno", data[7]);
                        cmd.Parameters.AddWithValue("@dueexempt", Convert.ToInt32(data[8]));
                        cmd.Parameters.AddWithValue("@withholdingtax", Convert.ToInt32(data[9]));
                        cmd.Parameters.AddWithValue("@wtpercent", Convert.ToInt32(data[10]));
                        cmd.Parameters.AddWithValue("@scpercent", Convert.ToInt32(data[11]));
                        cmd.Parameters.AddWithValue("@routeno", Convert.ToInt32(data[12]));
                        cmd.Parameters.AddWithValue("@taxexempt", Convert.ToInt32(data[13]));
                        cmd.Parameters.AddWithValue("@seniorcitizen", Convert.ToInt32(data[14]));
                        cmd.Parameters.AddWithValue("@districtno", Convert.ToInt32(data[16]));
                        cmd.Parameters.AddWithValue("@balance", Convert.ToDecimal(data[17]));

                        // Auto-calculated / UI values
                        cmd.Parameters.AddWithValue("@billcode", billCodeLabel.Text.Trim()); 
                        cmd.Parameters.AddWithValue("@billnumber", Convert.ToInt32(extractedBillNumberLabel.Text.Trim())); // the int part only
                        cmd.Parameters.AddWithValue("@fromreadingdate", formattedFromDate);
                        cmd.Parameters.AddWithValue("@toreadingdate", formattedToDate);
                        cmd.Parameters.AddWithValue("@duedate", formattedDueDate);
                        cmd.Parameters.AddWithValue("@duegraceperiod", formattedDueDate);
                        cmd.Parameters.AddWithValue("@datebilled", formattedDateBilled);

                        cmd.Parameters.AddWithValue("@previousreading", Convert.ToInt32(previousReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@presentreading", Convert.ToInt32(presentReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@meterconsumed", Convert.ToInt32(meterConsumedReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@charge", Convert.ToDecimal(chargeLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@taxpercent", taxPercent);

                        // Handle discount/tax amounts safely
                        decimal.TryParse(exemptedAmountLabel.Text.Replace("%", "").Trim(), out decimal taxAmount);
                        decimal.TryParse(discountedAmountLabel.Text.Replace("%", "").Trim(), out decimal discountAmount);
                        decimal.TryParse(arrearsLabel.Text.Trim(), out decimal arrearsAmount);

                        cmd.Parameters.AddWithValue("@taxamount", taxAmount);
                        cmd.Parameters.AddWithValue("@senioramount", discountAmount);
                        cmd.Parameters.AddWithValue("@arrearsamount", arrearsAmount);
                        cmd.Parameters.AddWithValue("@wtamount", 0.00m); // Set to 0.00 unless specified

                        cmd.Parameters.AddWithValue("@month", DateTime.Now.Month);
                        cmd.Parameters.AddWithValue("@year", DateTime.Now.Year);

                        // Fixed/default values
                        cmd.Parameters.AddWithValue("@paid", 0);
                        cmd.Parameters.AddWithValue("@firstbill", 0);
                        cmd.Parameters.AddWithValue("@penaltyamount", 0.00m);
                        cmd.Parameters.AddWithValue("@arrears", 0);
                        cmd.Parameters.AddWithValue("@amountpaid", 0.00m);
                        cmd.Parameters.AddWithValue("@adjustdebit", 0.00m);
                        cmd.Parameters.AddWithValue("@adjustcredit", 0.00m);
                        cmd.Parameters.AddWithValue("@totaladditionalcharge", 0.00m);
                        cmd.Parameters.AddWithValue("@totalbillcharge", Convert.ToDecimal(chargeLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@billcharge", Convert.ToDecimal(chargeLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@partiallypaid", 0);
                        cmd.Parameters.AddWithValue("@othermeterconsumed", 0);
                        cmd.Parameters.AddWithValue("@presentmeterconsumed", Convert.ToInt32(meterConsumedReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@electriccharge", 0.00m);
                        cmd.Parameters.AddWithValue("@uploaded", 0);
                        cmd.Parameters.AddWithValue("@disconnectiondate", DBNull.Value); // optional field

                        // Final insert execution
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Billing record inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting billing record:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private async void BillingControl_Load(object sender, EventArgs e)
        {
            ClearWaterChargeLabels();
            PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "🔎 Fullname or Account Number.");
            ClearButtonDisable();
            SetDateNow();
            GetDueDays();
            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_concessionaire_detail", loadingForm);
                var task2 = DGVHelper.LoadDataToGridAsync(printBillDataGridView, "tb_bill_print", loadingForm);

                await Task.WhenAll(task1, task2);
            }

            // 🟢 Optional: Setup autocomplete after data loaded
            AutoCompleteHelper.FillTextBoxWithColumns("v_concessionaire_detail", new string[] { "accountno", "fullname" }, searchAccountNumberTextBox);
        }





        private void accountDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ClearWaterChargeLabels();
            meterConsumedReadingTextBox.Text = "";
            totalQuantityLabel.Text = "";
            DisableButton();
            if (e.RowIndex < 0) return; // Ignore header or invalid rows

            // 🟦 Get selected row
            DataGridViewRow selectedRow = accountDataGridView.Rows[e.RowIndex];

            // 🟦 Extract individual values using the column names
            string accountNo = selectedRow.Cells["accountno"].Value?.ToString();
            string fullname = selectedRow.Cells["fullname"].Value?.ToString();
            string address = selectedRow.Cells["address"].Value?.ToString();
            string concessionCode = selectedRow.Cells["concessionairecode"].Value?.ToString();
            string zoneCode = selectedRow.Cells["zonecode"].Value?.ToString();
            string serviceCode = selectedRow.Cells["servicecode"].Value?.ToString();
            string serviceType = selectedRow.Cells["servicetype"].Value?.ToString();
            string meterNo = selectedRow.Cells["meterno"].Value?.ToString();
            string dueExempt = selectedRow.Cells["dueexempt"].Value?.ToString();
            string withholdingTax = selectedRow.Cells["withholdingtax"].Value?.ToString();
            string wtPercent = selectedRow.Cells["wtpercent"].Value?.ToString();
            string scPercent = selectedRow.Cells["scpercent"].Value?.ToString();
            string routeNo = selectedRow.Cells["routeno"].Value?.ToString();
            string taxExempted = selectedRow.Cells["taxexempt"].Value?.ToString();
            string discounted = selectedRow.Cells["seniorcitizen"].Value?.ToString();
            string billCode = selectedRow.Cells["billcodex"].Value?.ToString();
            string balance = selectedRow.Cells["balancex"].Value?.ToString();
            string districtno = selectedRow.Cells["districtno"].Value?.ToString();

            selectedBillingData = new string[]
            {
            accountNo,
            fullname,
            address,
            concessionCode,
            zoneCode,
            serviceCode,
            serviceType,
            meterNo,
            dueExempt,
            withholdingTax,
            wtPercent,
            scPercent,
            routeNo,
            taxExempted,
            discounted,
            billCode,
            districtno,
            balance
            };


            discountedLabel.Text = scPercent + '%';

            // Tax Exempt
            if (taxExempted == "0")
            {
                taxExemptedLabel.Visible = true;
                taxExemptedLabel.Text = "2%";
            }
            else
            {
                taxExemptedLabel.Visible = false;
            }



            // 🟦 Update UI fields
            accountNumberTextBox.Text = accountNo;

            fullnameTextBox.Text = fullname;
            addressTextBox.Text = address;
            accountnoBillHistory.Text = $"Account ID: {accountNo}";

            // 🟦 Get the latest bill_id for this account
            string latestBillID = GetLatestBillIDHelper.GetLatestBillId(accountNo);
            Debug.WriteLine(!string.IsNullOrEmpty(latestBillID)
                ? $"✅ Latest bill_id: {latestBillID}"
                : $"⚠️ No bill found for account number: {accountNo}");

            // 🟦 Load reading info if available
            var readingInfo = RecentBillDetailsHelper.GetReadingInfoByBillId(latestBillID);
            if (readingInfo != null)
            {
                // to reading date of the recent bill is the from reading date of the next bill which is today
                fromReadingDateLabel.Text = readingInfo.ToReadingDate.ToString("MMMM dd, yyyy");

                // this previous reading is the present reading of the last bill
                previousReadingTextBox.Text = readingInfo.PreviousReading.ToString();
                arrearsLabel.Text = balance;








                Debug.WriteLine($"Previous Reading: {readingInfo.PreviousReading}");
                Debug.WriteLine($"Reading Date: {readingInfo.FromReadingDate.ToShortDateString()}");
            }
            else
            {
                Debug.WriteLine($"⚠️ No data found for bill_id: {latestBillID}");
            }

            // 🟦 Load billing history and generate invoice code
            if (!string.IsNullOrWhiteSpace(accountNo))
            {
                LoadAccountBillHistory(accountNo);

                string zonePrefix = GetZonePrefixFromAccountNo(accountNo);
                string nextBillCode = GenerateNextBillCode(zonePrefix, DateTime.Now);
                string[] parts = nextBillCode.Split('-');
                if (parts.Length == 2)
                {
                    invoiceLabel.Text = parts[1];
                }
            }

        }








        private double ParseCurrency(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            string cleaned = text.Replace("₱", "").Replace(",", "").Trim();
            return double.TryParse(cleaned, out double result) ? result : 0;
        }



        private void LoadAccountBillHistory(string accountNo)
        {
            // Call helper to load billing summary rows where accountno = accountNo
            DataTable billData = ExclusiveDGVHelper.LoadRowsByExactAccount("tb_bill", "accountno", accountNo);

            if (billData != null)
            {
                billDataGridView.DataSource = billData;
                billDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                billDataGridView.Sort(billDataGridView.Columns["bill_id"], ListSortDirection.Descending);

            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string keyword = searchAccountNumberTextBox.Text.Trim().Replace("'", "''"); // prevent errors with single quotes

            if (accountDataGridView.DataSource is DataTable dt)
            {
                // Filter on both 'accountno' and 'fullname' columns
                dt.DefaultView.RowFilter = $"accountno LIKE '%{keyword}%' OR fullname LIKE '%{keyword}%'";
            }
        }





        private void clearButton_Click(object sender, EventArgs e)
        {
            if (accountDataGridView.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = ""; // reset filter
            }

            searchAccountNumberTextBox.Clear();
        }
        private void accountNumberTextBox_TextChanged(object sender, EventArgs e)
        {


        }
        private string GenerateNextBillCode(string zonePrefix, DateTime billingDate)
        {
            string formattedBillCode = string.Empty;
            int nextBillNumber = 1; // Default if no existing bill

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();

                string billingMonth = billingDate.ToString("yyyyMM");

                string query = @"
            SELECT MAX(CAST(SUBSTRING_INDEX(billcode, '-', -1) AS UNSIGNED)) 
            FROM tb_bill 
            WHERE zonecode = @zonePrefix 
            AND DATE_FORMAT(datebilled, '%Y%m') = @billingMonth";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@zonePrefix", zonePrefix);
                    cmd.Parameters.AddWithValue("@billingMonth", billingMonth);

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && int.TryParse(result.ToString(), out int lastNumber))
                    {
                        nextBillNumber = lastNumber + 1;
                    }
                }
            }

            // Set the extracted number
            extractedBillNumberLabel.Text = nextBillNumber.ToString();

            // Format: 003-0000014 (last part is always 7 digits)
            formattedBillCode = $"{zonePrefix}-{nextBillNumber.ToString("D7")}";
            invoiceLabel.Text = nextBillNumber.ToString();
            billCodeLabel.Text = formattedBillCode;
            return formattedBillCode;
        }



        private void amountPaidTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(textBox.Text))
                return;

            // Save cursor position
            int cursorPosition = textBox.SelectionStart;

            // Remove commas first
            string rawText = textBox.Text.Replace(",", "");

            // Try parse
            if (decimal.TryParse(rawText, out decimal value))
            {
                // Format using helper
                string formattedText = NumberFormatterHelper.FormatWithCommas(value);

                // Update only if changed to avoid flicker
                if (textBox.Text != formattedText)
                {
                    textBox.Text = formattedText;

                    // Set cursor at end (you can improve to restore exact position if needed)
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
            else
            {
                // Invalid input, clear or handle as needed
                textBox.Text = "";
            }
        }
        private void CalculateWaterCharges(int totalConsumption)
        {
            // Define the bracket rates and label prefixes using English words
            var brackets = new[]
            {
        new { Limit = 10, Label = "ten", Price = 35.20m },
        new { Limit = 10, Label = "twenty", Price = 37.71m },
        new { Limit = 10, Label = "thirty", Price = 39.15m },
        new { Limit = 10, Label = "forty", Price = 41.15m },
        new { Limit = int.MaxValue, Label = "fortyUp", Price = 43.15m }
    };

            int remaining = totalConsumption;
            decimal totalAmount = 0;
            int totalQty = 0;

            // Calculate bracket charges
            foreach (var b in brackets)
            {
                if (remaining <= 0)
                    break;

                int qty = Math.Min(b.Limit, remaining);
                decimal amount = qty * b.Price;

                switch (b.Label)
                {
                    case "ten":
                        tenQuantityLabel.Text = qty.ToString();
                        tenUnitPriceLabel.Text = b.Price.ToString("N2");
                        tenAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "twenty":
                        twentyQuantityLabel.Text = qty.ToString();
                        twentyUnitPriceLabel.Text = b.Price.ToString("N2");
                        twentyAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "thirty":
                        thirtyQuantityLabel.Text = qty.ToString();
                        thirtyUnitPriceLabel.Text = b.Price.ToString("N2");
                        thirtyAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "forty":
                        fortyQuantityLabel.Text = qty.ToString();
                        fortyUnitPriceLabel.Text = b.Price.ToString("N2");
                        fortyAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "fortyUp":
                        fortyUpQuantityLabel.Text = qty.ToString();
                        fortyUpUnitPriceLabel.Text = b.Price.ToString("N2");
                        fortyUpAmountLabel.Text = amount.ToString("N2");
                        break;
                }

                remaining -= qty;
                totalQty += qty;
                totalAmount += amount;
            }

            // Show total consumption and base amount
            totalQuantityLabel.Text = totalQty.ToString();
            totalAmountLabel.Text = totalAmount.ToString("N2");

            decimal scDiscounted = 0;
            decimal taxAdded = 0;
            decimal arrears = 0;

            // Remove "%" symbol and extra spaces
            string discountText = discountedLabel.Text.Replace("%", "").Trim();
            string taxAddedText = taxExemptedLabel.Text.Replace("%", "").Trim();

            // Try to parse the discount value
            if (decimal.TryParse(discountText, out decimal percent1))
            {
                scDiscounted = totalAmount * (percent1 / 100);
                discountedAmountLabel.Text = scDiscounted.ToString("0.00");
            }
            else
            {
                discountedAmountLabel.Text = "0.00";
            }

            // Try to parse the tax/exemption value
            if (decimal.TryParse(taxAddedText, out decimal percent2))
            {
                taxAdded = totalAmount * (percent2 / 100);
                exemptedAmountLabel.Text = taxAdded.ToString("0.00");
            }
            else
            {
                exemptedAmountLabel.Text = "0.00";
            }

            // Parse arrears from label text
            if (decimal.TryParse(arrearsLabel.Text.Trim(), out decimal parsedArrears))
            {
                arrears = parsedArrears;
            }
            else
            {
                arrears = 0; // default to 0 if parsing fails
            }

            // Calculate final charge
            decimal chargeSubTotal = (totalAmount - scDiscounted + taxAdded + arrears);

            // Display formatted value
            chargeLabel.Text = chargeSubTotal.ToString("0.00");

        }



        private void meterConsumedReadingTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClearWaterChargeLabels()
        {
            // Clear all tier labels if input is invalid
            tenQuantityLabel.Text = tenUnitPriceLabel.Text = tenAmountLabel.Text = "";
            twentyQuantityLabel.Text = twentyUnitPriceLabel.Text = twentyAmountLabel.Text = "";
            thirtyQuantityLabel.Text = thirtyUnitPriceLabel.Text = thirtyAmountLabel.Text = "";
            fortyQuantityLabel.Text = fortyUnitPriceLabel.Text = fortyAmountLabel.Text = "";
            fortyUpQuantityLabel.Text = fortyUpUnitPriceLabel.Text = fortyUpAmountLabel.Text = "";
            presentReadingTextBox.Text = "";
            discountedAmountLabel.Text = "";
            exemptedAmountLabel.Text = "";
            chargeLabel.Text = "";

        }
        private void ClearBillingLabels()
        {
            // Clear discount and tax labels
            discountedLabel.Text = "0";
            discountedAmountLabel.Text = "0";
            taxExemptedLabel.Text = "0";
            exemptedAmountLabel.Text = "0.00";
            chargeLabel.Text = "0.00";

            // Also clear totals
            totalQuantityLabel.Text = "0";
            totalAmountLabel.Text = "0.00";
        }


        private async void syncButton_Click(object sender, EventArgs e)
        {
            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm()) // make sure you created LoadingForm
            {
                await DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_concessionaire_detail", loadingForm);
            }
        }
        private void printOnlyButton_Click(object sender, EventArgs e)
        {
            billingPrintDocument.DefaultPageSettings.Landscape = false;

            //  Set small margins (optional: adjust for borderless or compact layout)
            billingPrintDocument.DefaultPageSettings.Margins = new Margins(3, 3, 3, 3);

            // Set paper size to Legal (8.5 x 14 inches = 850 x 1400 hundredths of an inch)
            PaperSize legalSize = new PaperSize("Legal", 850, 1400);
            billingPrintDocument.DefaultPageSettings.PaperSize = legalSize;

            // Show Print Dialog
            if (billingPrintDialog.ShowDialog() == DialogResult.OK)
            {
                // Proceed with printing
                billingPrintDocument.Print();

                MessageBox.Show("Print Billing record Print successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void arrearsLabel_Click(object sender, EventArgs e)
        {

        }

        private void discountedAmountLabel_Click(object sender, EventArgs e)
        {

        }

        private void exemptedAmountLabel_Click(object sender, EventArgs e)
        {

        }

        private void chargeLabel_Click(object sender, EventArgs e)
        {

        }

        private void presentReadingTextBox_TextChanged(object sender, EventArgs e)
        {
            string input = presentReadingTextBox.Text.Trim();

            // 🟡 Check if empty or zero
            if (string.IsNullOrEmpty(input) || input == "0")
            {
                printSaveButton.Enabled = false;
            }
            else
            {
                printSaveButton.Enabled = true;
            }
            // Try to parse the entered value as an integer
            // Try to parse the entered present reading
            if (int.TryParse(presentReadingTextBox.Text.Trim(), out int presentReading))
            {
                // Try to parse the previous reading
                if (int.TryParse(previousReadingTextBox.Text.Trim(), out int previousReading))
                {
                    // Calculate the consumption (present - previous)
                    int meterConsumed = presentReading - previousReading;

                    // Display the calculated consumption
                    meterConsumedReadingTextBox.Text = meterConsumed.ToString();

                    // Now calculate the water charges based on consumption
                    CalculateWaterCharges(meterConsumed);
                }
                else
                {
                    MessageBox.Show("Invalid previous reading input.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                ClearWaterChargeLabels();

            }


        }
    }
}
