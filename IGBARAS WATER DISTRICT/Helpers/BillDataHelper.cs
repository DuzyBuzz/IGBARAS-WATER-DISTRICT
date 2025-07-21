using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class BillDataHelper
    {
        /// <summary>
        /// Loads the billing data from the tb_bill table into a DataTable.
        /// </summary>
        /// <param name="loadingForm">A loading form to show while retrieving data.</param>
        /// <returns>Task<DataTable> containing billing data</returns>
        public static async Task<DataTable> LoadBillingDataAsync(Form loadingForm)
        {
            DataTable dataTable = new DataTable();

            await Task.Run(() =>
            {
                try
                {
                    // Show the loading form while loading data
                    loadingForm.Invoke((Action)(() => loadingForm.Show()));

                    using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM tb_bill";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable); // Load all data into DataTable
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load billing data:\n\n" + ex.Message, "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Safely close the loading form on the UI thread
                    loadingForm.Invoke((Action)(() => loadingForm.Close()));
                }
            });

            return dataTable;
        }
    }
}
