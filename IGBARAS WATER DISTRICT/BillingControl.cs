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
        private string lastOrderByField = "concessionaireno";
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
        }
        private async void BillingControl_Load(object sender, EventArgs e)
        {
            pager.SetPageSize(25); // default rows per page
            rowsNumberComboBox.SelectedIndex = 0; // default to 20 rows
            pager.SetTotalRecords(GetTotalBillCount());
            UpdatePageLabel();
            UpdatePageButtons();
            AutoCompleteHelper.FillTextBoxWithColumn("v_concessionaire_summary", "accountno", searchBillTextBox);
            await RunWithLoadingAsync(() => LoadPagedBillsAsync());
            SetDateNow();
        }
        private async Task RunWithLoadingAsync(Func<Task> task)
        {
            try
            {
                ShowLoading();              // Show progress bar + label
                await Task.Delay(50);       // Let the UI render loading state
                await task();               // Run the async work
            }
            finally
            {
                HideLoading();              // Hide progress bar + label after done
            }
        }

        private void ShowLoading()
        {


            if (loadingLabel == null)
            {
                loadingLabel = new Label();
                loadingLabel.Text = "Loading...";
                loadingLabel.AutoSize = true;
                loadingLabel.Font = new Font("Segoe UI", 20, FontStyle.Italic);
                loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(loadingLabel);
            }



            loadingProgressBar.Visible = true;
            loadingLabel.Visible = true;
        }

        private void HideLoading()
        {
            loadingProgressBar.Visible = false;
            loadingLabel.Visible = false;
        }
        private int GetTotalBillCount()
        {
            string query = "SELECT COUNT(*) FROM v_concessionaire_summary";
            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private async Task LoadPagedBillsAsync(string filterKeyword = "")
        {
            bool isAll = rowsNumberComboBox.SelectedItem?.ToString().ToLower() == "all";

            string baseQuery = $@"
        SELECT * FROM v_concessionaire_summary
        {(string.IsNullOrWhiteSpace(filterKeyword) ? "" : "WHERE accountno LIKE @keyword")}
        ORDER BY {lastOrderByField} DESC
        {(isAll ? "" : $"LIMIT {pager.PageSize} OFFSET {pager.GetOffset()}")}";

            var parameters = string.IsNullOrWhiteSpace(filterKeyword)
                ? null
                : new MySqlParameter[] { new MySqlParameter("@keyword", $"%{filterKeyword}%") };

            billDataGridView.DataSource = await Task.Run(() => DGVHelper.LoadData(baseQuery, parameters));
        }



        private async void rowsNumberComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = rowsNumberComboBox.SelectedItem.ToString().ToLower();
            int size = selected == "all" ? -1 : int.Parse(selected);

            pager.SetPageSize(size);
            pager.Reset();

            await RunWithLoadingAsync(() => LoadPagedBillsAsync(searchBillTextBox.Text.Trim()));
            UpdatePageLabel();
            UpdatePageButtons();
        }


        private async void nextButton_Click(object sender, EventArgs e)
        {
            pager.NextPage();
            await RunWithLoadingAsync(() => LoadPagedBillsAsync(searchBillTextBox.Text.Trim()));
            UpdatePageLabel();
            UpdatePageButtons();
        }

        private async void prevButton_Click(object sender, EventArgs e)
        {
            pager.PreviousPage();
            await RunWithLoadingAsync(() => LoadPagedBillsAsync(searchBillTextBox.Text.Trim()));
            UpdatePageLabel();
            UpdatePageButtons();
        }


        private void UpdatePageLabel()
        {
            pageLabel.Text = pager.GetPageInfo();
        }

        private void UpdatePageButtons()
        {
            nextButton.Enabled = pager.CurrentPage < pager.TotalPages - 1;
            prevButton.Enabled = pager.CurrentPage > 0;
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


        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
