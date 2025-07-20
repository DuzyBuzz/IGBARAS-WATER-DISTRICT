using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class ExclusiveDGVHelper
    {
        /// <summary>
        /// Queries a specific table and returns only rows that exactly match the account number.
        /// </summary>
        /// <param name="tableName">The table to query (e.g., "tb_bill")</param>
        /// <param name="columnName">The column to match (e.g., "accountno")</param>
        /// <param name="accountNumber">The exact account number to filter (e.g., "01-1-12-016E")</param>
        /// <returns>A DataTable containing the matching rows, or null if there's an error.</returns>
        public static DataTable LoadRowsByExactAccount(string tableName, string columnName, string accountNumber)
        {
            try
            {
                string query = $"SELECT * FROM `{tableName}` WHERE `{columnName}` = @accountNo";

                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@accountNo", accountNumber);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable resultTable = new DataTable();
                            adapter.Fill(resultTable);
                            return resultTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error querying exclusive data:\n" + ex.Message,
                    "Query Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
