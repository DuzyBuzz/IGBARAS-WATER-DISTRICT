using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class DGVHelper
    {
        /// <summary>
        /// Loads data into a DataGridView with optional multi-column filtering and loading form support.
        /// </summary>
        /// <param name="dgv">The DataGridView to load data into</param>
        /// <param name="tableName">Table or view name</param>
        /// <param name="parentForm">Parent form to block UI during loading (optional)</param>
        /// <param name="loadingForm">Loading form to show while fetching data (optional)</param>
        /// <param name="filterColumns">Array of column names to filter on (optional)</param>
        /// <param name="filterValues">Array of values corresponding to filterColumns (optional)</param>
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

                await Task.Run(() =>
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
                                // Use LIKE for partial match
                                conditions.Add($"`{filterColumns[i]}` LIKE @val{i}");
                            }

                            // Join with OR instead of AND
                            query += " WHERE " + string.Join(" OR ", conditions);
                        }

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            if (filterColumns != null && filterValues != null && filterColumns.Length == filterValues.Length)
                            {
                                for (int i = 0; i < filterColumns.Length; i++)
                                {
                                    cmd.Parameters.AddWithValue($"@val{i}", "%" + filterValues[i].ToString() + "%");
                                }
                            }

                            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);

                                dgv.Invoke((MethodInvoker)(() =>
                                {
                                    dgv.DataSource = dt;
                                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                                }));
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (loadingForm != null) loadingForm.Close();
            }
        }

    }
}