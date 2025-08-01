using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class DGVHelper
    {
        public static async Task LoadDataToGridAsync(
            DataGridView dgv,
            string tableName,
            Form loadingForm = null,
            string[] filterColumns = null,
            object[] filterValues = null)
        {
            try
            {
                loadingForm?.Show();
                loadingForm?.Refresh();

                Stopwatch sw = Stopwatch.StartNew();

                DataTable dt = await Task.Run(() =>
                {
                    using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
                    {
                        conn.Open();

                        string query = $"SELECT * FROM [{tableName}]";

                        if (filterColumns != null && filterValues != null && filterColumns.Length == filterValues.Length)
                        {
                            List<string> conditions = new List<string>();
                            for (int i = 0; i < filterColumns.Length; i++)
                            {
                                conditions.Add($"[{filterColumns[i]}] LIKE ?");
                            }

                            query += " WHERE " + string.Join(" OR ", conditions);
                        }

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            if (filterColumns != null && filterValues != null && filterColumns.Length == filterValues.Length)
                            {
                                for (int i = 0; i < filterColumns.Length; i++)
                                {
                                    cmd.Parameters.AddWithValue($"@val{i}", $"%{filterValues[i]}%");
                                }
                            }

                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                            {
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);

                                sw.Stop();
                                Debug.WriteLine($"[SQL Query Time] Fetched {dataTable.Rows.Count} rows from `{tableName}` in {sw.ElapsedMilliseconds} ms.");
                                return dataTable;
                            }
                        }
                    }
                });

                sw.Restart();

                if (dgv.IsHandleCreated)
                {
                    dgv.Invoke((MethodInvoker)(() =>
                    {
                        dgv.DataSource = dt;
                        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                        sw.Stop();
                        Debug.WriteLine($"[UI Bind Time] Data bound to DataGridView in {sw.ElapsedMilliseconds} ms.");
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingForm?.Close();
            }
        }

        public static async Task<DataTable> LoadDataToDataTableAsync(string tableName, Form loadingForm = null)
        {
            DataTable resultTable = new DataTable();

            try
            {
                loadingForm?.Show();
                loadingForm?.Refresh();

                Stopwatch sw = Stopwatch.StartNew();

                resultTable = await Task.Run(() =>
                {
                    using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
                    {
                        conn.Open();
                        string query = $"SELECT * FROM [{tableName}]";

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                });

                sw.Stop();
                Debug.WriteLine($"[SQL Query Time] Loaded {resultTable.Rows.Count} rows in {sw.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingForm?.Close();
            }

            return resultTable;
        }
    }
}
