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
        private string lastOrderByField = "bill_id";
        private int currentCopyIndex = 0;
        private readonly string[] copyNames = { "Concessionaire's Copy", "Records Copy", "File Copy" };
        public BillingControl()
        {
            InitializeComponent();


        }

        private async void BillingControl_Load(object sender, EventArgs e)
        {
            pager.SetPageSize(25); // default rows per page
            rowsNumberComboBox.SelectedIndex = 0; // default to 20 rows
            pager.SetTotalRecords(GetTotalBillCount());
            UpdatePageLabel();
            UpdatePageButtons();
            AutoCompleteHelper.FillTextBoxWithColumn("tb_bill", "accountno", searchBillTextBox);
            await RunWithLoadingAsync(() => LoadPagedBillsAsync());

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
            string query = "SELECT COUNT(*) FROM tb_bill";
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
        SELECT * FROM tb_bill
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
            billingPrintDocument.DefaultPageSettings.Landscape = true;
            billingPrintDocument.DefaultPageSettings.Margins = new Margins(3, 3, 3, 3);
            currentCopyIndex = 0; // Reset before printing

            if (billingPrintDialog.ShowDialog() == DialogResult.OK)
            {
                billingPrintDocument.Print();
            }
        }

        private void billingPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            billingPrintDocument.DefaultPageSettings.Landscape = true; // Ensure landscape for every page

            if (currentCopyIndex < copyNames.Length)
            {
                copyTypeLabel.Text = copyNames[currentCopyIndex];
                Bitmap panelImage = CapturePanel(billingPanel);

                float scale = Math.Min(
                    (float)e.MarginBounds.Width / panelImage.Width,
                    (float)e.MarginBounds.Height / panelImage.Height);

                int printWidth = (int)(panelImage.Width * scale);
                int printHeight = (int)(panelImage.Height * scale);

                int x = e.MarginBounds.Left + (e.MarginBounds.Width - printWidth) / 2;
                int y = e.MarginBounds.Top + (e.MarginBounds.Height - printHeight) / 2;

                e.Graphics.DrawImage(panelImage, new Rectangle(x, y, printWidth, printHeight));
                panelImage.Dispose();

                currentCopyIndex++;
                e.HasMorePages = currentCopyIndex < copyNames.Length;
            }
            else
            {
                e.HasMorePages = false;
            }
        }
        private Bitmap CapturePanel(Control panel)
        {
            Bitmap bmp = new Bitmap(panel.Width, panel.Height);
            panel.DrawToBitmap(bmp, new Rectangle(0, 0, panel.Width, panel.Height));
            return bmp;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
