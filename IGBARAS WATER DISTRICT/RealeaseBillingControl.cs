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
        private int _previousMeterConsumed = -1; // Default to -1 for first time check

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

            // First confirmation message
            string verifyDataMessage = "Please verify the input data carefully to ensure accuracy.\n\nDo you want to proceed with saving the billing record?";
            DialogResult verifyResult = MessageBox.Show(verifyDataMessage, "Verify Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (verifyResult == DialogResult.No)
            {
                return;
            }

            // Second confirmation message
            string preparePrinterMessage = "Please prepare the preprint paper and ensure the printer is properly set up and ready to print.\n\nAre you ready to proceed?";
            DialogResult prepareResult = MessageBox.Show(preparePrinterMessage, "Prepare Printer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (prepareResult == DialogResult.No)
            {
                return;
            }

            try
            {
                // Save billing data to database
                InsertToBillingTable(selectedBillingData);

                // Third confirmation message
                string printConfirmationMessage = "The billing record has been saved successfully.\n\nDo you want to print the billing invoice now?";
                DialogResult printResult = MessageBox.Show(printConfirmationMessage, "Print Invoice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (printResult == DialogResult.Yes)
                {
                    // Setup print document settings
                    billingPrintDocument.DefaultPageSettings.Landscape = false;
                    billingPrintDocument.DefaultPageSettings.Margins = new Margins(3, 3, 3, 3);

                    using (PrintDialog printDialog = new PrintDialog())
                    {
                        printDialog.Document = billingPrintDocument;
                        printDialog.AllowSomePages = false;
                        printDialog.AllowSelection = false;

                        // Show printer selection dialog
                        if (printDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Use selected printer settings
                            billingPrintDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);

                            // Print directly
                            billingPrintDocument.Print();
                        }
                    }
                }
                else
                {
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
            if(collectionTotalAmountPaidTextBox.Text == "0")
            {
                billPaidButton.Enabled = false;
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
        private void UpdateBillingRecord()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(DbConfig.ConnectionString))
                {
                    con.Open();

                    string query = @"UPDATE tb_bill SET
                totalbillcharge = @totalbillcharge,
                billcharge = @billcharge,
                balance = @balance,
                paid = @paid,
                amountpaid = @amountpaid,
                penaltyamount = @penaltyamount,
                arrearsamount = @arrearsamount,
                partiallypaid = @partiallypaid,
                adjustdebit = @adjustdebit,
                adjustcredit = @adjustcredit,
                uploaded = @uploaded,
                arrears = @arrears
                WHERE bill_id = @bill_id";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        // Parse monetary values from UI controls
                        double totalBillCharge = double.TryParse(totalAmountDueLabel2.Text.Replace(",", ""), out double tbc) ? tbc : 0;
                        double amountPaid = double.TryParse(collectionTotalPaidAmointLabel.Text.Replace(",", ""), out double ap) ? ap : 0;
                        double penaltyAmount = double.TryParse(collectionTotalAmountPaidTextBox.Text.Replace(",", ""), out double pa) ? pa : 0;
                        double arrearsAmount = double.TryParse(collectionArrearsAmountLabel.Text.Replace(",", ""), out double ar) ? ar : 0;


                        // Optional: Adjustments — customize if needed
                        double adjustDebit = 0;
                        double adjustCredit = 0;

                        // 1. Calculate balance
                        double balance = totalBillCharge - amountPaid;

                        // 2. Clamp balance to zero if overpaid
                        if (balance < 0) balance = 0;

                        // 3. Determine payment status
                        int paid = (balance == 0 && amountPaid > 0) ? 1 : 0;
                        int partiallyPaid = (amountPaid > 0 && balance > 0) ? 1 : 0;

                        // 4. Determine if overdue (for real-world arrears)
                        DateTime dueDate = DateTime.TryParse(dueDateLabel.Text, out DateTime dd) ? dd : DateTime.MinValue;
                        bool isOverdue = DateTime.Today > dueDate;

                        // 5. Set arrears only if unpaid or underpaid AND overdue
                        int arrears = (paid == 0 && isOverdue) ? 1 : 0;

                        // Add parameters to SQL command
                        cmd.Parameters.AddWithValue("@totalbillcharge", totalBillCharge);
                        cmd.Parameters.AddWithValue("@billcharge", totalBillCharge); // May separate later for raw vs full charge
                        cmd.Parameters.AddWithValue("@balance", balance);
                        cmd.Parameters.AddWithValue("@paid", paid);
                        cmd.Parameters.AddWithValue("@amountpaid", amountPaid);
                        cmd.Parameters.AddWithValue("@penaltyamount", penaltyAmount);
                        cmd.Parameters.AddWithValue("@arrearsamount", arrearsAmount);
                        cmd.Parameters.AddWithValue("@partiallypaid", partiallyPaid);
                        cmd.Parameters.AddWithValue("@adjustdebit", adjustDebit);
                        cmd.Parameters.AddWithValue("@adjustcredit", adjustCredit);
                        cmd.Parameters.AddWithValue("@uploaded", 0); // 0 = not yet synced to cloud
                        cmd.Parameters.AddWithValue("@arrears", arrears);
                        cmd.Parameters.AddWithValue("@bill_id", latestBillIdLabel.Text);

                        // Execute the update
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Debug.WriteLine("✅ Billing record updated successfully.", "Success");
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
            collectingOfficerNameLabel.Text = UserCredentials.Name;
            billPaidButton.Enabled = false;
            SetDateNow();
            LoadZoneComboBox();
            ClearWaterChargeLabels();
            ClearWaterChargeLabels2();
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
            AutoCompleteHelper.FillTextBoxWithColumns("v_concessionaire_detail", new string[] { "accountno", "name" }, searchAccountNumberTextBox);
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
            ClearWaterChargeLabels2();
            collectionTotalAmountPaidTextBox.Text = "0";
            fromReadingDateLabel.Text = "";
            previousReadingTextBox.Text = "0";
            meterConsumedReadingTextBox.Text = "0";
            presentReadingTextBox.Text = "";
            totalQuantityLabel.Text = "0.00";
            totalWaterConsumptionAmountLabel.Text = "0";
            totalAmountDueLabel.Text = "0.00";

            isWithHoldingTaxLabel.Text = "0";
            isArrearsLabel.Text = "0";
            dueExemptLabel.Text = "0";
            collectionTotalMeteredAmountLabel.Text = "0.00";
            totalWaterConsumptionAmountLabel2.Text = "0.00";
            collectionTaxAmountLabel.Text = "0.00";
            collectionTotalMeteredAmountLabel.Text = "0.00";
            collectionPenaltyLabel.Text = "0.00";
            collectionArrearsAmountLabel.Text = "0.00";
            collectionTotalPaidAmointLabel.Text = "0.00";
            taxExemptedPercentLabel2.Text = "2%";
            arrearsAmountLabel2.Text = "0.00";
            totalAmountDueLabel2.Text = "0.00";
            penaltyPercentLabel2.Text = "0%";
            totalQuantityLabel2.Text = "0";
            bankNumberTextBox.Text = "0.00";
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
            string concessionaireCode = selectedRow.Cells["concessionairecode"].Value?.ToString();

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
                    invoiceTextBox.Text = parts[1]; // e.g., "0000401"
                }

                // Step 6: Display full billcode (e.g., "001-0000401")
                billCodeLabel.Text = nextBillCode;
                string[] billNumberParts = nextBillCode.Split('-');
                string output = int.Parse(billNumberParts[1]).ToString();

                Debug.WriteLine(output); // Output: 1
                extractedBillNumberLabel.Text = output;
            }

            discountedPercentLabel.Text = scPercent + '%';
            discountedPercentLabel2.Text = scPercent + '%';
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

            //// Declare output variables
            //DateTime? usedDueDate;
            //int usedBill_id;
            //string usedBillCode;

            //// Call the helper method
            //decimal penalty = GetPenaltyHelper.GetPenalty(
            //    accountNo,
            //    billCharge,
            //    dueExempt,
            //    existingPenalty,
            //    isPaid,
            //    out usedDueDate,
            //    out usedBill_id,
            //    out usedBillCode
            //);

            concessionaireCodeLabel.Text = concessionaireCode + "-000001";
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




                //READINGS LATEST BILL
                previousReadingTextBox.Text = readingInfo.PresentReading.ToString();


                //percentage label
                arrearsAmountLabel.Text = readingInfo.Balance.ToString("N2");








                if (currentTabLabel.Text == "Collection Reciept")
                {
                    int totalConsumption = readingInfo.MeterConsumed;

                    {

                        // FOR THE COLLECTION TAB 
                        collectionNameLabel.Text = readingInfo.Name;
                        collectionAddressLabel.Text = readingInfo.Address;
                        collectionArrearsAmountLabel.Text = readingInfo.ArrearsAmount.ToString("N2");
                        arrearsAmountLabel2.Text = readingInfo.ArrearsAmount.ToString("N2");
                        dueDateLabel2.Text = readingInfo.DueDate.ToString("MMMM dd, yyyy");



                        collectionTaxAmountLabel.Text = readingInfo.TaxAmount.ToString("N2");
                        collectionTotalAddtionalChargeLabel.Text = readingInfo.TotalAditionalCharge.ToString("N2");
                        collectionBillingInvoiceTextBox.Text = readingInfo.BillCode.ToString();
                        ClearWaterChargeLabels2(); // clear values first


                        CalculateWaterCharges2(totalConsumption); // calculate new values
                        collectionPenaltyLabel.Text = penaltyAmountLabel2.Text;
                        collectionTotalMeteredAmountLabel.Text = totalWaterConsumptionAmountLabel2.Text; // update collection label
                    }
                }
                else
                {
                    ClearWaterChargeLabels();
                }


                    Debug.WriteLine($"Previous Reading: {readingInfo.PreviousReading}");
                Debug.WriteLine($"Reading Date: {readingInfo.FromReadingDate.ToShortDateString()}");
            }
            else
            {
                Debug.WriteLine($"⚠️ No data found for bill_id: {latestBillID}");
            }


        }

        private void InsertIntoPayments()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO tb_payment (
                    ornumber, paymentdate, billcode, transactioncode, transno,
                    bill, penalty, totalamount, districtno, concessionairecode, 
                    accountno, cash, service_connection_fee, 
                    other_charge1, other_charge2, other_charge3,
                    other_charge1_amount, other_charge2_amount, other_charge3_amount, 
                    total_other_charges, discount1, discount2, discount3,
                    discount1_amount, discount2_amount, discount3_amount, 
                    total_discount, tax, total_cheque, cash_tendered, bill_without_tax
                )
                VALUES (
                    @ornumber, @paymentdate, @billcode, @transactioncode, @transno,
                    @bill, @penalty, @totalamount, @districtno, @concessionairecode, 
                    @accountno, @cash, @scf, 
                    @other1, @other2, @other3,
                    @other1_amount, @other2_amount, @other3_amount, 
                    @total_other, @discount1, @discount2, @discount3,
                    @discount1_amount, @discount2_amount, @discount3_amount, 
                    @total_discount, @tax, @cheque, @cash_tendered, @bill_without_tax
                );
            ";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Basic info
                        cmd.Parameters.AddWithValue("@ornumber", orNumberTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@paymentdate", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@billcode", collectionBillingInvoiceTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@transactioncode", concessionaireCodeLabel.Text.Trim());
                        cmd.Parameters.AddWithValue("@transno", 1);

                        // Amounts (remove commas)
                        decimal billAmount = decimal.Parse(totalWaterConsumptionAmountLabel2.Text.Replace(",", "").Trim());
                        decimal penalty = decimal.Parse(penaltyAmountLabel2.Text.Replace(",", "").Trim());
                        decimal totalAmount = billAmount + penalty;

                        cmd.Parameters.AddWithValue("@bill", billAmount);
                        cmd.Parameters.AddWithValue("@penalty", penalty);
                        cmd.Parameters.AddWithValue("@totalamount", totalAmount);

                        // Concessionaire info
                        string accountNo = accountNumberTextBox.Text.Trim();
                        var (districtNo, concessionaireCode) = GetConcessionaireInfo(accountNo, conn);

                        cmd.Parameters.AddWithValue("@districtno", districtNo);
                        cmd.Parameters.AddWithValue("@concessionairecode", concessionaireCode);
                        cmd.Parameters.AddWithValue("@accountno", accountNo);

                        // Payment mode
                        if (cashCheckBox.Checked)
                        {
                            cmd.Parameters.AddWithValue("@cash", totalAmount);
                            cmd.Parameters.AddWithValue("@cheque", 0.00m);
                        }
                        else if (checkCheckBox.Checked)
                        {
                            cmd.Parameters.AddWithValue("@cash", 0.00m);
                            cmd.Parameters.AddWithValue("@cheque", totalAmount);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@cash", 0.00m);
                            cmd.Parameters.AddWithValue("@cheque", 0.00m);
                        }

                        // Service connection fee and other charges
                        cmd.Parameters.AddWithValue("@scf", 0.00m);
                        cmd.Parameters.AddWithValue("@other1", "");
                        cmd.Parameters.AddWithValue("@other2", "");
                        cmd.Parameters.AddWithValue("@other3", "");
                        cmd.Parameters.AddWithValue("@other1_amount", 0.00m);
                        cmd.Parameters.AddWithValue("@other2_amount", 0.00m);
                        cmd.Parameters.AddWithValue("@other3_amount", 0.00m);
                        cmd.Parameters.AddWithValue("@total_other", 0.00m);

                        // Discounts
                        cmd.Parameters.AddWithValue("@discount1", "");
                        cmd.Parameters.AddWithValue("@discount2", "");
                        cmd.Parameters.AddWithValue("@discount3", "");
                        decimal discount = decimal.TryParse(penaltyPercentLabel.Text.Replace(",", "").Trim(), out var d) ? d : 0.00m;
                        cmd.Parameters.AddWithValue("@discount1_amount", discount);
                        cmd.Parameters.AddWithValue("@discount2_amount", 0.00m);
                        cmd.Parameters.AddWithValue("@discount3_amount", 0.00m);
                        cmd.Parameters.AddWithValue("@total_discount", discount);

                        // Tax, cash tendered, bill without tax
                        cmd.Parameters.AddWithValue("@tax", 0.00m);
                        cmd.Parameters.AddWithValue("@cash_tendered", decimal.Parse(collectionTotalAmountPaidTextBox.Text.Replace(",", "").Trim()));
                        cmd.Parameters.AddWithValue("@bill_without_tax", billAmount);

                        // Execute
                        int rows = cmd.ExecuteNonQuery();
                        Debug.WriteLine("✅ Billing record updated successfully.", "Success");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ ERROR: " + ex.Message);
            }
        }
        private void InsertCheque(string chequeNo, decimal amount)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO tb_cheque ( dateissued, chequeno, amount)
                VALUES (@dateissued, @chequeno, @amount)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateissued", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@chequeno", chequeNo);
                        cmd.Parameters.AddWithValue("@amount", decimal.Parse(collectionTotalAmountPaidTextBox.Text.Replace(",", "").Trim()));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Failed to insert cheque data: " + ex.Message);
            }
        }


        private (int districtNo, string concessionaireCode) GetConcessionaireInfo(string accountNo, MySqlConnection conn)
        {
            string query = "SELECT districtno, concessionairecode FROM tb_concessionaire WHERE accountno = @accNo";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@accNo", accountNo);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            reader.GetInt32("districtno"),
                            reader.GetString("concessionairecode")
                        );
                    }
                }
            }

            return (0, ""); // fallback if not found
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
            invoiceTextBox.Text = nextBillNumber.ToString("D7");
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
                dt.DefaultView.RowFilter = $"accountno LIKE '%{keyword}%' OR name LIKE '%{keyword}%'";
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


        private void CalculateWaterCharges2(int totalConsumption)
        {
            // Define tier brackets
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

            // Calculate charges for each bracket
            foreach (var b in brackets)
            {
                if (remaining <= 0) break;

                int qty = Math.Min(b.Limit, remaining);
                decimal amount = qty * b.Price;

                // Display each tier breakdown
                switch (b.Label)
                {
                    case "ten":
                        tenQuantityLabel2.Text = qty.ToString();
                        tenUnitPriceLabel2.Text = b.Price.ToString("N2");
                        tenAmountLabel2.Text = amount.ToString("N2");
                        break;
                    case "twenty":
                        twentyQuantityLabel2.Text = qty.ToString();
                        twentyUnitPriceLabel2.Text = b.Price.ToString("N2");
                        twentyAmountLabel2.Text = amount.ToString("N2");
                        break;
                    case "thirty":
                        thirtyQuantityLabel2.Text = qty.ToString();
                        thirtyUnitPriceLabel2.Text = b.Price.ToString("N2");
                        thirtyAmountLabel2.Text = amount.ToString("N2");
                        break;
                    case "forty":
                        fortyQuantityLabel2.Text = qty.ToString();
                        fortyUnitPriceLabel2.Text = b.Price.ToString("N2");
                        fortyAmountLabel2.Text = amount.ToString("N2");
                        break;
                    case "fortyUp":
                        fortyUpQuantityLabel2.Text = qty.ToString();
                        fortyUpUnitPriceLabel2.Text = b.Price.ToString("N2");
                        fortyUpAmountLabel2.Text = amount.ToString("N2");
                        break;
                }

                remaining -= qty;
                totalQty += qty;
                totalAmount += amount;
            }

            // Display total quantity and base water charge
            totalQuantityLabel2.Text = totalQty.ToString();
            totalWaterConsumptionAmountLabel2.Text = totalAmount.ToString("N2");

            // Handle discounts
            decimal scDiscounted = 0;
            if (decimal.TryParse(discountedPercentLabel2.Text.Replace("%", "").Trim(), out decimal percent1))
            {
                scDiscounted = totalAmount * (percent1 / 100);
            }
            discountedAmountLabel2.Text = scDiscounted.ToString("N2");

            // Handle tax exemption
            decimal taxAdded = 0;
            if (decimal.TryParse(taxExemptedPercentLabel2.Text.Replace("%", "").Trim(), out decimal percent2))
            {
                taxAdded = totalAmount * (percent2 / 100);
            }
            TaxExemptedAmountLabel2.Text = taxAdded.ToString("N2");

            // Arrears
            decimal arrears = 0;
            if (decimal.TryParse(arrearsAmountLabel2.Text.Trim(), out decimal parsedArrears))
            {
                arrears = parsedArrears;
            }

            // Subtotal before penalty
            decimal chargeSubTotal = totalAmount - scDiscounted + taxAdded + arrears;
            subTotalAmountDueLabel2.Text = chargeSubTotal.ToString("N2");

            // Get Due Date
            DateTime duedate;
            DateTime.TryParse(dueDateLabel.Text.Trim(), out duedate);

            // Due Exempt Flag
            int dueExempt = 0;
            int.TryParse(dueExemptLabel.Text.Trim(), out dueExempt);

            // Arrears flag
            int isArrears = 0;
            int.TryParse(isArrearsLabel.Text.Trim(), out isArrears);

            // Existing Penalty
            decimal existingPenalty = 0;
            decimal.TryParse(penaltyAmountLabel2.Text.Trim(), out existingPenalty);

            // Is paid? (currently 0 by default)
            int isPaid = 0;

            // Call the penalty helper
            DateTime? usedDueDate;
            int usedBill_id;
            string usedBillCode;
            string accountNo = accountNumberTextBox.Text.Trim();

            decimal penalty = GetPenaltyHelper.GetPenalty(
                accountNo,
                chargeSubTotal,
                dueExempt,
                existingPenalty,
                isPaid,
                out usedDueDate,
                out usedBill_id,
                out usedBillCode
            );

            // After calculating penalty and before displaying totalCharge
            decimal penaltyPercent = 0;
            if (chargeSubTotal > 0)
            {
                penaltyPercent = (penalty / chargeSubTotal) * 100;
            }
            penaltyPercentLabel2.Text = $"{Math.Round(penaltyPercent)}%";


            // Display calculated penalty
            penaltyAmountLabel2.Text = penalty.ToString("N2");

            // Final total
            decimal totalCharge = chargeSubTotal + penalty;
            totalAmountDueLabel2.Text = totalCharge.ToString("N2");
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





        }



        private void meterConsumedReadingTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClearWaterChargeLabels2()
        {
            // Clear all tier 1 (0–10) labels
            tenQuantityLabel2.Text = tenUnitPriceLabel2.Text = tenAmountLabel2.Text = "";
            twentyQuantityLabel2.Text = twentyUnitPriceLabel2.Text = twentyAmountLabel2.Text = "";
            thirtyQuantityLabel2.Text = thirtyUnitPriceLabel2.Text = thirtyAmountLabel2.Text = "";
            fortyQuantityLabel2.Text = fortyUnitPriceLabel2.Text = fortyAmountLabel2.Text = "";
            fortyUpQuantityLabel2.Text = fortyUpUnitPriceLabel2.Text = fortyUpAmountLabel2.Text = "";

            // Clear subtotal and tax/discounts
            discountedAmountLabel2.Text = "0";
            TaxExemptedAmountLabel2.Text = "0";
            subTotalAmountDueLabel2.Text = "";
            penaltyAmountLabel2.Text = "0.00";
        }

        private void ClearAmounts()
        {

            // Clear discount and tax labels
            discountedPercentLabel.Text = "0";
            discountedAmountLabel.Text = "0";
            taxExemptedPercentLabel.Text = "0";
            TaxExemptedAmountLabel.Text = "0.00";
            subTotalAmountDueLabel.Text = "0.00";
            arrearsAmountLabel.Text = "0.00";
            totalAmountDueLabel.Text = "0.00";
            // Also clear totals
            totalQuantityLabel.Text = "0";
            totalWaterConsumptionAmountLabel.Text = "0.00";
        }
        private void ClearAmounts2()
        {
            // Clear discount and tax labels
            discountedPercentLabel2.Text = "0";
            discountedAmountLabel2.Text = "0";
            taxExemptedPercentLabel2.Text = "0";
            TaxExemptedAmountLabel2.Text = "0.00";
            arrearsAmountLabel2.Text = "0.00";
            subTotalAmountDueLabel2.Text = "0.00";
            totalAmountDueLabel2.Text = "0.00";

            totalWaterConsumptionAmountLabel2.Text = "0.00";
            // Also clear totals
            totalQuantityLabel.Text = "0";
            totalWaterConsumptionAmountLabel.Text = "0.00";


            collectionArrearsAmountLabel.Text = "0.00";
            collectionBillingInvoiceTextBox.Text = "000-0000000";
            collectionTotalAmountPaidTextBox.Text = "0.00";
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
                dt.DefaultView.RowFilter = $"accountno LIKE '%{keyword}%' OR name LIKE '%{keyword}%'";
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
                    dt.DefaultView.RowFilter = $"accountno LIKE '%{keyword}%' OR name LIKE '%{keyword}%'";
                }
                e.Handled = true; // Optional, to prevent the Enter key from being processed further
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabControl = sender as TabControl;
            var selectedTab = tabControl.SelectedTab; // TabPage object
            int selectedIndex = tabControl.SelectedIndex; // Index
            ClearWaterChargeLabels();
            ClearWaterChargeLabels2();
            ClearAmounts();
            ClearAmounts2();
            // Example: Show tab name in a label
            currentTabLabel.Text = $"{selectedTab.Text}";

            // Or use selectedIndex for logic
            // if (selectedIndex == 0) { ... }
        }

        private void totalAmountPaidTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // Disable the button if the textbox is empty or zero
            if (string.IsNullOrWhiteSpace(collectionTotalAmountPaidTextBox.Text) || collectionTotalAmountPaidTextBox.Text == "0")
            {
                billPaidButton.Enabled = false;
            }
            else
            {
                billPaidButton.Enabled = true;
            }

            // Return early if empty to avoid parsing issues
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                collectionTotalPaidAmointLabel.Text = "0.00";
                return;
            }

            // Save cursor position before formatting
            int cursorPosition = textBox.SelectionStart;

            // Remove commas to parse clean number
            string rawText = textBox.Text.Replace(",", "");

            // Try parsing the value from the textbox
            if (decimal.TryParse(rawText, out decimal paidAmount))
            {
                // Format with commas (e.g., 1,000.00)
                string formattedText = NumberFormatterHelper.FormatWithCommas(paidAmount);

                // Update text only if it's different (to avoid flicker)
                if (textBox.Text != formattedText)
                {
                    textBox.Text = formattedText;

                    // Move cursor to the end
                    textBox.SelectionStart = textBox.Text.Length;
                }

                // Try to parse the total amount due from the label
                if (decimal.TryParse(totalAmountDueLabel2.Text.Replace(",", ""), out decimal totalAmountDue))
                {
                    // Compare entered amount with total due
                    if (paidAmount > totalAmountDue)
                    {
                        // Show total amount due only if paid amount exceeds it
                        collectionTotalPaidAmointLabel.Text = totalAmountDue.ToString("N2");
                    }
                    else
                    {
                        // Show the entered amount if within limit
                        collectionTotalPaidAmointLabel.Text = formattedText;
                    }
                }
                else
                {
                    // Handle if the total due label contains invalid number
                    collectionTotalPaidAmointLabel.Text = "0.00";
                }
            }
            else
            {
                // If input is not valid number, clear textbox and label
                textBox.Text = "";
                collectionTotalPaidAmointLabel.Text = "0.00";
            }
        }


        private void billPaidButton_Click(object sender, EventArgs e)
        {
            // 🔒 Check if bill is already paid
            if (CheckIfBillIsPaid())
            {
                MessageBox.Show("❌ This bill has already been paid. Saving or printing is not allowed.", "Bill Already Paid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // First confirmation message
            string paymentConfirmationMessage = "Are you certain that this bill has been paid in full and all details are accurate?";
            DialogResult paymentResult = MessageBox.Show(paymentConfirmationMessage, "Confirm Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (paymentResult == DialogResult.No)
            {
                return;
            }

            // Second confirmation message
            string printerPreparationMessage = "Please ensure that the printer is properly set up, loaded with the correct paper (legal size), and all necessary documents are prepared for printing.\n\nAre you ready to proceed?";
            DialogResult printerResult = MessageBox.Show(printerPreparationMessage, "Prepare Printer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (printerResult == DialogResult.No)
            {
                return;
            }

            // Third confirmation message
            string printConfirmationMessage = "The billing record will now be saved and printed. This action cannot be undone.\n\nStart printing?";
            DialogResult printResult = MessageBox.Show(printConfirmationMessage, "Start Printing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (printResult == DialogResult.No)
            {
                return;
            }

            // Proceed with saving and printing
            try
                           
            {
                // Save billing record to database
                UpdateBillingRecord();
                InsertIntoPayments();
                if (checkCheckBox.Checked)
                {
                    string chequeNo = bankNumberTextBox.Text.Trim();
                    decimal chequeAmount = decimal.Parse(collectionTotalAmountPaidTextBox.Text.Trim());

                    InsertCheque(chequeNo, chequeAmount);
                }

                // Set printer settings to landscape and legal paper
                billingPrintDocument.DefaultPageSettings.Landscape = true;
                LoadPayments();
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

                // Print the billing document
                billingPrintDocument.Print();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ An error occurred while saving or printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //private void billPaidButton_Click(object sender, EventArgs e)
        //{
        //    // 🔒 Check if bill is already paid
        //    if (CheckIfBillIsPaid())
        //    {
        //        MessageBox.Show("❌ This bill has already been paid. Saving or printing is not allowed.", "Bill Already Paid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    try
        //    {
        //        // 💾 Save billing record to database
        //        UpdateBillingRecord();

        //        MessageBox.Show("✅ Billing record saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        // 🖨️ Set printer settings to landscape and legal paper
        //        billingPrintDocument.DefaultPageSettings.Landscape = true;

        //        foreach (PaperSize ps in billingPrintDocument.PrinterSettings.PaperSizes)
        //        {
        //            if (ps.Kind == PaperKind.Legal)
        //            {
        //                billingPrintDocument.DefaultPageSettings.PaperSize = ps;
        //                break;
        //            }
        //        }

        //        // Optional: Set margins (in hundredths of an inch, 30 = 0.3")
        //        billingPrintDocument.DefaultPageSettings.Margins = new Margins(30, 30, 30, 30);

        //        // 🖨️ Print the billing document
        //        billingPrintDocument.Print();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"❌ An error occurred while saving or printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private bool CheckIfBillIsPaid()
        {
            bool isPaid = false;

            try
            {
                using (MySqlConnection con = new MySqlConnection(DbConfig.ConnectionString))
                {
                    con.Open();
                    Debug.WriteLine("✅ Database connection opened.");

                    string query = "SELECT paid, datebilled FROM tb_bill WHERE bill_id = @bill_id";
                    Debug.WriteLine($"🟡 Executing query: {query}");

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        string bill_id = latestBillIdLabel.Text.Trim();
                        cmd.Parameters.AddWithValue("@bill_id", bill_id);
                        Debug.WriteLine($"🔍 Using bill ID: {bill_id}");

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


        //Y↓\X→|000|025|050|075|100|125|150|175|200|225|250|275|300|325|350|375|400|425|
        //-----+-----------------------------------------------------------------------+
        //000  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //025  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //050  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //075  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //100  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //125  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //150  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //175  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //200  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //225  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //250  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //275  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //300  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //325  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //350  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //375  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //400  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //425  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //450  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //475  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //500  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //525  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //550  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //575  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //600  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //625  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
        //650  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |




        private void printlangmuna_Click(object sender, EventArgs e)
        {
            // Create a new PrintDocument
            PrintDocument pd = new PrintDocument();

            // Optional: set the paper size to custom 8.25" x 11.75"
            pd.DefaultPageSettings.PaperSize = new PaperSize("CustomA4", 825, 1175); // 100 DPI units (1 inch = 100)


            // Assign the PrintPage handler
            pd.PrintPage += new PrintPageEventHandler(MapPrintPage);

            // Show a print dialog for user confirmation
            PrintDialog dialog = new PrintDialog();
            dialog.Document = pd;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print(); // Start the print job
            }
        }










        void MapPrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Consolas", 4);
            Pen gridPen = Pens.Orange;
            Brush brush = Brushes.Red;

            int paperWidth = 825;
            int paperHeight = 1175;
            int cellSize = 25;

            // 🔲 Draw Grid
            for (int x = 0; x <= paperWidth; x += cellSize)
                g.DrawLine(gridPen, x, 0, x, paperHeight);

            for (int y = 0; y <= paperHeight; y += cellSize)
                g.DrawLine(gridPen, 0, y, paperWidth, y);

            // 🏷 Label Cells
            for (int y = 0; y < paperHeight; y += cellSize)
            {
                for (int x = 0; x < paperWidth; x += cellSize)
                {
                    string label = $"{x},\n{y}";
                    g.DrawString(label, font, brush, x + 2, y + 2);
                }
            }

            string name = fullnameTextBox.Text;
            string address = addressTextBox.Text;
            string accountNo = accountNumberTextBox.Text;
            string dateBilled = dateBilledLabel.Text;
            string dueDate = dueDateLabel.Text;

            g.DrawString(name, font, Brushes.Black, 35, 65);
            g.DrawString(address, font, Brushes.Black, 25, 75);
            g.DrawString(accountNo, font, Brushes.Black, 25, 100);
            g.DrawString(dateBilled, font, Brushes.Black, 25, 125);
            g.DrawString(dueDate, font, Brushes.Black, 200, 125);

            string[] qtys = { "10", "2", "0" };
            string[] prices = { "15.00", "20.00", "0.00" };
            string[] amounts = { "150.00", "40.00", "0.00" };

            int rowStartY = 175;
            int rowSpacing = 25;

            for (int i = 0; i < qtys.Length; i++)
            {
                int rowY = rowStartY + (i * rowSpacing);

                g.DrawString(qtys[i], font, Brushes.Black, 200, rowY);
                g.DrawString(prices[i], font, Brushes.Black, 250, rowY);
                g.DrawString(amounts[i], font, Brushes.Black, 300, rowY);
            }

            e.HasMorePages = false;
        }


        void PrintPages(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 10);
            Brush brush = Brushes.Black;

            int receiptHeight = 390;

            for (int i = 0; i < 3; i++)
            {
                int yOffset = i * receiptHeight;

                // Example: Draw some key labels (map more as needed)
                g.DrawString("0.00", font, brush, 295, 1 + yOffset);   // totalWaterConsumptionAmountLabel
                g.DrawString("0", font, brush, 149, 1 + yOffset);       // totalQuantityLabel
                g.DrawString("0.00", font, brush, 222, 150 + yOffset);  // fortyUpUnitPriceLabel
                g.DrawString("0", font, brush, 149, 150 + yOffset);     // fortyUpQuantityLabel
                g.DrawString("0.00", font, brush, 295, 150 + yOffset);  // fortyUpAmountLabel
                g.DrawString("0.00", font, brush, 295, 125 + yOffset);  // fortyAmountLabel

                // ... add the rest of your label mappings here
            }

            e.HasMorePages = false;
        }

        private void sfcInstallmentTextBox_TextChanged(object sender, EventArgs e)
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

        private void tableLayoutPanel51_Paint(object sender, PaintEventArgs e)
        {

        }

        private void orNumberTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void collectionTotalPaidAmointLabel_Click(object sender, EventArgs e)
        {

        }

        private void cashCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (cashCheckBox.Checked)
            {
                // If cash is selected, disable check-related fields
                bankNumberTextBox.Enabled = false;
                checkCheckBox.Checked = false;
                bankNumberTextBox.Text = "";
            }
            else
            {
                // Enable check-related fields if cash is not selected
                bankNumberTextBox.Enabled = true;
                checkCheckBox.Enabled = true;
            }
        }

        private void checkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkCheckBox.Checked)
            {
                cashCheckBox.Checked = false;
               
            }
            else
            {
                cashCheckBox.Checked = true;
            }
        }
    }
}
