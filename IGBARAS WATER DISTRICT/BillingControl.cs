using IGBARAS_WATER_DISTRICT.Helpers;
using Microsoft.VisualBasic.Devices;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Utilities;
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
            // 🔒 Check if bill is already paid
            if (CheckIfBillIsPaid())
            {
                MessageBox.Show("❌ This bill has already been paid. Saving or printing is not allowed.", "Bill Already Paid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 💾 Save billing record to database
                UpdateBillingRecord();

                MessageBox.Show("✅ Billing record saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 🖨️ Set printer settings to landscape and legal paper
                billingPrintDocument.DefaultPageSettings.Landscape = true;

                foreach (PaperSize ps in billingPrintDocument.PrinterSettings.PaperSizes)
                {
                    if (ps.Kind == PaperKind.Legal)
                    {
                        billingPrintDocument.DefaultPageSettings.PaperSize = ps;
                        break;
                    }
                }

                // Optional: Set margins (in hundredths of an inch, 30 = 0.3")
                billingPrintDocument.DefaultPageSettings.Margins = new Margins(30, 30, 30, 30);

                // 🖨️ Print the billing document
                billingPrintDocument.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ An error occurred while saving or printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// end of the print save button click event.
        /// </summary>


        /// <summary>
        /// Checks if the specified bill ID has already been marked as paid in the tb_bill table.
        /// </summary>
        /// <param name="billId">The bill ID to check.</param>
        /// <returns>True if paid (paid = 1), otherwise false.</returns>
        private bool CheckIfBillIsPaid()
        {
            bool isPaid = false;

            try
            {
                using (MySqlConnection con = new MySqlConnection(DbConfig.ConnectionString))
                {
                    con.Open();
                    Debug.WriteLine("✅ Database connection opened.");

                    string query = "SELECT paid, datebilled FROM tb_bill WHERE bill_id = @billId";
                    Debug.WriteLine($"🟡 Executing query: {query}");

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        string billId = billIdTextBox.Text.Trim();
                        cmd.Parameters.AddWithValue("@billId", billId);
                        Debug.WriteLine($"🔍 Using bill ID: {billId}");

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int paidValue = 0;
                                DateTime dateBilled = DateTime.MinValue;

                                // ✅ Handle nullable 'paid'
                                if (!reader.IsDBNull(reader.GetOrdinal("paid")))
                                {
                                    paidValue = reader.GetInt32("paid");
                                }

                                // ✅ Handle nullable 'datebilled'
                                if (!reader.IsDBNull(reader.GetOrdinal("datebilled")))
                                {
                                    dateBilled = reader.GetDateTime("datebilled");
                                }

                                Debug.WriteLine($"📄 Retrieved: paid = {paidValue}, dateBilled = {(dateBilled == DateTime.MinValue ? "NULL" : dateBilled.ToString("yyyy-MM-dd"))}");

                                int currentYear = DateTime.Now.Year;
                                int currentMonth = DateTime.Now.Month;
                                Debug.WriteLine($"📅 Current Date: {DateTime.Now:yyyy-MM-dd}");

                                // ✅ Check paid == 1 and date within current month/year
                                if (paidValue == 1 &&
                                    dateBilled.Year == currentYear &&
                                    dateBilled.Month == currentMonth)
                                {
                                    isPaid = true;
                                    Debug.WriteLine("✅ Bill is PAID this month.");
                                }
                                else
                                {
                                    Debug.WriteLine("❌ Bill is NOT paid this month.");
                                }
                            }
                            else
                            {
                                Debug.WriteLine("⚠️ No bill found with the given bill ID.");
                            }
                        }
                    }

                    con.Close();
                    Debug.WriteLine("🔚 Database connection closed.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❗ Exception: " + ex.Message);
                MessageBox.Show("Error checking bill payment status: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isPaid;
        }




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

        private async void BillingControl_Load(object sender, EventArgs e)
        {
            ClearWaterChargeLabels();
            userIdLabel.Text = $"{UserCredentials.UserId}";
            PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "🔎 Fullname or Account Number.");
            ClearButtonDisable();
            SetDateNow();
            LoadBillSettingsAsSideways();
            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_concessionaire_detail", loadingForm);
                var task2 = DGVHelper.LoadDataToGridAsync(billSetingsDataGridView, "tb_billsettings", loadingForm);

                await Task.WhenAll(task1, task2);
            }

            // 🟢 Optional: Setup autocomplete after data loaded
            AutoCompleteHelper.FillTextBoxWithColumns("v_concessionaire_detail", new string[] { "accountno", "fullname" }, searchAccountNumberTextBox);
        }



        private void LoadBillSettingsAsSideways()
        {
            billSettingsListView.Clear(); // clear old data

            // Create 2 columns: Setting | Value
            billSettingsListView.Columns.Add("Bill Setting", 100, HorizontalAlignment.Left);
            billSettingsListView.Columns.Add("Value", 70, HorizontalAlignment.Left);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // 🔹 Get only 1 row (settings are usually single row)
                    string query = "SELECT * FROM tb_billsettings LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // 🔹 Loop through each column and display as a row
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                string columnValue = reader[i].ToString();

                                ListViewItem item = new ListViewItem(columnName); // left column
                                item.SubItems.Add(columnValue);                  // right column
                                billSettingsListView.Items.Add(item);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No settings found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message);
            }
        }


        private void accountDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ClearWaterChargeLabels();

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
            string balance = selectedRow.Cells["balancex"].Value?.ToString();

            arrearsLabel.Text = balance.ToString();
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
            billIdTextBox.Text = latestBillID;
            // 🟦 Load reading info if available
            var readingInfo = RecentBillDetailsHelper.GetReadingInfoByBillId(latestBillID);
            if (readingInfo != null)
            {

                // the to reading date is the same as the date billed this is all exacty the same date
                fromReadingDateLabel.Text = readingInfo.FromReadingDate.ToString("MMMM dd, yyyy");
                toReadingDateLabel.Text = readingInfo.ToReadingDate.ToString("MMMM dd, yyyy");
                dueDateLabel.Text = readingInfo.DueDate.ToString("MMMM dd, yyyy");
                previousReadingTextBox.Text = readingInfo.PreviousReadingBill.ToString();
                meterConsumedReadingTextBox.Text = readingInfo.MeterConsumed.ToString();
                arrearsLabel.Text = readingInfo.Arrears.ToString();
                int meterConsumedReading = 0;

                // ➕ Calculate present reading
                int previousReading = 0;
                int.TryParse(readingInfo.PreviousReadingBill.ToString(), out previousReading);
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
                            oberDueDaysLabel.Text = $"{overdueDays} days overdue";

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

            if (CheckIfBillIsPaid())
            {
                MessageBox.Show("This bill is already paid.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Exit or disable further actions

            }
            else
            {
                if (billData != null)
                {
                    billDataGridView.DataSource = billData;
                    billDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    billDataGridView.Sort(billDataGridView.Columns["bill_id"], ListSortDirection.Descending);

                }
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

        private void UpdateBillingRecord()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(DbConfig.ConnectionString))
                {
                    con.Open();

                    string query = @"UPDATE tb_bill SET
                charge = @charge,
                taxpercent = @taxpercent,
                taxamount = @taxamount,
                scpercent = @scpercent,
                senioramount = @senioramount,
                totalbillcharge = @totalbillcharge,
                billcharge = @billcharge,
                balance = @balance,
                paid = @paid,
                amountpaid = @amountpaid,
                penaltyamount = @penaltyamount,
                arrearsamount = @arrearsamount,
                datebilled = @datebilled,
                partiallypaid = @partiallypaid,
                adjustdebit = @adjustdebit,
                adjustcredit = @adjustcredit,
                uploaded = @uploaded
                WHERE bill_id = @bill_id";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        // Parse and calculate financial fields
                        double charge = double.TryParse(chargeLabel.Text, out double c) ? c : 0;
                        int taxPercent = int.TryParse(taxExemptedLabel.Text, out int tp) ? tp : 0;
                        double taxAmount = (charge * taxPercent) / 100;

                        int scPercent = int.TryParse(discountedLabel.Text, out int scp) ? scp : 0;
                        double seniorAmount = (charge * scPercent) / 100;

                        double totalBillCharge = double.TryParse(totalChargeBillLabel.Text, out double tbc) ? tbc : 0;
                        double amountPaid = double.TryParse(amountPaidTextBox.Text, out double ap) ? ap : 0;

                        double penaltyAmount = double.TryParse(penaltyAmountLabel.Text, out double pa) ? pa : 0;
                        double arrearsAmount = double.TryParse(arrearsLabel.Text, out double ar) ? ar : 0;

                        // 🧮 Calculate balance
                        double balance = totalBillCharge - amountPaid;
                        if (balance < 0) balance = 0; // Never negative

                        // ✅ Set payment status
                        int paid = (balance == 0 && amountPaid > 0) ? 1 : 0; // Fully paid if no balance and payment was made
                        int partiallyPaid = (amountPaid > 0 && balance > 0) ? 1 : 0; // Partially paid

                        // Add parameters to the command
                        cmd.Parameters.AddWithValue("@charge", charge);
                        cmd.Parameters.AddWithValue("@taxpercent", taxPercent);
                        cmd.Parameters.AddWithValue("@taxamount", taxAmount);
                        cmd.Parameters.AddWithValue("@scpercent", scPercent);
                        cmd.Parameters.AddWithValue("@senioramount", seniorAmount);
                        cmd.Parameters.AddWithValue("@totalbillcharge", totalBillCharge);
                        cmd.Parameters.AddWithValue("@billcharge", totalBillCharge); // for simplicity same as total for now
                        cmd.Parameters.AddWithValue("@balance", balance);
                        cmd.Parameters.AddWithValue("@paid", paid);
                        cmd.Parameters.AddWithValue("@amountpaid", amountPaid);
                        cmd.Parameters.AddWithValue("@penaltyamount", penaltyAmount);
                        cmd.Parameters.AddWithValue("@arrearsamount", arrearsAmount);
                        cmd.Parameters.AddWithValue("@datebilled", DateTime.Now);
                        cmd.Parameters.AddWithValue("@partiallypaid", partiallyPaid);
                        cmd.Parameters.AddWithValue("@adjustdebit", 0.00); // default value
                        cmd.Parameters.AddWithValue("@adjustcredit", 0.00); // default value
                        cmd.Parameters.AddWithValue("@uploaded", 0); // default value
                        cmd.Parameters.AddWithValue("@bill_id", billIdTextBox.Text);

                        // Execute update
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("✅ Billing record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("⚠️ No record was updated. Please check Bill ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error updating billing record:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string input = meterConsumedReadingTextBox.Text.Trim();


            // 🔵 Try parsing as integer
            if (int.TryParse(input, out int consumption))
            {
                CalculateWaterCharges(consumption);
            }
            else
            {
                ClearWaterChargeLabels(); // invalid input
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
            meterConsumedReadingTextBox.Text= "";
            // Clear discount and tax labels
            discountedLabel.Text = "";
            discountedAmountLabel.Text = "";
            taxExemptedLabel.Text = "";
            exemptedAmountLabel.Text = "";
            chargeLabel.Text = "0.00";
            // Also clear totals
            totalQuantityLabel.Text = "";
            totalAmountLabel.Text = "";
            string amount = amountPaidTextBox.Text.Trim();
            if (string.IsNullOrEmpty(amount) || amount == "0")
            {
                billPaidButton.Enabled = false;
            }
            else
            {
                billPaidButton.Enabled = true;
            }
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

        private void accountDataGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
