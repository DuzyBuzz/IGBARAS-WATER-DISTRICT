using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class DGVHelper
    {
        /// <summary>
        /// Loads data into a DataGridView with optional filtering and loading form support.
        /// </summary>
        public static async Task LoadDataToGridAsync(
            DataGridView dgv,
            string tableName,
            Form parentForm = null,
            Form loadingForm = null,
            string filterColumn = "",
            object filterValue = null)
        {
            try
            {
                // Show loading form if provided
                if (loadingForm != null)
                {
                    loadingForm.Show();
                    loadingForm.Refresh(); // Force paint immediately
                }

                // Disable parent UI (optional)
                if (parentForm != null) parentForm.Enabled = false;

                await Task.Run(() =>
                {
                    using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                    {
                        conn.Open();

                        string query = $"SELECT * FROM `{tableName}`";
                        if (!string.IsNullOrWhiteSpace(filterColumn) && filterValue != null)
                        {
                            query += $" WHERE `{filterColumn}` = @filterValue";
                        }

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            if (!string.IsNullOrWhiteSpace(filterColumn) && filterValue != null)
                            {
                                cmd.Parameters.AddWithValue("@filterValue", filterValue);
                            }

                            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);

                                // Invoke back to the main thread to update the UI
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
                // Close loading form if shown
                if (loadingForm != null) loadingForm.Close();
                if (parentForm != null) parentForm.Enabled = true;
            }
        }
    }
}
