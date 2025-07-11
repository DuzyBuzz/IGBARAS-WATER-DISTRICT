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

        public BillingControl()
        {
            InitializeComponent();
        }

        private void label_Click(object sender, EventArgs e)
        {

        }

        private void BillingControl_Load(object sender, EventArgs e)
        {

        }

        private void billingPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel10_Paint(object sender, PaintEventArgs e)
        {
        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel14_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel10_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel18_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel20_Paint(object sender, PaintEventArgs e)
        {

        }
        private void tableLayoutPanel19_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel22_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void tableLayoutPanel22_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click_1(object sender, EventArgs e)
        {

        }

        private void label82_Click(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel19_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label83_Click(object sender, EventArgs e)
        {

        }

        private void printButton_Click(object sender, EventArgs e)
        {
            // Capture the billingPanel as a high-quality bitmap
            billingPanelImage = CapturePanel(billingPanel);

            // Set print document to landscape
            billingPrintDocument.DefaultPageSettings.Landscape = true;

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
            if (billingPanelImage == null)
                return;

            // Get the Letter paper size in landscape (in hundredths of an inch)
            // Letter: 8.5 x 11 inches, so landscape is 11 x 8.5
            PaperSize letterLandscape = new PaperSize("Letter", 1100, 850); // 1/100 inch units

            // Get the DPI of the printer
            float dpiX = e.Graphics.DpiX;
            float dpiY = e.Graphics.DpiY;

            // Calculate the printable area in pixels
            int printableWidth = (int)(e.PageBounds.Width * (dpiX / 100f));
            int printableHeight = (int)(e.PageBounds.Height * (dpiY / 100f));

            // Use MarginBounds for the actual printable area (excluding margins)
            Rectangle marginBounds = e.MarginBounds;

            // Scale the image to fit the margin bounds, maintaining aspect ratio
            float scale = Math.Min(
                (float)marginBounds.Width / billingPanelImage.Width,
                (float)marginBounds.Height / billingPanelImage.Height);

            int printWidth = (int)(billingPanelImage.Width * scale);
            int printHeight = (int)(billingPanelImage.Height * scale);

            // Center the image
            int x = marginBounds.Left + (marginBounds.Width - printWidth) / 2;
            int y = marginBounds.Top + (marginBounds.Height - printHeight) / 2;

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            e.Graphics.DrawImage(billingPanelImage, new Rectangle(x, y, printWidth, printHeight));
        }
    }
}
