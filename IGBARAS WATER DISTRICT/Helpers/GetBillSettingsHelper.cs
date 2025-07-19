using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class GetBillSettingsHelper
    {
        /// <summary>
        /// Gets the value of a specific column from the first row of the tb_billsettings table.
        /// </summary>
        /// <param name="columnName">The column name to fetch (e.g., "duedateduration", "taxpercent").</param>
        /// <returns>Returns the value as a string. Returns empty string if not found or error.</returns>
        public static string GetValue(string columnName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // Only select the specified column from the first row
                    string query = $"SELECT `{columnName}` FROM tb_billsettings LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();

                        return result?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error reading bill settings column '{columnName}':\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }
    }
}
