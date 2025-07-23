using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Data;
using System.Drawing;
using System.IO;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class ExcelChartExportHelper
    {
        public static void ExportMultiReportToExcel(DataSet reportSet, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();

            foreach (DataTable table in reportSet.Tables)
            {
                string sheetName = table.TableName;
                var sheet = package.Workbook.Worksheets.Add(sheetName);

                // 📊 Load data
                sheet.Cells["A1"].LoadFromDataTable(table, true);
                sheet.Cells.AutoFitColumns();

                // 🎨 Style headers
                using (var header = sheet.Cells[1, 1, 1, table.Columns.Count])
                {
                    header.Style.Font.Bold = true;
                    header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    header.Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                }

                // 🎯 Create chart if numeric columns exist
                var chart = sheet.Drawings.AddChart($"chart_{sheetName}", eChartType.ColumnClustered) as ExcelBarChart;
                chart.Title.Text = sheetName;
                chart.SetSize(700, 400);
                chart.SetPosition(1, 0, table.Columns.Count + 1, 0);
                chart.YAxis.Title.Text = "₱ Amount";
                chart.XAxis.Title.Text = table.Columns[0].ColumnName;

                int rowCount = table.Rows.Count;

                // 🚦 Try to add common financial fields
                TryAddSeries(chart, sheet, table, "Total Charges", "A", "C", rowCount);
                TryAddSeries(chart, sheet, table, "Total Paid", "A", "D", rowCount);
                TryAddSeries(chart, sheet, table, "Total Balance", "A", "E", rowCount);
                TryAddSeries(chart, sheet, table, "Total Collected", "A", "C", rowCount);
                TryAddSeries(chart, sheet, table, "Total Penalty Revenue", "A", "B", rowCount);
                TryAddSeries(chart, sheet, table, "amountpaid", "A", "B", rowCount);
                TryAddSeries(chart, sheet, table, "balance", "A", "C", rowCount);
            }

            package.SaveAs(new FileInfo(filePath));
        }

        private static void TryAddSeries(ExcelBarChart chart, ExcelWorksheet sheet, DataTable table, string columnName, string xColumn, string yColumn, int rowCount)
        {
            if (table.Columns.Contains(columnName))
            {
                chart.Series.Add($"{yColumn}2:{yColumn}{rowCount + 1}", $"{xColumn}2:{xColumn}{rowCount + 1}")
                    .Header = columnName;
            }
        }
    }
}
