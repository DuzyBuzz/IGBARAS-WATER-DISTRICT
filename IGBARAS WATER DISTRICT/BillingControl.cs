using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class BillingControl : UserControl
    {
        private Bitmap billingPanelImage;
        private PaginationHelper pager = new PaginationHelper();
        private int currentCopyIndex = 0;
        private readonly string[] copyNames = { "Concessionaire's Copy", "Records Copy", "File Copy" };
        public BillingControl()
        {
            InitializeComponent();


        }
        private void SetDateNow()
        {
            // Set the current date and time to the label
            dateBilledLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            dateIssuedLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            dueDateLbael.Text = DateTime.Now.AddDays(14).ToString("MMMM dd, yyyy");

        }
        /// <summary>
        /// Extracts the zone code from the start of the account number and formats it into a 3-digit string.
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
        public int GetZoneStartRange(string zoneCode)
        {
            string query = "SELECT start_range FROM tb_zone WHERE zonecode = @zonecode LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@zonecode", zoneCode);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);
                    else
                        return 1; // fallback if not found
                }
            }
        }
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


        public string GenerateNextBillCode(string zoneCode, DateTime billMonth)
        {
            int zoneNo = GetZoneNoFromDB(zoneCode);
            int baseStart = ((zoneNo - 1) * 200) + 1;
            int monthOffset = (billMonth.Month - 1) * 400;
            int startingNumber = baseStart + monthOffset;

            // Extract zone prefix (e.g. "001", "002", etc.)
            string prefix = zoneCode; // already in "003" format


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

            using (var loadingForm = new LoadingForm())
            {
                Form parentForm = this.FindForm(); // 🟢 Converts the parent to a Form
                await DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_account_detailes", parentForm, loadingForm);
            }

            AutoCompleteHelper.FillTextBoxWithColumn("v_account_detailes", "accountno", searchAccountNumberTextBox);
            SetDateNow();

        }



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

        private void accountDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = accountDataGridView.Rows[e.RowIndex];
                string accountNo = selectedRow.Cells["accountno"].Value?.ToString();
                accountNumberTextBox.Text = accountNo;
                accountnoBillHistory.Text = $"Account ID: {accountNo}";

                if (!string.IsNullOrWhiteSpace(accountNo))
                {
                    LoadAccountBillHistory(accountNo);

                    // 🔧 Manually update the invoice label here:
                    string zonePrefix = GetZonePrefixFromAccountNo(accountNo);
                    string nextBillCode = GenerateNextBillCode(zonePrefix, DateTime.Now);
                    string[] parts = nextBillCode.Split('-');
                    if (parts.Length == 2)
                    {
                        invoiceLabel.Text = parts[1];
                    }
                }
            }
        }


        private void LoadAccountBillHistory(string accountNo)
        {
            // Call helper to load billing summary rows where accountno = accountNo
            DataTable billData = ExclusiveDGVHelper.LoadRowsByExactAccount("v_billing_summary", "accountno", accountNo);

            if (billData != null)
            {
                billDataGridView.DataSource = billData;
                billDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private async void searchButton_Click(object sender, EventArgs e)
        {
            string keyword = searchAccountNumberTextBox.Text.Trim();

            using (var loadingForm = new LoadingForm()) // ✅ Make sure this matches your form name
            {
                Form parentForm = this.FindForm(); // ✅ This gets the parent MainForm

                if (parentForm == null)
                {
                    MessageBox.Show("Parent form not found. Cannot continue.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    await DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_account_detailes", parentForm, loadingForm);
                }
                else
                {
                    await DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_account_detailes", parentForm, loadingForm, "accountno", keyword);
                }
            }
        }



        private void searchAccountNumberTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchAccountNumberTextBox.Text) && searchAccountNumberTextBox.TextLength < 3)

            {
                clearButton.Enabled = false;
            }
            else
            {
                clearButton.Enabled = true;
            }
        }

        private async void clearButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchAccountNumberTextBox.Text))
            {
                using (var loadingForm = new LoadingForm())
                {
                    Form parentForm = this.FindForm(); // 🟢 Converts the parent to a Form
                    await DGVHelper.LoadDataToGridAsync(accountDataGridView, "v_account_detailes", parentForm, loadingForm);
                }
            }
            searchAccountNumberTextBox.Text = string.Empty;
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

        private void label32_Click(object sender, EventArgs e)
        {

        }
    }
}
