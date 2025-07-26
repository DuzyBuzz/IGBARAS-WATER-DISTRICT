using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class PrinterService
    {
        /// <summary>
        /// Reusable method to print a DataGridView with professional header/footer.
        /// </summary>
        /// <param name="dgv">The DataGridView to print</param>
        /// <param name="reportTitle">The title of the report</param>
        public static void PrintDataGridView(DataGridView dgv, string reportTitle)
        {
            // Save full DataGridView visual state
            var originalHeaderStyle = dgv.ColumnHeadersDefaultCellStyle.Clone();
            var originalCellStyle = dgv.DefaultCellStyle.Clone();
            var originalAutoSizeMode = dgv.AutoSizeColumnsMode;
            var originalRowHeight = dgv.RowTemplate.Height;

            // Save column settings individually
            var originalColumnSettings = dgv.Columns
                .Cast<DataGridViewColumn>()
                .Select(col => new
                {
                    col.Index,
                    col.AutoSizeMode,
                    col.MinimumWidth,
                    col.Width
                })
                .ToList();

            try
            {
                DGVPrinter printer = new DGVPrinter();

                printer.PageSettings.Landscape = true;



                // Subtitle
                printer.SubTitle = string.Join(Environment.NewLine, new[]
                {
            "Republic of the Philippines",
            "IGBARAS WATER DISTRICT (ILOILO)",
            "Stall No. 0-1, Igbaras Bus. Complex",
            "M. Ezpeleta St., Igbaras, Iloilo",
            "Tel No. (033)315-6264",
            "NON-VAT Reg.",
            "TIN: 006-231-718.000",
            "                       "
        });
                printer.SubTitleFont = new Font("Arial", 9, FontStyle.Regular);
                printer.SubTitleAlignment = StringAlignment.Center;
                printer.SubTitleSpacing = 10;
                // Title
                printer.Title = reportTitle.ToUpper();
                printer.TitleFont = new Font("Arial", 11, FontStyle.Bold);
                printer.TitleAlignment = StringAlignment.Center;

                printer.PageNumbers = true;
                printer.PageNumberInHeader = false;
                printer.PorportionalColumns = true;
                printer.HeaderCellAlignment = StringAlignment.Center;
                printer.CellAlignment = StringAlignment.Center;

                // Set print formatting
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 6, FontStyle.Bold);
                dgv.DefaultCellStyle.Font = new Font("Arial", 6, FontStyle.Regular);
                dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.RowTemplate.Height = 16;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.MinimumWidth = 15;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                // Footer
                printer.Footer = $"Date: {DateTime.Now:MMMM dd, yyyy}";
                printer.FooterFont = new Font("Arial", 7, FontStyle.Italic);
                printer.FooterAlignment = StringAlignment.Near;
                printer.FooterSpacing = 15;

                // Print Preview
                printer.PrintPreviewDataGridView(dgv);
            }
            finally
            {
                // Restore visual styles
                dgv.ColumnHeadersDefaultCellStyle = originalHeaderStyle;
                dgv.DefaultCellStyle = originalCellStyle;
                dgv.AutoSizeColumnsMode = originalAutoSizeMode;
                dgv.RowTemplate.Height = originalRowHeight;

                // Restore individual column settings
                foreach (var setting in originalColumnSettings)
                {
                    var col = dgv.Columns[setting.Index];
                    col.AutoSizeMode = setting.AutoSizeMode;
                    col.MinimumWidth = setting.MinimumWidth;
                    col.Width = setting.Width;
                }
            }
        }




        /// <summary>
        /// Print a DataGridView directly to the printer (no preview).
        /// </summary>
        /// <param name="dgv">The DataGridView to print</param>
        /// <param name="reportTitle">The title of the report</param>
        public static void PrintDataGridViewDirect(DataGridView dgv, string reportTitle)
        {
            DGVPrinter printer = new DGVPrinter();

            // Correcting the error by removing the invalid property 'DefaultPageSettings'
            // Setting Landscape mode using PageSettings property
            printer.PageSettings.Landscape = true;

            // Main report title
            printer.SubTitle = reportTitle.ToUpper();
            printer.TitleFont = new Font("Arial", 11, FontStyle.Bold);
            printer.TitleAlignment = StringAlignment.Center;

            // Water District details as subtitle
            printer.Title = string.Join(Environment.NewLine, new[]
            {
                    "Republic of the Philippines",
                    "IGBARAS WATER DISTRICT (ILOILO)",
                    "Stall No. 0-1, Igbaras Bus. Complex",
                    "M. Ezpeleta St., Igbaras, Iloilo",
                    "Tel No. (033)315-6264",
                    "NON-VAT Reg.",
                    "TIN: 006-231-718.000",
                    "                       "
                });
            printer.SubTitleAlignment = StringAlignment.Center;
            printer.SubTitleSpacing = 10;
            // Layout settings
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Center;
            printer.CellAlignment = StringAlignment.Near;

            // Footer
            printer.Footer = $"Date: {DateTime.Now:MMMM dd, yyyy} ";
            printer.FooterAlignment = StringAlignment.Near;
            printer.FooterSpacing = 15;

            // Auto-size columns
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Print directly (no preview)
            printer.PrintDataGridView(dgv);
        }
    }
}
