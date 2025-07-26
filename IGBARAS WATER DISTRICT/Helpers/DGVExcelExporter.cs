using System;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal static class DGVExcelExporter
    {
        /// <summary>
        /// Exports the given DataGridView to an Excel file using ClosedXML.
        /// </summary>
        /// <param name="dgv">The DataGridView to export.</param>
        /// <param name="fileName">Suggested filename (without path). Example: "BillingReport.xlsx"</param>
        public static void ExportToExcel(DataGridView dgv, string fileName)
        {
            try
            {
                // Show SaveFileDialog to let the user choose location
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                    sfd.FileName = string.IsNullOrWhiteSpace(fileName)
                        ? "Export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx"
                        : fileName.EndsWith(".xlsx") ? fileName : fileName + ".xlsx";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Sheet1");

                        // Write column headers
                        for (int col = 0; col < dgv.Columns.Count; col++)
                        {
                            worksheet.Cell(1, col + 1).Value = dgv.Columns[col].HeaderText;
                            worksheet.Cell(1, col + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(1, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                        }

                        // Write data rows
                        for (int row = 0; row < dgv.Rows.Count; row++)
                        {
                            for (int col = 0; col < dgv.Columns.Count; col++)
                            {
                                object value = dgv.Rows[row].Cells[col].Value;
                                worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? string.Empty;
                            }
                        }

                        // Auto-fit columns
                        worksheet.Columns().AdjustToContents();

                        // Save file
                        workbook.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("✅ Export to Excel completed successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error exporting to Excel:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
