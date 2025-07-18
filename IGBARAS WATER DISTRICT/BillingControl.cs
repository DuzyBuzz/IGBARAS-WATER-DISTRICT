using IGBARAS_WATER_DISTRICT.Helpers;
using Microsoft.VisualBasic.Devices;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Sec;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class BillingControl : UserControl
    {

        public BillingControl()
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
            // Set print document to portrait
            billingPrintDocument.DefaultPageSettings.Landscape = false;

            // Set all margins to 3 (unit: hundredths of an inch)
            billingPrintDocument.DefaultPageSettings.Margins = new Margins(3, 3, 3, 3);

            // Show print dialog
            if (billingPrintDialog.ShowDialog() == DialogResult.OK)
            {
                billingPrintDocument.Print();



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
        /// end of the print save button click event.
        /// </summary>





        private void SetDateNow()
        {
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
            userIdLabel.Text = $"{UserCredentials.UserId}";
            PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "🔎 Fullname or Account Number.");
            ClearButtonDisable();
            SetDateNow();

            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm()) // make sure you created LoadingForm
            {
                await DGVHelper.LoadDataToGridAsync(accountDataGridView, "concessionaire_detail", loadingForm);
            }

            // 🟢 Optional: Setup autocomplete after data loaded
            AutoCompleteHelper.FillTextBoxWithColumns("concessionaire_detail", new string[] { "accountno", "fullname" }, searchAccountNumberTextBox);
        }





        private void accountDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return; // Ignore header or invalid rows

            // 🟦 Get selected row
            DataGridViewRow selectedRow = accountDataGridView.Rows[e.RowIndex];

            // 🟦 Extract account details
            string accountNo = selectedRow.Cells["accountno"].Value?.ToString();
            string fullname = selectedRow.Cells["fullname"].Value?.ToString();
            string address = selectedRow.Cells["address"].Value?.ToString();
            int dueexempt = int.Parse(selectedRow.Cells["dueexempt"].Value?.ToString() ?? "0");

            // Get values from DataGridView
            string discounted = selectedRow.Cells["seniorcitizen"].Value?.ToString();
            string taxExempted = selectedRow.Cells["taxexempt"].Value?.ToString();

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
                // the to reading date is the same as the date billed this is all exacty the same date
                fromReadingDateLabel.Text = readingInfo.FromReadingDate.ToString("MMMM dd, yyyy");
                toReadingDateLabel.Text = readingInfo.ToReadingDate.ToString("MMMM dd, yyyy");
                dueDateLabel.Text = readingInfo.DueDate.ToString("MMMM dd, yyyy");
                previousReadingTextBox.Text = readingInfo.PreviousReading.ToString();
                meterConsumedReadingTextBox.Text = readingInfo.MeterConsumed.ToString();
                arrearsLabel.Text = readingInfo.Arrears.ToString();
                int meterConsumedReading = 0;

                // ➕ Calculate present reading
                int previousReading = 0;
                int.TryParse(readingInfo.PreviousReading.ToString(), out previousReading);
                int.TryParse(readingInfo.MeterConsumed.ToString(), out meterConsumedReading);
                int previousPenalty = 0;
                int.TryParse(readingInfo.Penalty.ToString(), out previousPenalty);
                preniousPenaltyTextBox.Text = previousPenalty.ToString();

                int presentReading = previousReading + meterConsumedReading;
                presentReadingTextBox.Text = presentReading.ToString();

                Debug.WriteLine($"Previous Reading: {readingInfo.PreviousReading}");
                Debug.WriteLine($"Reading Date: {readingInfo.ToReadingDate.ToShortDateString()}");
                GetPenalty(accountNo, dueexempt);

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


        /// <summary>
        /// JUST LIKE THE FUNCTION IN MYSQL THIS IS MY GET PENALTY
        /// </summary>
        /// <param name="accountno"></param>
        /// <param name="dueexempt"></param>
        private void GetPenalty(string accountno, int dueexempt)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // 🔹 1. Parse bill charge
                    if (!decimal.TryParse(chargeLabel.Text.Trim(), out decimal billCharge))
                    {
                        MessageBox.Show("Invalid bill charge value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 🔹 2. Parse due date
                    if (!DateTime.TryParseExact(dueDateLabel.Text.Trim(), "MMMM d, yyyy",
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.None,
                                                out DateTime dueDate))
                    {
                        MessageBox.Show("Invalid due date format. Use format like 'June 3, 2002'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 🔹 3. Arrears check
                    decimal.TryParse(arrearsLabel.Text.Trim(), out decimal arrearsAmount);
                    int arrears = arrearsAmount > 0 ? 1 : 0;

                    // 🔹 4. Previous penalty
                    decimal.TryParse(preniousPenaltyTextBox.Text.Trim(), out decimal previousPenalty);

                    // 🔹 5. Assume unpaid
                    int srcPaid = 0;

                    // 🔹 6. Early return if dueexempt or already paid
                    if (dueexempt == 1 || srcPaid == 1)
                    {
                        penaltyLabel.Text = "0%";
                        penaltyAmountLabel.Text = "0.00";
                        totalChargeBillLabel.Text = billCharge.ToString("N2");
                        return;
                    }

                    // 🔹 7. Get billsettings
                    string settingsQuery = "SELECT * FROM tb_billsettings WHERE districtno = (SELECT districtno FROM tb_concessionaire WHERE accountno = @accountno LIMIT 1) LIMIT 1";

                    MySqlCommand settingsCmd = new MySqlCommand(settingsQuery, conn);
                    settingsCmd.Parameters.AddWithValue("@accountno", accountno);

                    using (MySqlDataReader reader = settingsCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // 🔹 Read penalty settings
                            int gracePeriod = Convert.ToInt32(reader["graceperiod"]);
                            int firstDays = Convert.ToInt32(reader["firstnumdays"]);
                            int secondDays = Convert.ToInt32(reader["secondnumdays"]);
                            int thirdDays = Convert.ToInt32(reader["thirdnumdays"]);

                            int penalty10 = Convert.ToInt32(reader["penalty10"]);
                            int penalty15 = Convert.ToInt32(reader["penalty15"]);
                            int penalty20 = Convert.ToInt32(reader["penalty20"]);

                            int penalty10yn = Convert.ToInt32(reader["penalty10yn"]);
                            int penalty15yn = Convert.ToInt32(reader["penalty15yn"]);
                            int penalty20yn = Convert.ToInt32(reader["penalty20yn"]);

                            int defaultPenalty = Convert.ToInt32(reader["defaultpenalty"]);

                            // 🔹 Calculate date difference
                            DateTime now = DateTime.Now.Date;
                            DateTime effectiveDueDate = dueDate.AddDays(gracePeriod);
                            int overdueDays = (now - effectiveDueDate).Days;

                            decimal penaltyPercent = 0;

                            if (overdueDays <= 0)
                            {
                                penaltyPercent = 0;
                            }
                            else if (overdueDays <= firstDays && penalty10yn == 1)
                            {
                                penaltyPercent = penalty10;
                            }
                            else if (overdueDays <= secondDays && penalty15yn == 1)
                            {
                                penaltyPercent = penalty15;
                            }
                            else if (overdueDays >= thirdDays && penalty20yn == 1)
                            {
                                penaltyPercent = penalty20;
                            }
                            else
                            {
                                penaltyPercent = defaultPenalty;
                            }

                            // 🔹 Calculate penalty amount
                            decimal penaltyAmount = (penaltyPercent / 100m) * billCharge;
                            penaltyAmount = Math.Round(penaltyAmount, 2);

                            // 🔹 Update UI
                            penaltyLabel.Text = $"{Math.Round(penaltyPercent)}%";
                            penaltyAmountLabel.Text = penaltyAmount.ToString("N2");
                            totalChargeBillLabel.Text = (billCharge + penaltyAmount).ToString("N2");

                            // 🔹 Debug
                            Debug.WriteLine($"[Penalty] Arrears: {arrears} | Due Date: {dueDate.ToShortDateString()} | Overdue Days: {overdueDays}");
                            Debug.WriteLine($"[Penalty] Amount: {penaltyAmount:N2} | Percent: {penaltyPercent}%");
                        }
                        else
                        {
                            MessageBox.Show("No billing settings found for this account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating penalty:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        tenUnitPriceLabel.Text = b.Price.ToString("0.00");
                        tenAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "twenty":
                        twentyQuantityLabel.Text = qty.ToString();
                        twentyUnitPriceLabel.Text = b.Price.ToString("0.00");
                        twentyAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "thirty":
                        thirtyQuantityLabel.Text = qty.ToString();
                        thirtyUnitPriceLabel.Text = b.Price.ToString("0.00");
                        thirtyAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "forty":
                        fortyQuantityLabel.Text = qty.ToString();
                        fortyUnitPriceLabel.Text = b.Price.ToString("0.00");
                        fortyAmountLabel.Text = amount.ToString("N2");
                        break;
                    case "fortyUp":
                        fortyUpQuantityLabel.Text = qty.ToString();
                        fortyUpUnitPriceLabel.Text = b.Price.ToString("0.00");
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
            penaltyAmountLabel.Text = "";
            penaltyLabel.Text = "";
            // Clear discount and tax labels
            discountedLabel.Text = "";
            discountedAmountLabel.Text = "";
            taxExemptedLabel.Text = "";
            exemptedAmountLabel.Text = "";
            arrearsLabel.Text = "";
            chargeLabel.Text = "";
            // Also clear totals
            totalQuantityLabel.Text = "";
            totalAmountLabel.Text = "";
        }

        private async void syncButton_Click(object sender, EventArgs e)
        {
            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm()) // make sure you created LoadingForm
            {
                await DGVHelper.LoadDataToGridAsync(accountDataGridView, "concessionaire_detail", loadingForm);
            }
        }

        private void accountDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void accountDataGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
