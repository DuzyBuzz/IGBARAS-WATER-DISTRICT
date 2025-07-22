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
                // ✅ Save billing data to database
                InsertToBillingTable(selectedBillingData);
                MessageBox.Show("Billing record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ✅ Setup print document settings
                billingPrintDocument.DefaultPageSettings.Landscape = false;
                billingPrintDocument.DefaultPageSettings.Margins = new Margins(3, 3, 3, 3);

                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.Document = billingPrintDocument;
                    printDialog.AllowSomePages = false;
                    printDialog.AllowSelection = false;

                    // ✅ Show printer selection dialog
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 🔒 Use selected printer settings
                        billingPrintDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);

                        // 🖨️ Print directly
                        billingPrintDocument.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ An error occurred while saving or printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // user get bill settings helper to get the due date days ine tb_billsettings table


        private Bitmap CapturePanel(Control panel)
        {
            // Create a bitmap with the size of the panel
            Bitmap bmp = new Bitmap(panel.Width, panel.Height);
            panel.DrawToBitmap(bmp, new Rectangle(0, 0, panel.Width, panel.Height));
            return bmp;
        }

        private void billingPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Copy labels
            string[] copyNames = { "Concessionaire's Copy", "Records Copy", "File Copy" };

            // Get print area
            int availableHeight = e.MarginBounds.Height;
            int availableWidth = e.MarginBounds.Width;

            // Divide space for 3 copies
            int copiesPerPage = 3;
            int copyHeight = availableHeight / copiesPerPage;

            for (int i = 0; i < copiesPerPage; i++)
            {
                // Set label text
                copyTypeLabel.Text = copyNames[i];

                // Capture the panel
                using (Bitmap panelImage = CapturePanel(billingPanel))
                {
                    // Compute scale to fit width and height per copy
                    float scale = Math.Min(
                        (float)availableWidth / panelImage.Width,
                        (float)copyHeight / panelImage.Height);

                    int printWidth = (int)(panelImage.Width * scale);
                    int printHeight = (int)(panelImage.Height * scale);

                    // Centering coordinates
                    int x = e.MarginBounds.Left + (availableWidth - printWidth) / 2;
                    int y = e.MarginBounds.Top + i * copyHeight + (copyHeight - printHeight) / 2;

                    // High-quality rendering settings
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    // Draw the image
                    e.Graphics.DrawImage(panelImage, new Rectangle(x, y, printWidth, printHeight));
                }
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




        private void DisableButton()
        {
            if (string.IsNullOrEmpty(subTotalAmountDueLabel.Text))
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
                        double charge = double.TryParse(subTotalAmountDueLabel.Text, out double c) ? c : 0;
                        int taxPercent = int.TryParse(taxExemptedPercentLabel.Text, out int tp) ? tp : 0;
                        double taxAmount = (charge * taxPercent) / 100;

                        int scPercent = int.TryParse(discountedPercentLabel.Text, out int scp) ? scp : 0;
                        double seniorAmount = (charge * scPercent) / 100;

                        double totalBillCharge = double.TryParse(totalAmountDueLabel.Text, out double tbc) ? tbc : 0;
                        double amountPaid = double.TryParse(amountPaidTextBox.Text, out double ap) ? ap : 0;

                        double penaltyAmount = double.TryParse(penaltyAmountLabel.Text, out double pa) ? pa : 0;
                        double arrearsAmount = double.TryParse(arrearsAmountLabel.Text, out double ar) ? ar : 0;

                        // 🧮 Calculate balance
                        double balance = totalBillCharge - amountPaid;
                        if (balance < 0) balance = 0; // Never negative

                        // ✅ Set payment status
                        int paid = (balance == 0 && amountPaid > 0) ? 1 : 0; // Fully paid if no balance and payment was made
                        int partiallyPaid = (amountPaid > 0 && balance > 0) ? 1 : 0; // Partially paid

                        int arrears = 0;

                        if (paid == 0 || partiallyPaid == 1)
                            arrears = 1;
                        else
                            arrears = 0;


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
                        cmd.Parameters.AddWithValue("@arrears", arrears);
                        cmd.Parameters.AddWithValue("@uploaded", 0); // default value
                        cmd.Parameters.AddWithValue("@bill_id", latestBillIdLabel.Text);

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

        private void SetDateNow()
        {
            dueDateLabel.Text = DateTime.Now.AddDays(14).ToString("MMMM dd, yyyy");
            dateBilledLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            toReadingDateLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            paymentDateLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
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
                        // String fields
                        cmd.Parameters.AddWithValue("@billcode", billCodeLabel.Text.Trim());
                        cmd.Parameters.AddWithValue("@accountno", data[0]);
                        cmd.Parameters.AddWithValue("@name", data[1]);
                        cmd.Parameters.AddWithValue("@address", data[2]);
                        cmd.Parameters.AddWithValue("@concessionairecode", data[3]);
                        cmd.Parameters.AddWithValue("@zonecode", data[4]);
                        cmd.Parameters.AddWithValue("@servicecode", data[5]);
                        cmd.Parameters.AddWithValue("@servicetype", data[6]);
                        cmd.Parameters.AddWithValue("@meterno", data[7]);

                        // Int fields
                        cmd.Parameters.AddWithValue("@billnumber", int.Parse(extractedBillNumberLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@routeno", int.Parse(data[12]));
                        cmd.Parameters.AddWithValue("@taxexempt", int.Parse(data[13]));
                        cmd.Parameters.AddWithValue("@dueexempt", int.Parse(data[8]));
                        cmd.Parameters.AddWithValue("@withholdingtax", int.Parse(data[9]));
                        cmd.Parameters.AddWithValue("@wtpercent", int.Parse(data[10]));
                        cmd.Parameters.AddWithValue("@seniorcitizen", int.Parse(data[14]));
                        cmd.Parameters.AddWithValue("@scpercent", int.Parse(data[11]));
                        cmd.Parameters.AddWithValue("@districtno", int.Parse(data[16]));
                        cmd.Parameters.AddWithValue("@taxpercent", int.Parse(taxExemptedPercentLabel.Text.Replace("%", "").Trim()));
                        cmd.Parameters.AddWithValue("@month", DateTime.Now.Month);
                        cmd.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        cmd.Parameters.AddWithValue("@paid", 0);
                        cmd.Parameters.AddWithValue("@firstbill", string.IsNullOrWhiteSpace(fromReadingDateLabel.Text) ? 1 : 0);
                        cmd.Parameters.AddWithValue("@arrears", int.Parse(isArrearsLabel.Text.Trim()));
                        cmd.Parameters.AddWithValue("@partiallypaid", 0);
                        cmd.Parameters.AddWithValue("@uploaded", 0);

                        // Double fields
                        cmd.Parameters.AddWithValue("@wtamount", 0.00d);
                        cmd.Parameters.AddWithValue("@meterconsumed", double.Parse(meterConsumedReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@charge", double.Parse(subTotalAmountDueLabel.Text.Replace(",", "").Trim()));
                        decimal.TryParse(TaxExemptedAmountLabel.Text.Replace("%", "").Trim(), out decimal taxAmount);
                        decimal.TryParse(discountedAmountLabel.Text.Replace("%", "").Trim(), out decimal discountAmount);
                        cmd.Parameters.AddWithValue("@taxamount", (double)taxAmount);
                        cmd.Parameters.AddWithValue("@senioramount", (double)discountAmount);
                        cmd.Parameters.AddWithValue("@totalbillcharge", double.Parse(subTotalAmountDueLabel.Text.Replace(",", "").Trim()));

                        // Decimal fields
                        cmd.Parameters.AddWithValue("@totaladditionalcharge", decimal.Parse(sfcInstallmentTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@arrearsamount", decimal.Parse(arrearsAmountLabel.Text.Replace(",", "").Trim()));
                        cmd.Parameters.AddWithValue("@billcharge", decimal.Parse(subTotalAmountDueLabel.Text.Replace(",", "").Trim()));
                        cmd.Parameters.AddWithValue("@balance", decimal.Parse(subTotalAmountDueLabel.Text.Replace(",", "").Trim()));
                        cmd.Parameters.AddWithValue("@penaltyamount", decimal.Parse(penaltyAmountLabel.Text.Replace(",", "").Trim()));
                        cmd.Parameters.AddWithValue("@amountpaid", 0.00m);
                        cmd.Parameters.AddWithValue("@adjustdebit", 0.00m);
                        cmd.Parameters.AddWithValue("@adjustcredit", 0.00m);
                        cmd.Parameters.AddWithValue("@electriccharge", 0.00m);

                        // Bigint fields
                        cmd.Parameters.AddWithValue("@othermeterconsumed", 0L);
                        cmd.Parameters.AddWithValue("@presentmeterconsumed", long.Parse(meterConsumedReadingTextBox.Text.Trim()));

                        // Date fields
                        string formattedToDate = DateTime.TryParse(toReadingDateLabel.Text, out var toDate)
                            ? toDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");
                        string formattedDueDate = DateTime.TryParse(dueDateLabel.Text, out var dueDate)
                            ? dueDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");
                        string formattedDateBilled = DateTime.Now.ToString("yyyy-MM-dd");

                        if (fromReadingDateLabel.Tag is DateTime fromReadingDate)
                            cmd.Parameters.AddWithValue("@fromreadingdate", fromReadingDate);
                        else
                            cmd.Parameters.AddWithValue("@fromreadingdate", DBNull.Value);

                        cmd.Parameters.AddWithValue("@toreadingdate", formattedToDate);
                        cmd.Parameters.AddWithValue("@duedate", formattedDueDate);
                        cmd.Parameters.AddWithValue("@duegraceperiod", formattedDueDate);
                        cmd.Parameters.AddWithValue("@datebilled", formattedDateBilled);
                        cmd.Parameters.AddWithValue("@disconnectiondate", DBNull.Value);

                        // Int fields for readings
                        cmd.Parameters.AddWithValue("@previousreading", int.Parse(previousReadingTextBox.Text.Trim()));
                        cmd.Parameters.AddWithValue("@presentreading", int.Parse(presentReadingTextBox.Text.Trim()));

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




        private async void BillingControl_Load(object sender, EventArgs e)
        {
            SetDateNow();
            LoadZoneComboBox();
            ClearWaterChargeLabels();
            LoadPayments();
            PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "🔎 Fullname or Account Number.");
            ClearButtonDisable();
            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_concessionaire_detail", loadingForm);

                await Task.WhenAll(task1);
            }

            // 🟢 Optional: Setup autocomplete after data loaded
            AutoCompleteHelper.FillTextBoxWithColumns("v_concessionaire_detail", new string[] { "accountno", "fullname" }, searchAccountNumberTextBox);
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



        private void accountDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ClearWaterChargeLabels();
            fromReadingDateLabel.Text = "";
            previousReadingTextBox.Text = "0";
            meterConsumedReadingTextBox.Text = "0";
            totalQuantityLabel.Text = "0.00";
            totalWaterConsumptionAmountLabel.Text = "0";
            totalAmountDueLabel.Text = "0.00";
            presentReadingTextBox.Text = "";
            previousReadingTextBox.Text = "0";
            isWithHoldingTaxLabel.Text = "0";
            isArrearsLabel.Text = "0";
            dueExemptLabel.Text = "0";

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
                balance,
            };
            dueExemptLabel.Text = dueExempt;
            // 🟦 Step 1: Make sure accountNo is not empty
            if (!string.IsNullOrWhiteSpace(accountNo))
            {
                // Step 2: Load billing history from database (based on accountNo)
                LoadAccountBillHistory(accountNo);

                // Step 3: Extract zone prefix from the account number (e.g., "01" → "001")
                string zonePrefix = GetZonePrefixFromAccountNo(accountNo);

                // Step 4: Generate the next bill code using zone and current month
                string nextBillCode = GenerateNextBillCode_Advanced(zonePrefix, DateTime.Now);

                // Step 5: Split the billcode and display only the number part (e.g., "0000401")
                string[] parts = nextBillCode.Split('-');
                if (parts.Length == 2)
                {
                    invoiceLabel.Text = parts[1]; // e.g., "0000401"
                }

                // Step 6: Display full billcode (e.g., "001-0000401")
                billCodeLabel.Text = nextBillCode;
                string[] billNumberParts = nextBillCode.Split('-');
                string output = int.Parse(billNumberParts[1]).ToString();

                Debug.WriteLine(output); // Output: 1
                extractedBillNumberLabel.Text = output;
            }

            discountedPercentLabel.Text = scPercent + '%';
            // Tax Exempt
            if (taxExempted == "0")
            {
                taxExemptedPercentLabel.Visible = true;
                taxExemptedPercentLabel.Text = "2%";

            }
            else
            {
                taxExemptedPercentLabel.Text = "0%";
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
            latestBillIdLabel.Text = latestBillID;
            // 🟦 Load reading info if available
            var readingInfo = RecentBillDetailsHelper.GetReadingInfoByBillId(latestBillID);
            if (readingInfo != null)
            {
                // to reading date of the recent bill is the from reading date of the next bill which is today
                dateBilledLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");

                // 🟦 Show user-friendly date
                fromReadingDateLabel.Text = readingInfo.ToReadingDate.ToString("MMMM dd, yyyy");

                // 🟦 Store DB-safe value (for processing later)
                fromReadingDateLabel.Tag = readingInfo.ToReadingDate;



                //READINGS LATEST BILL
                previousReadingTextBox.Text = readingInfo.PresentReading.ToString();


                //percentage label
                arrearsAmountLabel.Text = readingInfo.Balance.ToString("N2");





                Debug.WriteLine($"Previous Reading: {readingInfo.PreviousReading}");
                Debug.WriteLine($"Reading Date: {readingInfo.FromReadingDate.ToShortDateString()}");
            }
            else
            {
                Debug.WriteLine($"⚠️ No data found for bill_id: {latestBillID}");
            }


        }
        private string GenerateNextBillCode_Advanced(string zoneCode, DateTime billingDate)
        {
            string formattedBillCode = "";
            int nextBillNumber = 0;

            // 🟦 Step 1: Load dynamic zone order
            List<string> zoneOrder = LoadZoneCodesFromDatabase();
            int rangeSize = 100;

            int zoneIndex = zoneOrder.IndexOf(zoneCode);
            if (zoneIndex == -1)
                zoneIndex = 0;

            int zoneStart = (zoneIndex * rangeSize) + 1;
            int zoneEnd = zoneStart + rangeSize - 1;

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();

                // 🟦 Step 2: Get the max billcode used for this zone in current month range
                string query = @"
            SELECT MAX(CAST(SUBSTRING_INDEX(billcode, '-', -1) AS UNSIGNED)) AS maxnum
            FROM tb_bill
            WHERE CAST(SUBSTRING_INDEX(billcode, '-', -1) AS UNSIGNED) BETWEEN @start AND @end
            AND DATE_FORMAT(datebilled, '%Y%m') = @currentMonth";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@start", zoneStart);
                    cmd.Parameters.AddWithValue("@end", zoneEnd);
                    cmd.Parameters.AddWithValue("@currentMonth", billingDate.ToString("yyyyMM"));

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && int.TryParse(result.ToString(), out int lastNum))
                    {
                        nextBillNumber = lastNum + 1;
                    }
                    else
                    {
                        nextBillNumber = zoneStart;
                    }
                }
            }

            formattedBillCode = $"{zoneCode}-{nextBillNumber.ToString("D7")}";
            invoiceLabel.Text = nextBillNumber.ToString("D7");
            billCodeLabel.Text = formattedBillCode;

            return formattedBillCode;
        }



        private string GetZonePrefixFromAccountNo(string accountNo)
        {
            if (string.IsNullOrWhiteSpace(accountNo))
                return "001"; // Default fallback

            // Split account number by dash (e.g., "01-1-12-214C")
            string[] parts = accountNo.Split('-');

            if (parts.Length > 0 && int.TryParse(parts[0], out int zoneNumber))
            {
                // Format as 3-digit string with leading zeroes (e.g., 1 → "001")
                return zoneNumber.ToString("D3");
            }

            return "001"; // Fallback if parsing fails
        }
        private List<string> LoadZoneCodesFromDatabase()
        {
            List<string> zoneCodes = new List<string>();

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();

                string query = "SELECT zonecode FROM tb_zone ORDER BY CAST(zonecode AS UNSIGNED) ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add zonecode to the list (e.g., "001", "002", etc.)
                        zoneCodes.Add(reader["zonecode"].ToString());
                    }
                }
            }

            return zoneCodes;
        }

        private int GetZoneStartNumber(string zoneCode)
        {
            List<string> zoneOrder = LoadZoneCodesFromDatabase(); // Load from DB dynamically
            int rangeSize = 100; // or 200 if needed

            int index = zoneOrder.IndexOf(zoneCode);
            if (index >= 0)
            {
                return (index * rangeSize) + 1;
            }

            return 1; // fallback default
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
            int nextBillNumber = 1; // Default to 1 if no existing bill found

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();

                // Format date like "202507" (YYYYMM)
                string billingMonth = billingDate.ToString("yyyyMM");

                // SQL to find the latest bill number for this zone and month
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

            // Optional: show raw number in label
            extractedBillNumberLabel.Text = nextBillNumber.ToString();

            // Format the full bill code with 7-digit number (e.g., 003-0000014)
            formattedBillCode = $"{zonePrefix}-{nextBillNumber.ToString("D7")}";

            return formattedBillCode;
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
            totalWaterConsumptionAmountLabel.Text = totalAmount.ToString("N2");

            decimal scDiscounted = 0;
            decimal taxAdded = 0;
            decimal arrears = 0;

            // Remove "%" symbol and extra spaces
            string discountText = discountedPercentLabel.Text.Replace("%", "").Trim();
            string taxAddedText = taxExemptedPercentLabel.Text.Replace("%", "").Trim();

            // Try to parse the discount value
            if (decimal.TryParse(discountText, out decimal percent1))
            {
                scDiscounted = totalAmount * (percent1 / 100);
                discountedAmountLabel.Text = scDiscounted.ToString("N2");
            }
            else
            {
                discountedAmountLabel.Text = "0.00";
            }

            // Try to parse the tax/exemption value
            if (decimal.TryParse(taxAddedText, out decimal percent2))
            {
                taxAdded = totalAmount * (percent2 / 100);
                TaxExemptedAmountLabel.Text = taxAdded.ToString("N2");
            }
            else
            {
                TaxExemptedAmountLabel.Text = "0.00";
            }

            // Parse arrears from label text
            if (decimal.TryParse(arrearsAmountLabel.Text.Trim(), out decimal parsedArrears))
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
            subTotalAmountDueLabel.Text = chargeSubTotal.ToString("N2");
            string dueDateText = dueDateLabel.Text.Trim();
            DateTime duedate;

            if (DateTime.TryParse(dueDateText, out duedate))
            {

            }
            int isDueExempt = 0;
            if (!int.TryParse(dueExemptLabel.Text.Trim(), out isDueExempt))
            {
                // Optional: handle if parsing fails
                isDueExempt = 0; // or you can show a message or throw an error
            }
            decimal billCharge = chargeSubTotal;
            DateTime dueGracePeriod = duedate;
            int dueExempt = isDueExempt;
            int isArrears = 0;

            if (!int.TryParse(isArrearsLabel.Text.Trim(), out isArrears))
            {
                // Optional: handle if parsing fails
                isArrears = 0; // or you can show a message or throw an error
            }

            decimal existingPenalty = 0;
            if (!decimal.TryParse(penaltyAmountLabel.Text.Trim(), out existingPenalty))
            {
                // Optional: handle if parsing fails
                existingPenalty = 0; // or you can show a message or throw an error
            }
            int isPaid = 0;
            decimal penalty = GetPenaltyHelper.GetPenalty(
                billCharge,
                dueGracePeriod,
                dueExempt,
                isArrears,
                existingPenalty,
                isPaid
            );
            Debug.WriteLine(isArrears);
            penaltyAmountLabel.Text = penalty.ToString("N2");

            decimal totalCharge = chargeSubTotal + penalty;
            totalAmountDueLabel.Text = totalCharge.ToString("N2");


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
            discountedAmountLabel.Text = "0";
            TaxExemptedAmountLabel.Text = "0";
            subTotalAmountDueLabel.Text = "";
        }
        private void ClearBillingLabels()
        {
            // Clear discount and tax labels
            discountedPercentLabel.Text = "0";
            discountedAmountLabel.Text = "0";
            taxExemptedPercentLabel.Text = "0";
            TaxExemptedAmountLabel.Text = "0.00";
            subTotalAmountDueLabel.Text = "0.00";

            // Also clear totals
            totalQuantityLabel.Text = "0";
            totalWaterConsumptionAmountLabel.Text = "0.00";
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

            if (meterConsumedReadingTextBox.Text != "")
            {
                ClearWaterChargeLabels();
                totalAmountDueLabel.Text = "0.00";
            }
            // 🟡 Disable button if input is empty or zero
            if (string.IsNullOrEmpty(input) || input == "0")
            {
                printSaveButton.Enabled = false;
            }
            else
            {
                printSaveButton.Enabled = true;
            }

            // ✅ Try to parse the present reading
            if (int.TryParse(presentReadingTextBox.Text.Trim(), out int presentReading))
            {
                // ✅ Try to parse the previous reading
                if (int.TryParse(previousReadingTextBox.Text.Trim(), out int previousReading))
                {
                    // ✅ Validate that present reading is not less than previous reading
                    if (presentReading >= previousReading)
                    {
                        // ✅ Calculate meter consumed
                        int meterConsumed = presentReading - previousReading;

                        // ✅ Display meter consumed
                        meterConsumedReadingTextBox.Text = meterConsumed.ToString();

                        // ✅ Calculate water charges based on consumption
                        CalculateWaterCharges(meterConsumed);

                    }
                    else
                    {
                        // ❌ Present reading is less than previous reading
                        meterConsumedReadingTextBox.Clear();
                    }
                }
                else
                {
                    // ❌ Invalid input in previous reading
                    meterConsumedReadingTextBox.Clear();
                    ClearWaterChargeLabels();
                }
            }
            else
            {
                // ❌ Invalid input in present reading
                meterConsumedReadingTextBox.Clear();
                ClearWaterChargeLabels();
            }


        }
        private void LoadPayments()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DbConfig.ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM tb_payment WHERE paymentdate = CURDATE()";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    paymentsOnThisDayDataGridView.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void searchAccountNumberTextBox_TextChanged(object sender, EventArgs e)
        {
            string keyword = searchAccountNumberTextBox.Text.Trim().Replace("'", "''"); // prevent errors with single quotes

            if (accountDataGridView.DataSource is DataTable dt)
            {
                // Filter on both 'accountno' and 'fullname' columns
                dt.DefaultView.RowFilter = $"accountno LIKE '%{keyword}%' OR fullname LIKE '%{keyword}%'";
            }
        }

        private void zoneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected zone code from ComboBox
            string zoneCode = zoneComboBox.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(zoneCode))
                return;

            if (accountDataGridView.DataSource is DataTable dt)
            {
                // Filter rows where accountno starts with the selected zoneCode (e.g., "04-")
                dt.DefaultView.RowFilter = $"accountno LIKE '{zoneCode}-%'";

                // Sort rows in ascending order by accountno
                dt.DefaultView.Sort = "accountno ASC";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel18_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void isWithHoldingTaxLabel_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel38_Paint(object sender, PaintEventArgs e)
        {

        }

        private void amountPaidTextBox_Click(object sender, EventArgs e)
        {

        }

        private void searchAccountNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Code to be executed when Enter is pressed
                string keyword = searchAccountNumberTextBox.Text.Trim().Replace("'", "''"); // prevent errors with single quotes

                if (accountDataGridView.DataSource is DataTable dt)
                {
                    // Filter on both 'accountno' and 'fullname' columns
                    dt.DefaultView.RowFilter = $"accountno LIKE '%{keyword}%' OR fullname LIKE '%{keyword}%'";
                }
                e.Handled = true; // Optional, to prevent the Enter key from being processed further
            }
        }
    }
}
