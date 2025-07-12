using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class BillingControl : UserControl
    {
        private Bitmap billingPanelImage;
        private BillHelper billHelper = new BillHelper();
        public BillingControl()
        {
            InitializeComponent();
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private async void BillingControl_Load(object sender, EventArgs e)
        {
            PlaceholderHelper.AddPlaceholder(searchBillTextBox, "🔎 Search Account #");
            filterButton.Text = "Loading...";
            filterButton.Enabled = false;

            await billHelper.LoadAllBillsAsync(); // only 1 time, async
            billDataGridView.DataSource = billHelper.AllBills; // initial load

            filterButton.Text = "Filter";
            filterButton.Enabled = true;
            // Set auto-complete for account number
            AutoCompleteHelper.FillTextBoxWithColumn("v_billing_summary", "accountno", searchBillTextBox);
        }
        private void FilterBillsInMemory(DateTime fromDate, DateTime toDate)
        {
            if (toDate < fromDate)
            {
                MessageBox.Show("To Date cannot be earlier than From Date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataView view = new DataView(billHelper.AllBills);
            view.RowFilter = $"datebilled >= #{fromDate:MM/dd/yyyy}# AND datebilled <= #{toDate:MM/dd/yyyy}#";

            billDataGridView.DataSource = view;
        }
        private void SearchByAccountNo(string accountNo)
        {

        }


        private void fromDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
        }

        private void toDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void ApplyCombinedFilter(string accountNo, DateTime fromDate, DateTime toDate)
        {
            DataView view = new DataView(billHelper.AllBills);

            string filter = $"accountno LIKE '%{accountNo.Replace("'", "''")}%' " +
                            $"AND datebilled >= '#{fromDate:yyyy-MM-dd}#' AND datebilled <= '#{toDate:yyyy-MM-dd}#'";

            view.RowFilter = filter;

            billDataGridView.DataSource = view;
        }


        private void clearDateButton_Click(object sender, EventArgs e)
        {
            // Reset to show all data
            billDataGridView.DataSource = billHelper.AllBills;

            // Optional: reset date pickers
            fromDateTimePicker.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            toDateTimePicker.Value = DateTime.Today;
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            DateTime fromDate = fromDateTimePicker.Value;
            DateTime toDate = toDateTimePicker.Value;

            if (toDate < fromDate)
            {
                MessageBox.Show("To Date cannot be earlier than From Date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🔍 Apply date filter using DataView
            DataView view = new DataView(billHelper.AllBills);
            view.RowFilter = $"datebilled >= '#{fromDate:yyyy-MM-dd}#' AND datebilled <= '#{toDate:yyyy-MM-dd}#'";

            billDataGridView.DataSource = view;
        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void searchBillTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            ApplyCombinedFilter(searchBillTextBox.Text, fromDateTimePicker.Value, toDateTimePicker.Value);
        }
    }
}
