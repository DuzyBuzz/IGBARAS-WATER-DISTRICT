using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.Security.AccessControl;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using TextBox = System.Windows.Forms.TextBox;

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
            if (accountNumberTextBox.Text == null)
            {
                MessageBox.Show("No selected Account. Please select an account first.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CheckBillingDate())
            {
                MessageBox.Show("This customer is already billed for this month.", "Duplicate Billing", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                int serviceId = int.Parse(serviceIDLabel.Text.Trim());
                // Save billing data to database
                InsertToBillingTable(serviceId);

                // Third confirmation message
                string printConfirmationMessage = "The billing record has been saved successfully.\n\nDo you want to print the billing invoice now?";
                DialogResult printResult = MessageBox.Show(printConfirmationMessage, "Print Invoice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (printResult == DialogResult.Yes)
                {
                    // Create a new PrintDocument
                    PrintDocument pd = new PrintDocument();

                    // Optional: set the paper size to custom 8.25" x 11.75"
                    pd.DefaultPageSettings.PaperSize = new PaperSize("CustomA4", 825, 1175); // 100 DPI units (1 inch = 100)


                    // Assign the PrintPage handler
                    pd.PrintPage += new PrintPageEventHandler(BillingMapPrintPage);

                    // Show a print dialog for user confirmation
                    PrintDialog dialog = new PrintDialog();
                    dialog.Document = pd;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        pd.Print(); // Start the print job
                    }
                    SetNextBillNo();
                }
                else
                {
                    MessageBox.Show("The printing process was cancelled due to an interruption. Please try again if needed.",
                                    "Printing Cancelled",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ An error occurred while saving or printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void billPaidButton_Click(object sender, EventArgs e)
        {
            //// 🔒 Check if bill is already paid
            //if (CheckIfBillIsPaid())
            //{
            //    MessageBox.Show("This bill has already been paid or partially paid. Saving or printing is not allowed.", "Bill Already Paid or Partially Paid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

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
                // Save billing record to database
                //UpdateBillingRecord();
                InsertIntoPayments();

                //LoadPayments();

                // Third confirmation message
                string printConfirmationMessage = "The billing record has been saved successfully.\n\nDo you want to print the billing invoice now?";
                DialogResult printResult = MessageBox.Show(printConfirmationMessage, "Print Invoice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (printResult == DialogResult.Yes)
                {
                    // Create a new PrintDocument
                    PrintDocument pd = new PrintDocument();

                    // Optional: set the paper size to custom 8.25" x 11.75"
                    pd.DefaultPageSettings.PaperSize = new PaperSize("CustomA4", 825, 1175); // 100 DPI units (1 inch = 100)


                    // Assign the PrintPage handler
                    pd.PrintPage += new PrintPageEventHandler(CollectionMapPrintPage);

                    // Show a print dialog for user confirmation
                    PrintDialog dialog = new PrintDialog();
                    dialog.Document = pd;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        pd.Print(); // Start the print job
                    }
                    MessageBox.Show(
                        $"Concessionaire Change: ₱{changeLabel.Text}.",
                        "Transaction Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );



                }
                else
                {
                    MessageBox.Show("The printing process was cancelled due to an interruption. Please try again if needed.",
                                    "Printing Cancelled",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ An error occurred while saving or printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // user get bill settings helper to get the due date days ine tb_billsettings table




        /// <summary>

        /// <summary>
        /// end of the print save button click event.
        /// </summary>

        private bool CheckBillingDate()
        {
            string readingDateText = fromReadingDateLabel.Text.Trim();

            if (string.IsNullOrWhiteSpace(readingDateText))
            {
                // No reading date = not yet billed
                return false;
            }

            if (DateTime.TryParseExact(readingDateText, "MMM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fromDate))
            {
                DateTime now = DateTime.Now;

                if (fromDate.Month == now.Month && fromDate.Year == now.Year)
                {
                    return true;
                }

                return false;
            }

            // If the format is invalid, assume not billed — OR handle differently if needed
            return false;
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
            if (collectionTotalAmountPaidTextBox.Text == "0")
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
                        double penaltyAmount = double.TryParse(collectionPenaltyLabel.Text.Replace(",", ""), out double pa) ? pa : 0;
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
                        cmd.Parameters.AddWithValue("@bill_id", latestBillNoLabel.Text);

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
            toReadingDateLabel.Text = DateTime.Now.ToString("MMM-dd-yyyy");
            paymentDateLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
        }

        private void InsertToBillingTable(int serviceId)
        {
            try
            {
                using (var connection = new OleDbConnection(DbConfig.ConnectionString))
                {
                    connection.Open();

                    // Step 1: Get service rate info
                    string serviceQuery = "SELECT * FROM Tb_Service WHERE ServiceId = @serviceId";
                    using (var serviceCmd = new OleDbCommand(serviceQuery, connection))
                    {
                        serviceCmd.Parameters.AddWithValue("@serviceId", serviceId);

                        using (var reader = serviceCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string serviceType = reader["ServiceType"].ToString();
                                decimal minRate = Convert.ToDecimal(reader["MinRate"]);
                                decimal rate11_20 = Convert.ToDecimal(reader["Rate11-20"]);
                                decimal rate21_30 = Convert.ToDecimal(reader["Rate21-30"]);
                                decimal rate31_40 = Convert.ToDecimal(reader["Rate31-40"]);
                                decimal rate41_Above = Convert.ToDecimal(reader["Rate41-Above"]);

                                string insertQuery = @"
                        INSERT INTO Tb_Billing (
                            BillNo, DateCreated, AccountNo, ServiceDescription, DateFrom, DateTo, PrevReading, PresentReading, 
                            DueDate, MinRate, [Rate11-20], [Rate21-30], [Rate31-40], [Rate41-Above], PenaltyRate, 
                            Penalty, Tax, ServiceConnectionFee, Is_Arrears, DiscountName, Discount, DiscountAmount, ArrearsAmount
                        ) VALUES (
                            @BillNo, @DateCreated, @AccountNo, @ServiceDescription, @DateFrom, @DateTo, @PrevReading, @PresentReading, 
                            @DueDate, @MinRate, @Rate11_20, @Rate21_30, @Rate31_40, @Rate41_Above, 
                            @PenaltyRate, @Penalty, @Tax, @ServiceConnectionFee, @Is_Arrears, @DiscountName, @Discount, @DiscountAmount, @ArrearsAmount
                        )";

                                using (var insertCmd = new OleDbCommand(insertQuery, connection))
                                {
                                    insertCmd.Parameters.AddWithValue("@BillNo", int.Parse(invoiceTextBox.Text.Trim()));
                                    insertCmd.Parameters.AddWithValue("@DateCreated", DateTime.Now.ToString("MMMM dd, yyyy"));
                                    insertCmd.Parameters.AddWithValue("@AccountNo", accountNumberTextBox.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@ServiceDescription", serviceType);
                                    string fromDateText = fromReadingDateLabel.Text.Trim();

                                    if (string.IsNullOrWhiteSpace(fromDateText))
                                    {
                                        insertCmd.Parameters.AddWithValue("@DateFrom", DateTime.ParseExact(firstReadingDateLabel.Text.Trim(), "MMM-d-yyyy", CultureInfo.InvariantCulture));
                                    }
                                    else
                                    {
                                        insertCmd.Parameters.AddWithValue("@DateFrom",
                                            DateTime.ParseExact(fromDateText, "MMM-d-yyyy", CultureInfo.InvariantCulture));
                                    }

                                    insertCmd.Parameters.AddWithValue("@DateTo", DateTime.ParseExact(toReadingDateLabel.Text.Trim(), "MMM-d-yyyy", CultureInfo.InvariantCulture));
                                    insertCmd.Parameters.AddWithValue("@PrevReading", int.Parse(previousReadingTextBox.Text.Trim()));
                                    insertCmd.Parameters.AddWithValue("@PresentReading", int.Parse(presentReadingTextBox.Text.Trim()));

                                    insertCmd.Parameters.AddWithValue("@DueDate", DateTime.ParseExact(dueDateLabel.Text.Trim(), "MMMM dd, yyyy", CultureInfo.InvariantCulture));
                                    insertCmd.Parameters.AddWithValue("@MinRate", minRate);
                                    insertCmd.Parameters.AddWithValue("@Rate11_20", rate11_20);
                                    insertCmd.Parameters.AddWithValue("@Rate21_30", rate21_30);
                                    insertCmd.Parameters.AddWithValue("@Rate31_40", rate31_40);
                                    insertCmd.Parameters.AddWithValue("@Rate41_Above", rate41_Above);

                                    insertCmd.Parameters.AddWithValue("@PenaltyRate", int.Parse(penaltyPercentLabel.Text.Trim().Replace("%", "")));
                                    insertCmd.Parameters.AddWithValue("@Penalty", decimal.Parse(penaltyAmountLabel.Text.Trim().Replace(",", "")));
                                    insertCmd.Parameters.AddWithValue("@Tax", int.Parse(taxExemptedPercentLabel.Text.Trim().Replace("%", "")));
                                    insertCmd.Parameters.AddWithValue("@ServiceConnectionFee", decimal.Parse(sfcInstallmentTextBox.Text.Trim()));
                                    insertCmd.Parameters.AddWithValue("@Is_Arrears", int.Parse(isArrearsLabel.Text.Trim()));
                                    insertCmd.Parameters.AddWithValue("@DiscountName", discountNameLabel.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@Discount", int.Parse(discountedPercentLabel.Text.Trim().Replace("%", "")));
                                    insertCmd.Parameters.AddWithValue("@DiscountAmount", decimal.Parse(discountedAmountLabel.Text.Trim().Replace(",", "")));
                                    insertCmd.Parameters.AddWithValue("@ArrearsAmount", decimal.Parse(arrearsAmountLabel.Text.Trim().Replace(",", "")));


                                    insertCmd.ExecuteNonQuery();
                                    MessageBox.Show("Billing record inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    printSaveButton.Enabled = false;
                                    ClearWaterChargeLabels();
                                    ClearBillinInfo();
                                    SetNextBillNo();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Service not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Insert failed:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearBillinInfo()
        {
            fullnameTextBox.Text = string.Empty;
            addressTextBox.Text = string.Empty;
            accountNumberTextBox.Text = string.Empty;

            fromReadingDateLabel.Text = string.Empty;

            previousReadingTextBox.Text = string.Empty;
            presentReadingTextBox.Text = string.Empty;
            collectionTotalMeteredAmountLabel.Text = string.Empty;


        }

        private void SetNextBillNo()
        {
            string query = "SELECT MAX(Val(BillNo)) FROM Tb_Billing";
            using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();

                int latestBillNo = 0;

                if (result != DBNull.Value && int.TryParse(result.ToString(), out int parsedNo))
                {
                    latestBillNo = parsedNo;
                }

                invoiceTextBox.Text = (latestBillNo + 1).ToString();
            }
        }

        private void SetNextORNo()
        {
            string query = "SELECT MAX(Val(ORNumber)) FROM Tb_Payments";
            using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();

                int latestORNo = 0;

                if (result != DBNull.Value && int.TryParse(result.ToString(), out int parsedNo))
                {
                    latestORNo = parsedNo;
                }

                orNumberTextBox.Text = (latestORNo + 1).ToString();
            }
        }

        private async void BillingControl_Load(object sender, EventArgs e)
        {

            collectingOfficerNameLabel.Text = UserCredentials.Fullname;
            billPaidButton.Enabled = false;
            SetDateNow();
            SetNextBillNo();
            SetNextORNo();
            ClearWaterChargeLabels();
            ClearWaterChargeLabels2();

            PlaceholderHelper.AddPlaceholder(searchAccountNumberTextBox, "🔎 Fullname or Account Number.");
            PlaceholderHelper.AddPlaceholder(remarksTextBox, "📝 Remarks");

            ClearButtonDisable();
            // 🟡 Load data from DB to billingDataGridView
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountDataGridView, "Tb_Concessionaire", loadingForm);

                await Task.WhenAll(task1);
            }
            // 🟢 Optional: Setup autocomplete after data loaded
            AutoCompleteHelper.FillTextBoxWithColumns("Tb_Concessionaire", new string[] { "AccountNo", "ConcessionaireName" }, searchAccountNumberTextBox);
            AutoCompleteHelper.FillTextBoxWithColumns("Tb_Payments", new string[] { "BankName" }, bankNameTextBox);
            cashCheckBox.Checked = true; // Default to cash payment
            //LoadPayments();

            FormatDataGridView(accountDataGridView);
            FormatDataGridView(billDataGridView);
            FormatDataGridView(paymentsOnThisDayDataGridView);
            LoadZoneComboBox();
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
            arrearsAmountLabel.Text = "0.00";
            collectionTotalAmountPaidTextBox.Text = "0";
            fromReadingDateLabel.Text = "";
            previousReadingTextBox.Text = "0";
            meterConsumedReadingTextBox.Text = "0";
            presentReadingTextBox.Text = "";
            totalQuantityLabel.Text = "0";
            totalWaterConsumptionAmountLabel.Text = "0.00";
            totalAmountDueLabel.Text = "0.00";
            sfcInstallmentTextBox.Text = "0.00";
            minimumChargeLabel.Text = "0.00";
            penaltyAmountLabel.Text = "0.00";
            penaltyPercentLabel.Text = "0%";
            checkBox1.Checked = false;


            isWithHoldingTaxLabel.Text = "0";
            isArrearsLabel.Text = "0";
            dueExemptLabel.Text = "0";
            collectionTotalMeteredAmountLabel.Text = "0.00";
            totalWaterConsumptionAmountLabel2.Text = "0.00";
            collectionTaxAmountLabel.Text = "0.00";
            collectionPenaltyLabel.Text = "0.00";
            collectionArrearsAmountLabel.Text = "0.00";
            collectionTotalPaidAmointLabel.Text = "0.00";
            taxExemptedPercentLabel2.Text = "0%";
            arrearsAmountLabel2.Text = "0.00";
            totalAmountDueLabel2.Text = "0.00";
            penaltyPercentLabel2.Text = "0%";
            totalQuantityLabel2.Text = "0";
            bankNameTextBox.Text = "";
            checkNumberTextBox.Text = "";
            bankAccountNumberText.Text = "";
            cashCheckBox.Checked = true;
            DisableButton();
            if (e.RowIndex < 0) return; // Ignore header or invalid rows

            // 🟦 Get selected row
            DataGridViewRow selectedRow = accountDataGridView.Rows[e.RowIndex];

            // 🟦 Extract individual values using the column names
            string accountNo = selectedRow.Cells["accountno"].Value?.ToString();
            string fullname = selectedRow.Cells["fullname"].Value?.ToString();
            string address = selectedRow.Cells["address"].Value?.ToString();
            string zoneCode = selectedRow.Cells["zoneCode"].Value?.ToString();
            string serviceID = selectedRow.Cells["serviceId"].Value?.ToString();
            string meterNo = selectedRow.Cells["meterNo"].Value?.ToString();
            string frdObj = selectedRow.Cells["firstReadingDate"].Value?.ToString();
            int taxExempt = Convert.ToInt32(selectedRow.Cells["taxExempt"].Value);
            int IsSeniorCitizen = Convert.ToInt32(selectedRow.Cells["seniorCitizen"].Value);
            string dueExempted = selectedRow.Cells["dueExempt"].Value?.ToString();
            string status = selectedRow.Cells["status"].Value?.ToString();


            if (DateTime.TryParse(frdObj?.ToString(), out DateTime frd))
            {
                fromReadingDateLabel.Text = frd.ToString("MMM-dd-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fromReadingDateLabel.Text = ""; // or show a default/fallback message
            }
            discountedPercentLabel.Text = $"{DiscountHelper.GetSeniorCitizenDiscountPercent(IsSeniorCitizen)}%";
            if (IsSeniorCitizen == 1)
            {
                discountNameLabel.Text = "SENIOR CITIZEN";
            }
            else
            {
                discountNameLabel.Text = "";
            }
            defaultDiscount = discountedPercentLabel.Text;
            defaultDiscountName = discountNameLabel.Text;
            firstReadingDateLabel.Text = frdObj;
            serviceIDLabel.Text = serviceID;
            dueExemptLabel.Text = dueExempted;
            double taxPercent = SettingsHelper.GetTaxPercent(taxExempt);
            taxExemptedPercentLabel.Text = $"{taxPercent:0.##}%";
            if (!string.IsNullOrWhiteSpace(accountNo))
            {
                // Load billing history
                LoadAccountBillHistory(accountNo);

                // 🟦 Get latest bill number
                string latestBillNo = GetLatestBillNoHelper.GetLatestBillNo(accountNo);
                Debug.WriteLine(!string.IsNullOrEmpty(latestBillNo)
                    ? $"✅ Latest bill_id: {latestBillNo}"
                    : $"⚠️ No bill found for account number: {accountNo}");
                // Convert latestBillNo to string if it's not already
                string billNo = latestBillNoLabel.Text;

                // Create an instance of RecentBillDetailsHelper
                var recentBillDetailsHelper = new RecentBillDetailsHelper();
                var bill = recentBillDetailsHelper.GetBillByBillNo(latestBillNo);

                if (bill != null)
                {
                    string message =
                        $"Account No: {bill.AccountNo}\n" +
                        $"Billing Period: {bill.DateFrom:MMMM dd, yyyy} to {bill.DateTo:MMMM dd, yyyy}\n" +
                        $"Previous Reading: {bill.PrevReading:N0} cu.m\n" +
                        $"Present Reading: {bill.PresentReading:N0} cu.m\n" +
                        $"Penalty: {bill.Penalty:C}\n" +
                        $"Tax: {bill.Tax:C}\n" +
                        $"Total Paid: {bill.AmountPaid:C}\n" +
                        $"Due Date: {bill.DueDate:MMMM dd, yyyy}";
                    fromReadingDateLabel.Text = $"{bill.DateTo:MMM-dd-yyyy}";
                    previousReadingTextBox.Text = $"{bill.PresentReading}";
                    arrearsAmountLabel.Text = $"{bill.Balance.ToString("N2")}";
                    arrearsAmountLabel2.Text = $"{bill.ArrearsAmount.ToString("N2")}";
                    Debug.WriteLine($"{bill.Balance}");
                    if (arrearsAmountLabel.Text != "0.00")
                    {
                        isArrearsLabel.Text = "-1";
                    }
                    else
                    {
                        isArrearsLabel.Text = "0";
                    }
                    if (currentTabLabel.Text == "Collection Reciept")
                    {
                        // Fix for CS0266: Explicitly cast 'double' to 'int' to resolve the type mismatch.
                        int meterConsumed = Math.Max(0, (int)(bill.PresentReading - bill.PrevReading));
                        Debug.WriteLine($"Meter consumed: {meterConsumed} cu.m");
                        meterConsumedReadingTextBox.Text = meterConsumed.ToString();
                        taxExemptedPercentLabel2.Text = $"{bill.Tax}%";
                        discountedPercentLabel2.Text = $"{bill.Discount}%";
                        dueDateLabel2.Text = bill.DueDate.ToString("MMMM dd, yyyy");

                        if (int.TryParse(serviceIDLabel.Text.Trim(), out int serviceId))
                        {
                            PopulateServiceRateLabels2(serviceId, meterConsumed);
                        }
                        collectionNameLabel.Text = fullname;
                        collectionAddressLabel.Text = address;
                        collectionBillingInvoiceTextBox.Text = bill.BillNo;




                    }

                }
                else
                {
                }
            }

            // Tax Exempt


            // 🟦 Update UI fields
            accountNumberTextBox.Text = accountNo;

            fullnameTextBox.Text = fullname;
            addressTextBox.Text = address;
            accountnoBillHistory.Text = $"Account ID: {accountNo}";
        }

        private void InsertIntoPayments()
        {
            try
            {
                using (var connection = new OleDbConnection(DbConfig.ConnectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                INSERT INTO Tb_Payments (
                    ORNumber, CurrentBillNo, AccountNo, PaymentDate, PaymentType, ArrearsAmount, ArrearsPenalty, TotalArrears, 
                    BillCharge, TaxAmount, TotalCurrent, CheckNumber, BankName, BankAccountNumber, DateIssued, 
                    CheckAmount, CashAmount, AmountPaid, [Net Bill Charge], Balance, DiscountName, DiscountAmount, 
                    Penalty, ServiceConnectionFee, Remarks, OthersAmount1
                ) VALUES (
                    @ORNumber, @CurrentBillNo, @AccountNo, @PaymentDate, @PaymentType, @ArrearsAmount, @ArrearsPenalty, @TotalArrears, 
                    @BillCharge, @TaxAmount, @TotalCurrent, @CheckNumber, @BankName, @BankAccountNumber, @DateIssued, 
                    @CheckAmount, @CashAmount, @AmountPaid, @NetBillCharge, @Balance, @DiscountName, @DiscountAmount, 
                    @Penalty, @ServiceConnectionFee, @Remarks, @OthersAmount1
                )";

                    // Pre-calculate values used in both insert and update
                    double totalBillCharge = double.TryParse(totalAmountDueLabel2.Text.Replace(",", ""), out double tbc) ? tbc : 0;
                    double amountPaid = double.TryParse(collectionTotalPaidAmointLabel.Text.Replace(",", ""), out double ap) ? ap : 0;
                    double penaltyAmount = double.TryParse(penaltyAmountLabel2.Text.Replace(",", ""), out double pa) ? pa : 0;
                    double balance = totalBillCharge - amountPaid;
                    if (balance < 0) balance = 0;

                    decimal arrearsAmount = decimal.Parse(arrearsAmountLabel2.Text.Trim().Replace(",", ""));
                    decimal arrearsPenalty = SettingsHelper.CalculatePenaltyOnArrears(arrearsAmount);
                    decimal totalArrears = arrearsAmount + arrearsPenalty;

                    // INSERT payment record
                    using (var insertCmd = new OleDbCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@ORNumber", int.Parse(orNumberTextBox.Text.Trim()));
                        insertCmd.Parameters.AddWithValue("@CurrentBillNo", int.Parse(collectionBillingInvoiceTextBox.Text.Trim()));
                        insertCmd.Parameters.AddWithValue("@AccountNo", accountNumberTextBox.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now.ToString("M/d/yyyy"));

                        insertCmd.Parameters.AddWithValue("@PaymentType", cashCheckBox.Checked ? "Cash" : "Check");
                        insertCmd.Parameters.AddWithValue("@ArrearsAmount", arrearsAmount);
                        insertCmd.Parameters.AddWithValue("@ArrearsPenalty", arrearsPenalty);
                        insertCmd.Parameters.AddWithValue("@TotalArrears", totalArrears);

                        insertCmd.Parameters.AddWithValue("@BillCharge", decimal.Parse(totalWaterConsumptionAmountLabel2.Text.Replace(",", "")));
                        insertCmd.Parameters.AddWithValue("@TaxAmount", decimal.Parse(taxAmountLabel2.Text.Replace(",", "")));
                        insertCmd.Parameters.AddWithValue("@TotalCurrent", decimal.Parse(subTotalAmountDueLabel2.Text.Replace(",", "")));

                        insertCmd.Parameters.AddWithValue("@CheckNumber", checkNumberTextBox.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@BankName", bankNameTextBox.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@BankAccountNumber", bankAccountNumberText.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@DateIssued", checkDateIssuedDateTimePicker.Value.ToString("M/d/yyyy"));

                        insertCmd.Parameters.AddWithValue("@CheckAmount", cashCheckBox.Checked ? 0 : decimal.Parse(collectionTotalPaidAmointLabel.Text.Trim()));
                        insertCmd.Parameters.AddWithValue("@CashAmount", cashCheckBox.Checked ? decimal.Parse(collectionTotalPaidAmointLabel.Text.Trim()) : 0);

                        insertCmd.Parameters.AddWithValue("@AmountPaid", amountPaid);
                        insertCmd.Parameters.AddWithValue("@NetBillCharge", decimal.Parse(totalAmountDueLabel2.Text.Replace(",", "")));
                        insertCmd.Parameters.AddWithValue("@Balance", balance);
                        insertCmd.Parameters.AddWithValue("@DiscountName", discountNameLabel.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@DiscountAmount", decimal.Parse(discountedAmountLabel2.Text.Replace(",", "")));
                        insertCmd.Parameters.AddWithValue("@Penalty", penaltyAmount);
                        insertCmd.Parameters.AddWithValue("@ServiceConnectionFee", decimal.Parse(collectionSCFTextBox.Text.Replace(",", "")));
                        insertCmd.Parameters.AddWithValue("@Remarks", remarksTextBox.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@OthersAmount1", decimal.Parse(collectionOtherPaymentTextBox.Text.Replace(",", "")));

                        insertCmd.ExecuteNonQuery();
                    }

                    // UPDATE billing status
                    string updateBillingQuery = @"
                UPDATE Tb_Billing
                SET Is_FullyPaid = @IsFullyPaid, Is_PartiallyPaid = @IsPartiallyPaid
                WHERE BillNo = @BillNo";

                    using (var updateCmd = new OleDbCommand(updateBillingQuery, connection))
                    {
                        bool isFullyPaid = amountPaid >= totalBillCharge;
                        bool isPartiallyPaid = amountPaid > 0 && amountPaid < totalBillCharge;

                        updateCmd.Parameters.AddWithValue("@IsFullyPaid", isFullyPaid);
                        updateCmd.Parameters.AddWithValue("@IsPartiallyPaid", isPartiallyPaid);
                        updateCmd.Parameters.AddWithValue("@BillNo", int.Parse(collectionBillingInvoiceTextBox.Text.Trim()));

                        updateCmd.ExecuteNonQuery();
                        SetNextORNo();

                    }

                    MessageBox.Show("Payment record inserted and billing status updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Insert failed:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //private string GenerateNextBillCode_Advanced(string zoneCode, DateTime billingDate)
        //{
        //    string formattedBillCode = "";
        //    int nextBillNumber = 0;

        //    // 🟦 Step 1: Load dynamic zone order
        //    List<string> zoneOrder = LoadZoneCodesFromDatabase();
        //    int rangeSize = 100;

        //    int zoneIndex = zoneOrder.IndexOf(zoneCode);
        //    if (zoneIndex == -1)
        //        zoneIndex = 0;

        //    int zoneStart = (zoneIndex * rangeSize) + 1;
        //    int zoneEnd = zoneStart + rangeSize - 1;

        //    using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
        //    {
        //        conn.Open();

        //        // 🟦 Step 2: Get the max billcode used for this zone in current month range
        //        string query = @"
        //    SELECT MAX(CAST(SUBSTRING_INDEX(billcode, '-', -1) AS UNSIGNED)) AS maxnum
        //    FROM tb_bill
        //    WHERE CAST(SUBSTRING_INDEX(billcode, '-', -1) AS UNSIGNED) BETWEEN @start AND @end
        //    AND DATE_FORMAT(datebilled, '%Y%m') = @currentMonth";

        //        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@start", zoneStart);
        //            cmd.Parameters.AddWithValue("@end", zoneEnd);
        //            cmd.Parameters.AddWithValue("@currentMonth", billingDate.ToString("yyyyMM"));

        //            object result = cmd.ExecuteScalar();
        //            if (result != DBNull.Value && int.TryParse(result.ToString(), out int lastNum))
        //            {
        //                nextBillNumber = lastNum + 1;
        //            }
        //            else
        //            {
        //                nextBillNumber = zoneStart;
        //            }
        //        }
        //    }

        //    formattedBillCode = $"{zoneCode}-{nextBillNumber.ToString("D7")}";
        //    invoiceTextBox.Text = nextBillNumber.ToString("D7");
        //    billCodeLabel.Text = formattedBillCode;

        //    return formattedBillCode;
        //}



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
        //private List<string> LoadZoneCodesFromDatabase()
        //{
        //    List<string> zoneCodes = new List<string>();

        //    using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
        //    {
        //        conn.Open();

        //        string query = "SELECT zonecode FROM tb_zone ORDER BY CAST(zonecode AS UNSIGNED) ASC";

        //        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        //        using (MySqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                // Add zonecode to the list (e.g., "001", "002", etc.)
        //                zoneCodes.Add(reader["zonecode"].ToString());
        //            }
        //        }
        //    }

        //    return zoneCodes;
        //}



        private void LoadAccountBillHistory(string accountNo)
        {
            string query = @"
                SELECT
                    BillNo AS [Bill No],
                    DateCreated AS [Date Billed],
                    DateFrom AS [Period From],
                    DateTo AS [Period To],
                    PrevReading AS [Previous Reading],
                    PresentReading AS [Present Reading],
                    (PresentReading - PrevReading) AS [Consumption (m³)],
                    DueDate AS [Due Date],
                    IIF(Is_PartiallyPaid = True, 'Partially Paid',
                        IIF(Is_FullyPaid = True, 'Fully Paid', 'Unpaid')) AS [Status]
                FROM Tb_Billing
                WHERE AccountNo = ?
                ORDER BY BillNo DESC;
            ";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", accountNo);

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            billDataGridView.DataSource = dt;

                            billDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                            if (billDataGridView.Columns.Contains("Status"))
                            {
                                var col = billDataGridView.Columns["Status"];
                                col.DefaultCellStyle.ForeColor = Color.Red;
                                col.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load bill history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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





        private async void clearButton_Click(object sender, EventArgs e)
        {
            if (accountDataGridView.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = ""; // reset filter
            }

            searchAccountNumberTextBox.Clear();
            using (var loadingForm = new LoadingForm())
            {
                var task1 = DGVHelper.LoadDataToGridAsync(accountDataGridView, "Tb_Concessionaire", loadingForm);

                await Task.WhenAll(task1);
            }
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
        new { Limit = 10, Label = "twenty", Price = 37.15m },
        new { Limit = 10, Label = "thirty", Price = 39.15m },
        new { Limit = 10, Label = "forty", Price = 41.15m },
        new { Limit = int.MaxValue, Label = "fortyUp", Price = 43.15m }
    };

            int remaining = Math.Max(totalConsumption, 10); // Ensure minimum of 10 cubic meters
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
            totalQuantityLabel2.Text = Math.Max(totalConsumption, 10).ToString();
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
            taxAmountLabel2.Text = taxAdded.ToString("N2");

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
        new { Limit = 10, Label = "twenty", Price = 37.15m },
        new { Limit = 10, Label = "thirty", Price = 39.15m },
        new { Limit = 10, Label = "forty", Price = 41.15m },
        new { Limit = int.MaxValue, Label = "fortyUp", Price = 43.15m }
    };

            int remaining = Math.Max(totalConsumption, 10); // Ensure minimum of 10 cubic meters
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
            totalQuantityLabel.Text = totalConsumption.ToString();

            totalWaterConsumptionAmountLabel.Text = totalAmount.ToString("N2");

            decimal scDiscounted = 0;
            decimal taxAdded = 0;
            decimal arrears = 0;

            // Remove "%" symbol and extra spaces
            string discountText = discountedPercentLabel.Text.Replace("%", "").Trim() ?? "0";
            string taxAddedText = taxExemptedPercentLabel.Text.Replace("%", "").Trim() ?? "0";

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
                taxAmountLabel.Text = taxAdded.ToString("N2");
            }
            else
            {
                taxAmountLabel.Text = "0.00";
            }

            // Parse arrears from label text
            if (decimal.TryParse(arrearsAmountLabel.Text.Replace(",", "").Trim(), out decimal parsedArrears))

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
        public void PopulateServiceRateLabels2(int serviceId, int totalConsumption)
        {
            using (var conn = new OleDbConnection(DbConfig.ConnectionString))
            {
                string query = @"
                    SELECT MinRate, [Rate11-20], [Rate21-30], [Rate31-40], [Rate41-Above]
                    FROM Tb_Service
                    WHERE ServiceID = ?";
                using (var cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", serviceId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Get service rates
                            decimal minRate = Convert.ToDecimal(reader["MinRate"]);
                            decimal rate11_20 = Convert.ToDecimal(reader["Rate11-20"]);
                            decimal rate21_30 = Convert.ToDecimal(reader["Rate21-30"]);
                            decimal rate31_40 = Convert.ToDecimal(reader["Rate31-40"]);
                            decimal rate41_above = Convert.ToDecimal(reader["Rate41-Above"]);

                            int q10 = Math.Min(totalConsumption, 10);
                            int q20 = Math.Min(Math.Max(totalConsumption - 10, 0), 10);
                            int q30 = Math.Min(Math.Max(totalConsumption - 20, 0), 10);
                            int q40 = Math.Min(Math.Max(totalConsumption - 30, 0), 10);
                            int q41 = Math.Max(totalConsumption - 40, 0);

                            decimal a10 = q10 > 0 ? minRate : 0; // Minimum charge
                            decimal a20 = q20 * rate11_20;
                            decimal a30 = q30 * rate21_30;
                            decimal a40 = q40 * rate31_40;
                            decimal a41 = q41 * rate41_above;

                            decimal total = a10 + a20 + a30 + a40 + a41;

                            // Populate labels
                            tenQuantityLabel2.Text = q10.ToString();
                            tenUnitPriceLabel2.Text = (minRate / 10).ToString("N2");
                            tenAmountLabel2.Text = a10.ToString("N2");

                            twentyQuantityLabel2.Text = q20.ToString();
                            twentyUnitPriceLabel2.Text = rate11_20.ToString("N2");
                            twentyAmountLabel2.Text = a20.ToString("N2");

                            thirtyQuantityLabel2.Text = q30.ToString();
                            thirtyUnitPriceLabel2.Text = rate21_30.ToString("N2");
                            thirtyAmountLabel2.Text = a30.ToString("N2");

                            fortyQuantityLabel2.Text = q40.ToString();
                            fortyUnitPriceLabel2.Text = rate31_40.ToString("N2");
                            fortyAmountLabel2.Text = a40.ToString("N2");

                            fortyUpQuantityLabel2.Text = q41.ToString();
                            fortyUpUnitPriceLabel2.Text = rate41_above.ToString("N2");
                            fortyUpAmountLabel2.Text = a41.ToString("N2");

                            minimumChargeLabel2.Text = minRate.ToString("N2");
                            totalWaterConsumptionAmountLabel2.Text = total.ToString("N2");
                            totalQuantityLabel2.Text = totalConsumption.ToString();


                        }

                        decimal discounted = 0;
                        decimal taxAdded = 0;
                        decimal arrears = 0;

                        // Clean up input texts
                        string discountText = discountedPercentLabel2.Text.Replace("%", "").Trim();
                        string taxAddedText = taxExemptedPercentLabel2.Text.Replace("%", "").Trim();

                        if (!decimal.TryParse(totalWaterConsumptionAmountLabel2.Text.Trim(), out decimal totalAmount))
                        {
                            totalAmount = 0;
                        }

                        // Parse Discount
                        if (decimal.TryParse(discountText, out decimal percent1))
                        {
                            discounted = totalAmount * (percent1 / 100);
                            discountedAmountLabel2.Text = discounted.ToString("N2");
                        }
                        else
                        {
                            discountedAmountLabel2.Text = "0.00";
                        }

                        // Step 1: Subtract discount from total
                        decimal discountedTotal = totalAmount - discounted;

                        // Parse Tax
                        if (decimal.TryParse(taxAddedText, out decimal percent2))
                        {
                            taxAdded = discountedTotal * (percent2 / 100);
                            taxAmountLabel2.Text = taxAdded.ToString("N2");
                        }
                        else
                        {
                            taxAdded = 0;
                            taxAmountLabel2.Text = "0.00";
                        }

                        // Parse Arrears
                        if (!decimal.TryParse(arrearsAmountLabel2.Text.Replace(",", "").Trim(), out arrears))
                        {
                            arrears = 0;
                        }

                        // Step 2: Compute subtotal before SCF and penalties
                        decimal chargeSubTotal = discountedTotal + taxAdded + arrears;
                        subTotalAmountDueLabel2.Text = chargeSubTotal.ToString("N2");

                        // Step 3: Get Due Date (required for late penalty)
                        DateTime dueDate;
                        if (!DateTime.TryParseExact(dueDateLabel2.Text, "MMMM dd, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate))
                        {
                            dueDate = DateTime.Now; // fallback, or handle differently if needed
                        }

                        // Calculate penalties
                        decimal arrearsPenalty = SettingsHelper.CalculatePenaltyOnArrears(arrears);
                        decimal latePenalty = SettingsHelper.CalculateLatePaymentPenalty(chargeSubTotal, dueDate);

                        penaltyAmountLabel2.Text = latePenalty.ToString("N2");

                        List<string> parts = new();

                        if (arrearsPenalty > 0)
                            parts.Add($"{arrearsPenalty:N2}");

                        if (latePenalty > 0)
                            parts.Add($"{latePenalty:N2}");

                        if (parts.Count > 0)
                        {
                            collectionPenaltyLabel.Visible = true;
                            collectionPenaltyLabel.Text = string.Join(" + ", parts);
                            penaltySumLabel.Text = (arrearsPenalty + latePenalty).ToString("N2");
                        }
                        else
                        {
                            collectionPenaltyLabel.Visible = true;
                            collectionPenaltyLabel.Text = "0.00";
                            penaltySumLabel.Text = "0.00";
                        }

                        arrearsPenaltyAmountLabel.Text = arrearsPenalty.ToString("N2");

                        collectionArrearsAmountLabel.Text = arrearsAmountLabel2.Text;
                        collectionTaxAmountLabel.Text = taxAmountLabel2.Text;
                        collectionSCFTextBox.Text = sfcInstallmentTextBox2.Text.Trim();

                        // Step 5: Add SCF, Other Payment and Penalties
                        decimal scf = decimal.Parse(sfcInstallmentTextBox2.Text.Trim());
                        decimal othersPayment = decimal.Parse(collectionOtherPaymentTextBox.Text.Trim());

                        decimal totalAmountDue = chargeSubTotal + scf + othersPayment + arrearsPenalty + latePenalty;
                        arrearsPenaltyLabel.Text = arrearsPenalty.ToString();


                        if (chargeSubTotal > 0)
                        {
                            decimal totalPenalty = arrearsPenalty + latePenalty; // combine both penalties
                            decimal penaltyPercent = (latePenalty / chargeSubTotal) * 100;
                            penaltyPercentLabel2.Text = $"{penaltyPercent:0}%";

                        }
                        else
                        {
                            penaltyPercentLabel2.Text = "0.00%";
                        }

                        // Display final amount due
                        totalAmountDueLabel2.Text = totalAmountDue.ToString("N2");

                        decimal totalMeteredAmount = decimal.Parse(totalWaterConsumptionAmountLabel2.Text);
                        decimal discount = decimal.Parse(discountedAmountLabel2.Text);

                        decimal netAmount = totalMeteredAmount - discount;

                        if (discount > 0)
                        {
                            // Format like: "₱500.00 - ₱50.00 = ₱450.00"
                            netAmount = totalMeteredAmount - discount;
                            collectionTotalMeteredAmountLabel.Text =
                                $"{netAmount:N2}";
                        }
                        else
                        {
                            // Just show the total if no discount
                            collectionTotalMeteredAmountLabel.Text = $"{totalMeteredAmount:N2}";
                        }


                    }
                }
            }
        }

        public void PopulateServiceRateLabels(int serviceId, int totalConsumption)
        {
            using (var conn = new OleDbConnection(DbConfig.ConnectionString))
            {
                string query = @"
                    SELECT MinRate, [Rate11-20], [Rate21-30], [Rate31-40], [Rate41-Above]
                    FROM Tb_Service
                    WHERE ServiceID = ?";
                using (var cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", serviceId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Get service rates
                            decimal minRate = Convert.ToDecimal(reader["MinRate"]);
                            decimal rate11_20 = Convert.ToDecimal(reader["Rate11-20"]);
                            decimal rate21_30 = Convert.ToDecimal(reader["Rate21-30"]);
                            decimal rate31_40 = Convert.ToDecimal(reader["Rate31-40"]);
                            decimal rate41_above = Convert.ToDecimal(reader["Rate41-Above"]);

                            int q10 = Math.Min(totalConsumption, 10);
                            int q20 = Math.Min(Math.Max(totalConsumption - 10, 0), 10);
                            int q30 = Math.Min(Math.Max(totalConsumption - 20, 0), 10);
                            int q40 = Math.Min(Math.Max(totalConsumption - 30, 0), 10);
                            int q41 = Math.Max(totalConsumption - 40, 0);

                            decimal a10 = q10 > 0 ? minRate : 0; // Minimum charge
                            decimal a20 = q20 * rate11_20;
                            decimal a30 = q30 * rate21_30;
                            decimal a40 = q40 * rate31_40;
                            decimal a41 = q41 * rate41_above;

                            decimal total = a10 + a20 + a30 + a40 + a41;

                            // Populate labels
                            tenQuantityLabel.Text = q10.ToString();
                            tenUnitPriceLabel.Text = (minRate / 10).ToString("N2");
                            tenAmountLabel.Text = a10.ToString("N2");

                            twentyQuantityLabel.Text = q20.ToString();
                            twentyUnitPriceLabel.Text = rate11_20.ToString("N2");
                            twentyAmountLabel.Text = a20.ToString("N2");

                            thirtyQuantityLabel.Text = q30.ToString();
                            thirtyUnitPriceLabel.Text = rate21_30.ToString("N2");
                            thirtyAmountLabel.Text = a30.ToString("N2");

                            fortyQuantityLabel.Text = q40.ToString();
                            fortyUnitPriceLabel.Text = rate31_40.ToString("N2");
                            fortyAmountLabel.Text = a40.ToString("N2");

                            fortyUpQuantityLabel.Text = q41.ToString();
                            fortyUpUnitPriceLabel.Text = rate41_above.ToString("N2");
                            fortyUpAmountLabel.Text = a41.ToString("N2");

                            minimumChargeLabel.Text = minRate.ToString("N2");
                            totalWaterConsumptionAmountLabel.Text = total.ToString("N2");
                            totalQuantityLabel.Text = totalConsumption.ToString();


                        }

                        decimal discounted = 0;
                        decimal taxAdded = 0;
                        decimal arrears = 0;

                        // Clean up input texts
                        string discountText = discountedPercentLabel.Text.Replace("%", "").Trim();
                        string taxAddedText = taxExemptedPercentLabel.Text.Replace("%", "").Trim();

                        if (!decimal.TryParse(totalWaterConsumptionAmountLabel.Text.Trim(), out decimal totalAmount))
                        {
                            totalAmount = 0;
                        }

                        // Parse Discount
                        if (decimal.TryParse(discountText, out decimal percent1))
                        {
                            discounted = totalAmount * (percent1 / 100);
                            discountedAmountLabel.Text = discounted.ToString("N2");
                        }
                        else
                        {
                            discountedAmountLabel.Text = "0.00";
                        }

                        // Step 1: Subtract discount from total
                        decimal discountedTotal = totalAmount - discounted;

                        // Parse Tax
                        if (decimal.TryParse(taxAddedText, out decimal percent2))
                        {
                            taxAdded = discountedTotal * (percent2 / 100);
                            taxAmountLabel.Text = taxAdded.ToString("N2");
                        }
                        else
                        {
                            taxAmountLabel.Text = "0.00";
                        }

                        // Parse Arrears
                        if (!decimal.TryParse(arrearsAmountLabel.Text.Replace(",", "").Trim(), out arrears))
                        {
                            arrears = 0;
                        }

                        // Step 2: Final Charge Calculation
                        decimal chargeSubTotal = discountedTotal + taxAdded + arrears;

                        // Display Final Total
                        subTotalAmountDueLabel.Text = chargeSubTotal.ToString("N2");


                        decimal scf = decimal.Parse(sfcInstallmentTextBox.Text.Trim());
                        // Display total amount due
                        decimal totalAmountDue = chargeSubTotal + scf;

                        totalAmountDueLabel.Text = chargeSubTotal.ToString("N2");
                        // You can now add this penalty to your total calculation


                    }
                }
            }
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
            taxAmountLabel2.Text = "0.00";
            subTotalAmountDueLabel2.Text = "";
            penaltyAmountLabel2.Text = "0.00";
        }

        private void ClearAmounts()
        {

            // Clear discount and tax labels
            discountedPercentLabel.Text = "0";
            discountedAmountLabel.Text = "0";
            taxExemptedPercentLabel.Text = "0";
            taxAmountLabel.Text = "0.00";
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
            discountedPercentLabel2.Text = "0%";
            discountedAmountLabel2.Text = "0.00";
            taxExemptedPercentLabel2.Text = "0%";
            taxAmountLabel2.Text = "0.00";
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
            taxAmountLabel.Text = "0";
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
                totalQuantityLabel.Text = "0";
                totalWaterConsumptionAmountLabel.Text = "0.00";
                subTotalAmountDueLabel.Text = "0.00";
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
                        meterConsumedReadingTextBox.Text = meterConsumed.ToString();
                        if (int.TryParse(serviceIDLabel.Text.Trim(), out int serviceId))
                        {
                            PopulateServiceRateLabels(serviceId, meterConsumed);
                        }

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
                e.SuppressKeyPress = true;

                string keyword = searchAccountNumberTextBox.Text.Trim();

                if (string.IsNullOrEmpty(keyword)) return;

                // Prevent special character issues
                keyword = keyword.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");

                if (accountDataGridView.DataSource is DataTable dt)
                {
                    // Ensure exact column names are used from your MDB table
                    if (dt.Columns.Contains("AccountNo") && dt.Columns.Contains("ConcessionaireName"))
                    {
                        dt.DefaultView.RowFilter =
                            $"Convert(AccountNo, 'System.String') LIKE '%{keyword}%' OR Convert(ConcessionaireName, 'System.String') LIKE '%{keyword}%'";
                    }
                    else
                    {
                        MessageBox.Show("Ensure your MDB columns are named exactly 'AccountNo' and 'ConcessionaireName'.", "Column Name Error");
                    }
                }
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
            // Remove commas and trim spaces, then parse to decimal
            decimal totalDue = decimal.TryParse(totalAmountDueLabel2.Text.Replace(",", "").Trim(), out decimal dueValue) ? dueValue : 0;
            decimal totalPaid = decimal.TryParse(collectionTotalAmountPaidTextBox.Text.Replace(",", "").Trim(), out decimal paidValue) ? paidValue : 0;

            // Subtract to get the change
            decimal change = totalPaid - totalDue;

            // Ensure negative values are shown as 0.00
            change = Math.Max(change, 0);

            // Format and display
            changeLabel.Text = change.ToString("N2");


        }



        private bool CheckIfBillIsPaid()
        {
            bool isPaid = false;

            try
            {
                using (MySqlConnection con = new MySqlConnection(DbConfig.ConnectionString))
                {
                    con.Open();

                    string query = "SELECT paid, partiallypaid, datebilled FROM tb_bill WHERE bill_id = @bill_id";
                    Debug.WriteLine($"🟡 Executing query: {query}");

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        string bill_id = latestBillNoLabel.Text.Trim();
                        cmd.Parameters.AddWithValue("@bill_id", bill_id);
                        Debug.WriteLine($"🔍 Using bill ID: {bill_id}");

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int paidValue = 0;
                                int partiallyPaidValue = 0;
                                DateTime dateBilled = DateTime.MinValue;

                                if (!reader.IsDBNull(reader.GetOrdinal("paid")))
                                {
                                    paidValue = reader.GetInt32("paid");
                                }

                                if (!reader.IsDBNull(reader.GetOrdinal("partiallypaid")))
                                {
                                    partiallyPaidValue = reader.GetInt32("partiallypaid");
                                }

                                if (!reader.IsDBNull(reader.GetOrdinal("datebilled")))
                                {
                                    dateBilled = reader.GetDateTime("datebilled");
                                }

                                Debug.WriteLine($"📄 Retrieved: paid = {paidValue}, partiallypaid = {partiallyPaidValue}, dateBilled = {(dateBilled == DateTime.MinValue ? "NULL" : dateBilled.ToString("yyyy-MM-dd"))}");

                                int currentYear = DateTime.Now.Year;
                                int currentMonth = DateTime.Now.Month;

                                Debug.WriteLine($"📅 Current Date: {DateTime.Now:yyyy-MM-dd}");

                                // 🔍 Check either paid OR partially paid, and if it's for the current month
                                if ((paidValue == 1 || partiallyPaidValue == 1) &&
                                    dateBilled.Year == currentYear &&
                                    dateBilled.Month == currentMonth)
                                {
                                    isPaid = true;
                                    Debug.WriteLine("✅ Bill is PAID (fully or partially) this month.");
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
        //450  |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
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
            PrintDocument pd = new PrintDocument();

            // Optional: set the paper size to custom 8.25" x 11.75"
            pd.DefaultPageSettings.PaperSize = new PaperSize("CustomA4", 825, 1175); // 100 DPI units (1 inch = 100)


            // Assign the PrintPage handler
            pd.PrintPage += new PrintPageEventHandler(BillingMapPrintPage);

            // Show a print dialog for user confirmation
            PrintDialog dialog = new PrintDialog();
            dialog.Document = pd;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print(); // Start the print job
            }
        }

        public void CollectionMapPrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Calibre", 9);
            Pen gridPen = Pens.Orange;
            Brush brush = Brushes.Red;

            int paperWidth = 825;
            int paperHeight = 1175;
            //int cellSize = 25;

            //// 🔲 Draw Grid
            //for (int x = 0; x <= paperWidth; x += cellSize)
            //    g.DrawLine(gridPen, x, 0, x, paperHeight);

            //for (int y = 0; y <= paperHeight; y += cellSize)
            //    g.DrawLine(gridPen, 0, y, paperWidth, y);

            //// 🏷 Label Cells
            //for (int y = 0; y < paperHeight; y += cellSize)
            //{
            //    for (int x = 0; x < paperWidth; x += cellSize)
            //    {
            //        string label = $"{x},\n{y}";
            //        g.DrawString(label, font, brush, x + 2, y + 2);
            //    }
            //}

            //header information
            string paymentDate = paymentDateLabel.Text;

            //personal information
            string name = collectionNameLabel.Text;
            string address = collectionAddressLabel.Text;
            string metered = collectionTotalMeteredAmountLabel.Text;
            string arrears = collectionArrearsAmountLabel.Text;
            string penalty = collectionPenaltyLabel.Text;
            string tax = collectionTaxAmountLabel.Text;

            string scf = collectionSCFTextBox.Text;
            string others = collectionOtherPaymentTextBox.Text;


            string totalamount = collectionTotalPaidAmointLabel.Text;

            string collectingOfficer = collectingOfficerNameLabel.Text;


            g.DrawString(paymentDate, font, Brushes.Black, 340, 170);
            g.DrawString(name, font, Brushes.Black, 110, 200);
            g.DrawString(address, font, Brushes.Black, 110, 248);


            g.DrawString(metered, font, Brushes.Black, 300, 310);
            g.DrawString(arrears, font, Brushes.Black, 300, 335);
            g.DrawString(penalty, font, Brushes.Black, 300, 360);
            g.DrawString(tax, font, Brushes.Black, 300, 385);
            g.DrawString(scf, font, Brushes.Black, 300, 410);
            g.DrawString(others, font, Brushes.Black, 300, 435);




            g.DrawString(totalamount, font, Brushes.Black, 300, 455);





            e.HasMorePages = false;
        }



        private void DrawBillingForm(Graphics g, int offsetY, Font font, Brush brush)
        {
            // Draw each field with offsetY applied
            g.DrawString(dateBilledLabel.Text, font, brush, 300, 105 + offsetY);
            g.DrawString(fullnameTextBox.Text, font, brush, 190, 153 + offsetY);
            g.DrawString(addressTextBox.Text, font, brush, 190, 168 + offsetY);
            g.DrawString(accountNumberTextBox.Text, font, brush, 190, 200 + offsetY);

            g.DrawString(fromReadingDateLabel.Text, font, brush, 288, 213 + offsetY);
            g.DrawString(toReadingDateLabel.Text, font, brush, 368, 213 + offsetY);

            g.DrawString(previousReadingTextBox.Text, font, brush, 210, 265 + offsetY);
            g.DrawString(presentReadingTextBox.Text, font, brush, 290, 265 + offsetY);
            g.DrawString(meterConsumedReadingTextBox.Text, font, brush, 370, 265 + offsetY);

            g.DrawString(dueDateLabel.Text, font, brush, 670, 35 + offsetY);

            g.DrawString(totalQuantityLabel.Text, font, brush, 605, 90 + offsetY);
            g.DrawString(minimumChargeLabel.Text, font, brush, 648, 105 + offsetY);
            g.DrawString(totalWaterConsumptionAmountLabel.Text, font, brush, 700, 90 + offsetY);

            g.DrawString(tenQuantityLabel.Text, font, brush, 605, 125 + offsetY);
            g.DrawString(twentyQuantityLabel.Text, font, brush, 605, 140 + offsetY);
            g.DrawString(thirtyQuantityLabel.Text, font, brush, 605, 155 + offsetY);
            g.DrawString(fortyQuantityLabel.Text, font, brush, 605, 170 + offsetY);
            g.DrawString(fortyUpQuantityLabel.Text, font, brush, 605, 185 + offsetY);

            g.DrawString(tenUnitPriceLabel.Text, font, brush, 648, 125 + offsetY);
            g.DrawString(twentyUnitPriceLabel.Text, font, brush, 648, 140 + offsetY);
            g.DrawString(thirtyUnitPriceLabel.Text, font, brush, 648, 155 + offsetY);
            g.DrawString(fortyUnitPriceLabel.Text, font, brush, 648, 170 + offsetY);
            g.DrawString(fortyUpUnitPriceLabel.Text, font, brush, 648, 185 + offsetY);

            g.DrawString(tenAmountLabel.Text, font, brush, 700, 125 + offsetY);
            g.DrawString(twentyAmountLabel.Text, font, brush, 700, 140 + offsetY);
            g.DrawString(thirtyAmountLabel.Text, font, brush, 700, 155 + offsetY);
            g.DrawString(fortyAmountLabel.Text, font, brush, 700, 170 + offsetY);
            g.DrawString(fortyUpAmountLabel.Text, font, brush, 700, 185 + offsetY);

            g.DrawString(discountedAmountLabel.Text, font, brush, 700, 203 + offsetY);
            g.DrawString(taxAmountLabel.Text, font, brush, 700, 218 + offsetY);
            g.DrawString(arrearsAmountLabel.Text, font, brush, 700, 248 + offsetY);
            g.DrawString(sfcInstallmentTextBox.Text, font, brush, 700, 263 + offsetY);
            g.DrawString(subTotalAmountDueLabel.Text, font, brush, 700, 283 + offsetY);
            g.DrawString(penaltyAmountLabel.Text, font, brush, 700, 298 + offsetY);
            g.DrawString(subTotalAmountDueLabel.Text, font, brush, 700, 313 + offsetY);
        }





        public void BillingMapPrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Calibre", 9);
            Brush brush = Brushes.Black;

            for (int i = 0; i < 3; i++)
            {
                int offsetY = i * 363;
                DrawBillingForm(g, offsetY, font, brush);
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

            // Save the original cursor position
            int cursorPosition = textBox.SelectionStart;

            // Remove commas before parsing
            string rawText = textBox.Text.Replace(",", "");

            if (decimal.TryParse(rawText, out decimal value))
            {
                // Format to N2 (with commas)
                string formattedText = value.ToString("N2");

                // Update only if necessary
                if (textBox.Text != formattedText)
                {
                    textBox.Text = formattedText;

                    // Adjust cursor to nearest valid position
                    int newCursorPos = Math.Min(cursorPosition + (textBox.Text.Length - rawText.Length), textBox.Text.Length);
                    textBox.SelectionStart = newCursorPos;
                }
            }
            else
            {
                // Invalid input; default to 0.00
                textBox.Text = "0.00";
                textBox.SelectionStart = textBox.Text.Length;
            }

            // Parse amountDue safely
            if (!decimal.TryParse(subTotalAmountDueLabel.Text.Trim(), out decimal amountDue))
            {
                amountDue = 0.00m;
            }

            // Parse penalty safely
            if (!decimal.TryParse(penaltyAmountLabel.Text.Trim(), out decimal penalty))
            {
                penalty = 0.00m;
            }

            // Parse SCF (Service Connection Fee / SFC Installment)
            if (!decimal.TryParse(sfcInstallmentTextBox.Text.Trim(), out decimal scf))
            {
                scf = 0.00m;
            }

            // Compute total amount due
            decimal totalAmountDue = amountDue + penalty + scf;

            // Format as currency
            totalAmountDueLabel.Text = totalAmountDue.ToString("N2");

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
                bankNameTextBox.Enabled = false;
                checkNumberTextBox.Enabled = false;
                bankAccountNumberText.Enabled = false;
                checkCheckBox.Checked = false;
                bankNameTextBox.Text = "";
                checkNumberTextBox.Text = "";
                bankAccountNumberText.Text = "";
            }
            else
            {
                // Enable check-related fields if cash is not selected
                bankNameTextBox.Enabled = true;
                checkNumberTextBox.Enabled = true;
                bankAccountNumberText.Enabled = true;
                checkCheckBox.Checked = true;
                checkCheckBox.Enabled = true;
            }
        }

        private void checkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkCheckBox.Checked)
            {
                cashCheckBox.Checked = false;
                PlaceholderHelper.AddPlaceholder(checkNumberTextBox, "Check No.");
                PlaceholderHelper.AddPlaceholder(bankNameTextBox, "Bank Name");
                PlaceholderHelper.AddPlaceholder(bankAccountNumberText, "Bank Account No.");
                chequePanel.Enabled = true;
                checkDateIssuedDateTimePicker.Format = DateTimePickerFormat.Short;
                checkDateIssuedDateTimePicker.Value = DateTime.Now;
            }
            else
            {
                checkDateIssuedDateTimePicker.Format = DateTimePickerFormat.Custom;
                checkDateIssuedDateTimePicker.CustomFormat = " "; 


                chequePanel.Enabled = false;
                cashCheckBox.Checked = true;
            }
        }
        private void LoadPayments()
        {
            string query = @"
                    SELECT
                        p.ornumber AS 'OR Number',
                        b.billcode AS 'Invoice Number',
                        b.accountno AS 'Account Number',
                        b.name AS 'Customer Name',
                        p.totalamount AS 'Amount Paid',
                        CASE
                            WHEN b.partiallypaid = 1 THEN 'Partially Paid'
                            WHEN b.paid = 0 THEN 'Partially Paid'
                            WHEN b.paid = 1 AND EXISTS (
                                SELECT 1 FROM tb_payment p2 WHERE p2.billcode = b.billcode
                            ) THEN 'Fully Paid'
                            ELSE 'Unpaid'
                        END AS 'Status',
                        p.paymentdate AS 'Payment Date',
                        b.balance AS 'Balance'
                    FROM tb_bill b
                    JOIN tb_payment p ON b.billcode = p.billcode
                    WHERE DATE(p.paymentdate) = CURDATE()
                    ORDER BY p.paymentdate DESC;

                        ";

            using (MySqlConnection con = new MySqlConnection(DbConfig.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    paymentsOnThisDayDataGridView.DataSource = dt;

                    // Format currency columns
                    string[] currencyColumns = { "Penalty Amount", "Bill Total", "Paid Today" };
                    foreach (DataGridViewColumn col in paymentsOnThisDayDataGridView.Columns)
                    {
                        if (currencyColumns.Contains(col.HeaderText))
                            col.DefaultCellStyle.Format = "₱#,##0.00";
                    }
                }
            }
        }
        private void FormatDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Arial", 10);
            dgv.EnableHeadersVisualStyles = false;
        }

        private void searchAccountNumberTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void thirtyQuantityLabel_Click(object sender, EventArgs e)
        {

        }

        private void collectingOfficerNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void accountDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Only format once per row (check if you're on the first column, or skip if you like)
            if (e.RowIndex >= 0 && accountDataGridView.Rows[e.RowIndex].Cells["status"].Value != null)
            {
                string status = accountDataGridView.Rows[e.RowIndex].Cells["status"].Value.ToString().Trim().ToLower();

                if (status == "disconnected")
                {
                    accountDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 204, 204);
                    accountDataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black; // Optional
                }
                else if (status == "active")
                {
                    accountDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 204, 255, 204);
                    accountDataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black; // Optional
                }
                else
                {
                    // Reset for other statuses
                    accountDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    accountDataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void accountDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void accountDataGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void discountCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void sfcInstallmentTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private string defaultDiscount;
        private string defaultDiscountName;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Parse meterConsumed safely
            int meterConsumed = 0;
            int.TryParse(meterConsumedReadingTextBox.Text?.Trim(), out meterConsumed);

            // Parse serviceId safely
            if (int.TryParse(serviceIDLabel.Text?.Trim(), out int serviceId))
            {
                if (checkBox1.Checked)
                {
                    discountedPercentLabel.Text = "100%";
                    discountNameLabel.Text = "FREE WATER";
                }
                else
                {
                    discountedPercentLabel.Text = defaultDiscount;
                    discountNameLabel.Text = defaultDiscountName;
                }

                // Always call Populate after setting discount
                PopulateServiceRateLabels(serviceId, meterConsumed);
            }
        }

        private void bankNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkNumberTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void penaltyAmountLabel2_Click(object sender, EventArgs e)
        {

        }
    }
}