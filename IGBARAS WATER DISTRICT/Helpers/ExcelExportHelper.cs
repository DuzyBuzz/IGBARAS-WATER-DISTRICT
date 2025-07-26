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
                // Create worksheet with the same name as the table
                var sheet = workbook.Worksheets.Add(table.TableName);

                // Insert the data as a table starting at cell A1 with headers
                var tableRange = sheet.Cell(1, 1).InsertTable(table, table.TableName, true);

                // Style the headers
                var headerRow = sheet.Range(1, 1, 1, table.Columns.Count);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Font.FontColor = XLColor.Black; // ✅ Correct usage of XLColor
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


                // Autofit columns
                sheet.Columns().AdjustToContents();

                // Identify the last row of data
                int lastDataRow = table.Rows.Count + 1; // +1 for the header row
                int totalRow = lastDataRow + 1;

                // Add "TOTAL" label in first column
                sheet.Cell(totalRow, 1).Value = "TOTAL";
                sheet.Cell(totalRow, 1).Style.Font.Bold = true;
                sheet.Cell(totalRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

                // Loop through each column to add SUM formulas for numeric types
                for (int col = 1; col <= table.Columns.Count; col++)
                {
                    DataColumn column = table.Columns[col - 1];

                    if (column.DataType == typeof(int) || column.DataType == typeof(decimal) || column.DataType == typeof(double))
                    {
                        string colLetter = XLHelper.GetColumnLetterFromNumber(col);
                        string sumFormula = $"=SUM({colLetter}2:{colLetter}{lastDataRow})";

                        var cell = sheet.Cell(totalRow, col);
                        cell.FormulaA1 = sumFormula;

                        // Format total cell
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        cell.Style.NumberFormat.Format = "#,##0.00"; // Format as currency/decimal
                    }
                }
            }

            // Save the Excel file
            workbook.SaveAs(filename);

            await Task.CompletedTask;
        }


    }
}
