using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class GetPenaltyHelper
    {
        /// <summary>
        /// Calls the MySQL function `GETPENALTY` to calculate the penalty based on billing parameters.
        /// </summary>
        /// <param name="billCharge">Total bill amount</param>
        /// <param name="dueGracePeriod">Grace period date</param>
        /// <param name="dueExempt">1 if due is exempted, else 0</param>
        /// <param name="arrears">1 if bill has arrears, else 0</param>
        /// <param name="existingPenalty">Existing penalty amount</param>
        /// <param name="isPaid">1 if bill is already paid, else 0</param>
        /// <returns>Penalty amount as decimal</returns>
        public static decimal GetPenalty(decimal billCharge, DateTime dueGracePeriod, int dueExempt, int arrears, decimal existingPenalty, int isPaid)
        {
            decimal penalty = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // Build SQL query to call the function
                    string query = "SELECT GETPENALTY(@billcharge, @duegraceperiod, @dueexempt, @arrears, @src_penalty, @src_paid);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add all parameters required by the function
                        cmd.Parameters.AddWithValue("@billcharge", billCharge);
                        cmd.Parameters.AddWithValue("@duegraceperiod", dueGracePeriod);
                        cmd.Parameters.AddWithValue("@dueexempt", dueExempt);
                        cmd.Parameters.AddWithValue("@arrears", arrears);
                        cmd.Parameters.AddWithValue("@src_penalty", existingPenalty);
                        cmd.Parameters.AddWithValue("@src_paid", isPaid);

                        // Execute the function and read the result
                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            penalty = Convert.ToDecimal(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or display in debug output
                Debug.WriteLine($"[GetPenaltyHelper] Error: {ex.Message}");
            }

            return penalty;
        }
    }
}
