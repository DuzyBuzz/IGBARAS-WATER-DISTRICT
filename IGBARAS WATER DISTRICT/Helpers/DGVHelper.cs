using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

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
                if (loadingForm != null)
                {
                    loadingForm.Show();
                    loadingForm.Refresh();
                }

                // 🔵 Start stopwatch to measure query time
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // 🔄 Run DB logic on background thread
                DataTable dt = await Task.Run(() =>
                {
                    using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                    {
                        conn.Open();

                        string query = $"SELECT * FROM `{tableName}`";

                        if (filterColumns != null && filterValues != null && filterColumns.Length == filterValues.Length)
                        {
                            List<string> conditions = new List<string>();
                            for (int i = 0; i < filterColumns.Length; i++)
                            {
                                conditions.Add($"`{filterColumns[i]}` LIKE @val{i}");
                            }

                            query += " WHERE " + string.Join(" OR ", conditions);
                        }

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            if (filterColumns != null && filterValues != null && filterColumns.Length == filterValues.Length)
                            {
                                for (int i = 0; i < filterColumns.Length; i++)
                                {
                                    cmd.Parameters.AddWithValue($"@val{i}", $"%{filterValues[i]}%");
                                }
                            }

                            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                            {
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);

                                // 🔵 Log raw query time
                                sw.Stop();
                                Debug.WriteLine($"[SQL Query Time] Fetched {dataTable.Rows.Count} rows from `{tableName}` in {sw.ElapsedMilliseconds} ms.");

                                return dataTable;
                            }
                        }
                    }
                });

                // 🔴 Start stopwatch for UI binding
                sw.Restart();

                if (dgv.IsHandleCreated)
                {
                    dgv.Invoke((MethodInvoker)(() =>
                    {
                        dgv.DataSource = dt;
                        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                        // 🔴 Log UI binding time
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
                if (loadingForm != null)
                {
                    loadingForm.Close();
                }
            }
        }

    }
}
