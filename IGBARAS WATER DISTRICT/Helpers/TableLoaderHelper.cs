using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal static class TableLoaderHelper
    {
        public static void LoadTableToGrid(DataGridView dgv, string tableName)
        {
            try
            {
                using (var connection = new OleDbConnection(DbConfig.ConnectionString))
                {
                    string query = $"SELECT * FROM {tableName}";
                    var adapter = new OleDbDataAdapter(query, connection);
                    var dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    dgv.DataSource = dataTable;

                    // Optional: Auto-resize columns
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgv.ReadOnly = false;
                    dgv.AllowUserToAddRows = true;
                    dgv.AllowUserToDeleteRows = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load table '{tableName}'.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void LoadTransposedTableToGrid(DataGridView dgv, string tableName)
        {
            using (var connection = new OleDbConnection(DbConfig.ConnectionString))
            {
                string query = $"SELECT * FROM {tableName}";
                var adapter = new OleDbDataAdapter(query, connection);
                var originalTable = new DataTable();
                adapter.Fill(originalTable);

                if (originalTable.Rows.Count == 0) return;

                var transposed = new DataTable();

                // First column for column names
                transposed.Columns.Add("Field");

                // Add one column per data row (Row 1, Row 2, etc.)
                for (int rowIndex = 0; rowIndex < originalTable.Rows.Count; rowIndex++)
                {
                    transposed.Columns.Add($"Row {rowIndex + 1}");
                }

                // Add each column as a row
                foreach (DataColumn column in originalTable.Columns)
                {
                    var newRow = transposed.NewRow();
                    newRow[0] = column.ColumnName;

                    for (int i = 0; i < originalTable.Rows.Count; i++)
                    {
                        newRow[i + 1] = originalTable.Rows[i][column.ColumnName];
                    }

                    transposed.Rows.Add(newRow);
                }

                dgv.DataSource = transposed;
                dgv.RowHeadersVisible = false;
                dgv.ReadOnly = true;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }
    }

}
