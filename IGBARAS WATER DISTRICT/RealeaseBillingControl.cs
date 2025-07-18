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
            // Set page orientation
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

                // Validate if billing data is selected
                if (selectedBillingData == null || selectedBillingData.Length < 17)
                {
                    MessageBox.Show("No selected billing data. Please select an account first.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate and Save Billing
                if (CheckBillingDate())
                {
                    InsertToBillingTable(selectedBillingData);
                    MessageBox.Show("Billing record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Invalid billing date. Data was not saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }



        private Bitmap CapturePanel(Control panel)
        {
            // Create a bitmap with the size of the panel
            Bitmap bmp = new Bitmap(panel.Width, panel.Height);
            panel.DrawToBitmap(bmp, new Rectangle(0, 0, panel.Width, panel.Height));
            return bmp;
        }
        private void PrepareAndPrintDocument()
        {
            // Set up print document
            billingPrintDocument.DefaultPageSettings.Landscape = false;

            // Set paper size to Legal (8.5 x 14 inches)
            PaperSize legalSize = new PaperSize("Legal", 850, 1400); // width and height in hundredths of an inch
            billingPrintDocument.DefaultPageSettings.PaperSize = legalSize;

            // Optional: Set margins
            billingPrintDocument.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50); // 0.5 inch margin

            // Trigger the printing
            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = billingPrintDocument;
            preview.ShowDialog(); // or use billingPrintDocument.Print(); for direct print
        }

        private void billingPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Names for each copy
            string[] copyNames = { "Concessionaire's Copy", "Records Copy", "File Copy" };

            int copiesPerPage = copyNames.Length;
            int availableHeight = e.MarginBounds.Height;
            int availableWidth = e.MarginBounds.Width;

            // Set spacing between copies (in pixels)
            int spacing = 10;
            int totalSpacing = spacing * (copiesPerPage - 1);
            int copyHeight = (availableHeight - totalSpacing) / copiesPerPage;

            int targetWidth = availableWidth;
            int targetHeight = copyHeight;

            for (int i = 0; i < copiesPerPage; i++)
            {
                // Set the label for the copy type
                copyTypeLabel.Text = copyNames[i];

                // Capture the panel as bitmap
                Bitmap panelImage = CapturePanel(billingPanel);

                // Calculate scale to fit width and height
                float scale = Math.Min(
                    (float)targetWidth / panelImage.Width,
                    (float)targetHeight / panelImage.Height);

                int printWidth = (int)(panelImage.Width * scale);
                int printHeight = (int)(panelImage.Height * scale);

                int x = e.MarginBounds.Left + (targetWidth - printWidth) / 2;
                int y = e.MarginBounds.Top + i * (copyHeight + spacing) + (copyHeight - printHeight) / 2;

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                e.Graphics.DrawImage(panelImage, new Rectangle(x, y, printWidth, printHeight));

                panelImage.Dispose();

                // Draw cut indication line after every copy
                int lineY = e.MarginBounds.Top + (i + 1) * copyHeight + i * spacing + spacing / 2;
                using (Font cutFont = new Font("Arial", 6, FontStyle.Bold))
                {
                    // Measure the width of "✄"
                    float scissorsWidth = e.Graphics.MeasureString("✄", cutFont).Width;
                    // Measure the width of one "┈"
                    float dashWidth = e.Graphics.MeasureString("┈", cutFont).Width;
                    // Calculate total dashes to fill the width minus scissors
                    int totalDashes = (int)Math.Floor((e.MarginBounds.Width - scissorsWidth) / dashWidth);
                    int leftDashes = totalDashes / 2;
                    int rightDashes = totalDashes - leftDashes;
                    string cutText = new string('┈', leftDashes) + "✄┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈✄" + new string('┈', rightDashes);

                    using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        RectangleF textRect = new RectangleF(e.MarginBounds.Left, lineY - 8, e.MarginBounds.Width, 16);
                        e.Graphics.DrawString(cutText, cutFont, Brushes.Black, textRect, sf);
                    }
                }
            }
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
          AND MONTH(datebilled) = @month 
          AND YEAR(datebilled) = @year";

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
        public string GenerateNextBillCode(string zoneCode, DateTime billMonth)
        {
            //the zoneCode is expected to be in the format "001", "002", etc.
            int zoneNo = GetZoneNoFromDB(zoneCode);
            int baseStart = ((zoneNo - 1) * 200) + 1;
            int monthOffset = (billMonth.Month - 1) * 400;
            int startingNumber = baseStart + monthOffset;

            // Extract zone prefix (e.g. "001", "002", etc.)
            string prefix = zoneCode;


            // Query latest billcode for that zone and month
            string query = @"
        SELECT MAX(CAST(SUBSTRING_INDEX(billcode, '-', -1) AS UNSIGNED))
        FROM tb_bill
        WHERE LEFT(billcode, 3) = @prefix
          AND MONTH(datebilled) = @month
          AND YEAR(datebilled) = @year";

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@prefix", prefix);
                    cmd.Parameters.AddWithValue("@month", billMonth.Month);
                    cmd.Parameters.AddWithValue("@year", billMonth.Year);

                    object result = cmd.ExecuteScalar();
                    int latest = (result != DBNull.Value && result != null)
                                ? Convert.ToInt32(result)
                                : startingNumber - 1;

                    int next = latest + 1;
                    return $"{prefix}-{next.ToString("D7")}";
                }
            }
        }

        private async void BillingControl_Load(object sender, EventArgs e)
        {
            ClearWaterChargeLabels();
            PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "🔎 Fullname or Account Number.");
            ClearButtonDisable();
            SetDateNow();

            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm()) // make sure you created LoadingForm
            {
                await DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_concessionaire_detail", loadingForm);
            }

            // 🟢 Optional: Setup autocomplete after data loaded
            AutoCompleteHelper.FillTextBoxWithColumns("v_concessionaire_detail", new string[] { "accountno", "fullname" }, searchAccountNumberTextBox);
        }





        private void accountDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ClearWaterChargeLabels();

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
            districtno
            };


            // Senior Citizen
            if (discounted == "1")
            {
                discountedLabel.Text = "7%";
            }
            else
            {
                discountedLabel.Visible = false;
            }

            // Tax Exempt
            if (taxExempted == "1")
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
                arrearsLabel.Text = readingInfo.Arrears.ToString();







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

        private void InsertToBillingTable(string[] data)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO tb_bill (
                    accountno, name, address, concessionairecode, zonecode, servicecode, servicetype, 
                    meterno, dueexempt, withholdingtax, wtpercent, scpercent, routeno, 
                    taxexempt, seniorcitizen, billcode, districtno, fromreadingdate, toreadingdate, duedate, 
                    previousreading, presentreading, meterconsumed, charge, taxpercent, taxamount, 
                    senioramount, month, year, datebilled, arrearsamount, presentmeterconsumed, 
                    adjustdebit, adjustcredit, partiallypaid, othermeterconsumed
                ) 
                VALUES (
                    @accountno, @fullname, @address, @concessionairecode, @zonecode, @servicecode, @servicetype, 
                    @meterno, @dueexempt, @withholdingtax, @wtpercent, @scpercent, @routeno, 
                    @taxexempt, @seniorcitizen, @billcode, @districtno, @fromreadingdate, @toreadingdate, @duedate, 
                    @previousreading, @presentreading, @meterconsumed, @charge, @taxpercent, @taxamount, 
                    @senioramount, @month, @year, @datebilled, @arrearsamount, @presentmeterconsumed, 
                    @adjustdebit, @adjustcredit, @partiallypaid, @othermeterconsumed
                )";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Clean percent signs (e.g., "5%") before converting
                        string taxPercentStr = taxExemptedLabel.Text.Trim().Replace("%", "");
                        string discountPercentStr = discountedAmountLabel.Text.Trim().Replace("%", "");

                        // Format dates to yyyy-MM-dd
                        // Parse the date strings from the labels and format them as yyyy-MM-dd
                        string formattedFromDate = DateTime.Parse(fromReadingDateLabel.Text).ToString("yyyy-MM-dd");
                        string formattedToDate = DateTime.Parse(toReadingDateLabel.Text).ToString("yyyy-MM-dd");
                        string formattedDueDate = DateTime.Parse(dueDateLabel.Text).ToString("yyyy-MM-dd");
                        string formattedDateBilled = DateTime.Now.ToString("yyyy-MM-dd");

                        // Add parameters from string[] data
                        cmd.Parameters.AddWithValue("@accountno", data[0]);
                        cmd.Parameters.AddWithValue("@fullname", data[1]);
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
                        cmd.Parameters.AddWithValue("@billcode", data[4] + "-" + data[15]);
                        cmd.Parameters.AddWithValue("@districtno", Convert.ToInt32(data[16]));

                        // Date values
                        cmd.Parameters.AddWithValue("@fromreadingdate", formattedFromDate);
                        cmd.Parameters.AddWithValue("@toreadingdate", formattedToDate);
                        cmd.Parameters.AddWithValue("@duedate", formattedDueDate);
                        cmd.Parameters.AddWithValue("@datebilled", formattedDateBilled);

                        // Reading and billing
                        cmd.Parameters.AddWithValue("@previousreading", Convert.ToInt32(previousReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@presentreading", Convert.ToInt32(presentReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@meterconsumed", Convert.ToDouble(meterConsumedReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@charge", Convert.ToDouble(chargeLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@taxpercent", Convert.ToInt32(taxPercentStr));
                        cmd.Parameters.AddWithValue("@taxamount", Convert.ToDouble(exemptedAmountLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@senioramount", Convert.ToDouble(discountedAmountLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@month", DateTime.Now.Month);
                        cmd.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        cmd.Parameters.AddWithValue("@arrearsamount", Convert.ToDecimal(arrearsLabel.Text.Trim()));

                        // Columns that have no default value — set them manually
                        cmd.Parameters.AddWithValue("@presentmeterconsumed", Convert.ToInt64(meterConsumedReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@partiallypaid", 0);
                        cmd.Parameters.AddWithValue("@othermeterconsumed", 0);

                        // Adjust columns defaulted to 0.00
                        cmd.Parameters.AddWithValue("@adjustdebit", 0.00m);
                        cmd.Parameters.AddWithValue("@adjustcredit", 0.00m);

                        // Execute insert
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
            CheckBillingDate();
            string accountNo = accountNumberTextBox.Text.Trim();
            string zonePrefix = GetZonePrefixFromAccountNo(accountNo);
            string nextBillCode = GenerateNextBillCode(zonePrefix, DateTime.Now);
            string[] parts = nextBillCode.Split('-');
            if (parts.Length == 2)
            {
                invoiceLabel.Text = parts[1];
            }

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

            // -------------------------------
            // Apply both discounts if any
            // -------------------------------
            decimal discountAmount1 = 0;
            decimal discountAmount2 = 0;

            // Discount from discountedLabel (e.g., 7%)
            string discountText = discountedLabel.Text.Replace("%", "").Trim();
            if (decimal.TryParse(discountText, out decimal percent1))
            {
                discountAmount1 = totalAmount * (percent1 / 100);
            }

            // Discount from taxExemptedLabel (e.g., 5%)
            string taxExemptText = taxExemptedLabel.Text.Replace("%", "").Trim();
            if (decimal.TryParse(taxExemptText, out decimal percent2))
            {
                discountAmount2 = totalAmount * (percent2 / 100);
            }

            // -------------------------------
            // Apply balance from balanceLabel
            // -------------------------------
            decimal previousBalance = 0;
            if (decimal.TryParse(arrearsLabel.Text, out decimal balance))
            {
                previousBalance = balance;
            }

            // -------------------------------
            // Compute Amount Due
            // -------------------------------
            decimal totalDiscount = discountAmount1 + discountAmount2;
            decimal amountDue = (totalAmount - totalDiscount) + previousBalance;

            // Show final payable amount
            chargeLabel.Text = amountDue.ToString("N2");
        }



        private void meterConsumedReadingTextBox_TextChanged(object sender, EventArgs e)
        {
            ClearWaterChargeLabels();
            // Try to parse the entered value as an integer
            if (int.TryParse(meterConsumedReadingTextBox.Text.Trim(), out int consumption))
            {
                // If parsing is successful, calculate water charges
                CalculateWaterCharges(consumption);
                int previousReading = 0;
                int.TryParse(previousReadingTextBox.Text.Trim(), out previousReading);
                // Calculate the present reading based on previous reading and consumption
                int presentReading = previousReading + consumption;
                presentReadingTextBox.Text = presentReading.ToString();

            }
            else
            {
                // If the input is not a valid number, clear labels to avoid wrong values
                ClearWaterChargeLabels();
            }
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

            // Clear discount and tax labels
            discountedLabel.Text = "0%";
            discountedAmountLabel.Text = "0.00";
            taxExemptedLabel.Text = "0%";
            exemptedAmountLabel.Text = "0.00";
            arrearsLabel.Text = "0.00";
            chargeLabel.Text = "0.00";
            // Also clear totals
            totalQuantityLabel.Text = "";
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

        private void accountDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
    }
}
