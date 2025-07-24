using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class ReportHelper
    {
        private static readonly string connectionString = DbConfig.ConnectionString;

        /// <summary>
        /// Executes a SQL query and returns a DataTable with the specified name.
        /// </summary>
        /// <param name="sqlQuery">The SQL SELECT query to execute.</param>
        /// <param name="dataTableName">The name of the resulting DataTable (used for Excel sheet name).</param>
        /// <returns>A DataTable containing the query results.</returns>
        public static DataTable GetDataTable(string sqlQuery, string dataTableName)
        {
            DataTable table = new DataTable(dataTableName);

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log error or rethrow
                throw new Exception("❌ Error fetching data: " + ex.Message);
            }

            return table;
        }

        /// <summary>
        /// Exports one or more DataTables to an Excel file.
        /// Each DataTable is written to a separate sheet.
        /// </summary>
        /// <param name="filePath">The full path to save the Excel file.</param>
        /// <param name="tables">One or more DataTables to export.</param>
        public static void ExportToExcel(string filePath, params DataTable[] tables)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    foreach (var table in tables)
                    {
                        var sheet = workbook.Worksheets.Add(table, table.TableName);

                        // 🔹 Format header row
                        var headerRow = sheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        // 🔹 Auto-size columns
                        sheet.Columns().AdjustToContents();

                        // 🔹 Freeze header
                        sheet.SheetView.FreezeRows(1);

                        // 🔹 Optionally: format numeric columns (ex. ₱ currency)
                        foreach (var column in table.Columns.Cast<DataColumn>())
                        {
                            if (column.DataType == typeof(decimal) || column.DataType == typeof(double))
                            {
                                int colIndex = table.Columns.IndexOf(column) + 1;
                                sheet.Column(colIndex).Style.NumberFormat.Format = "#,##0.00";
                            }
                        }
                    }

                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("❌ Failed to export Excel file: " + ex.Message);
            }
        }
    }
}
