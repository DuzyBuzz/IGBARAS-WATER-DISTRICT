using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

public static class ReportExportHelper
{
    public static void ExportReportsToExcel(Dictionary<string, DataTable> reports, string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage package = new ExcelPackage())
        {
            foreach (var report in reports)
            {
                var sheet = package.Workbook.Worksheets.Add(report.Key);

                // Load data
                sheet.Cells["A1"].LoadFromDataTable(report.Value, true);

                // Format header
                using (var range = sheet.Cells[1, 1, 1, report.Value.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                }

                // Format money columns
                for (int col = 1; col <= report.Value.Columns.Count; col++)
                {
                    string colName = report.Value.Columns[col - 1].ColumnName.ToLower();
                    if (colName.Contains("amount") || colName.Contains("paid") || colName.Contains("balance") || colName.Contains("charge") || colName.Contains("total"))
                    {
                        sheet.Column(col).Style.Numberformat.Format = "₱#,##0.00";
                    }
                }

                // Auto-fit
                sheet.Cells.AutoFitColumns();
            }

            package.SaveAs(new FileInfo(filePath));
        }
    }
}
