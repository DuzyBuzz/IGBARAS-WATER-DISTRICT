using ClosedXML.Excel;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class ExcelExportHelper
    {
        public static async Task ExportReportsToExcelAsync(DataSet reports, string filename)
        {
            using var workbook = new XLWorkbook();

            foreach (DataTable table in reports.Tables)
            {
                var sheet = workbook.Worksheets.Add(table.TableName);
                sheet.Cell(1, 1).InsertTable(table, table.TableName, true);

                // Beautify: AutoFit, Bold headers
                var headerRange = sheet.RangeUsed().Range(1, 1, 1, table.Columns.Count);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                sheet.Columns().AdjustToContents();
            }

            workbook.SaveAs(filename);
            await Task.CompletedTask;
        }
    }
}
