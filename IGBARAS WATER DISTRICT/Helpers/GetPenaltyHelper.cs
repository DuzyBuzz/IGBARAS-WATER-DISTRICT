using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class GetPenaltyHelper
    {
        // Replace with your actual MySQL connection string

        /// <summary>
        /// Calls the GETPENALTY() MySQL function and returns the calculated penalty.
        /// </summary>
        /// <param name="billCharge">The bill charge amount.</param>
        /// <param name="dueGracePeriod">The due date including grace period.</param>
        /// <param name="dueExempt">Whether the account is due-exempt (0 = no, 1 = yes).</param>
        /// <param name="arrears">Whether there are arrears (1 = yes, 0 = no).</param>
        /// <param name="srcPenalty">Existing penalty amount, if any.</param>
        /// <param name="srcPaid">Whether the bill is already paid (1 = yes, 0 = no).</param>
        /// <returns>The calculated penalty as a decimal.</returns>
        public static decimal GetPenalty(
            decimal billCharge,
            DateTime dueGracePeriod,
            int dueExempt,
            int arrears,
            decimal srcPenalty,
            int srcPaid)
        {
            decimal penaltyResult = 0;

            // MySQL SELECT function query
            string query = @"SELECT GETPENALTY(
                                @billCharge, 
                                @dueGracePeriod, 
                                @dueExempt, 
                                @arrears, 
                                @srcPenalty, 
                                @srcPaid
                             );";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Safely add all parameters
                        cmd.Parameters.AddWithValue("@billCharge", billCharge);
                        cmd.Parameters.AddWithValue("@dueGracePeriod", dueGracePeriod.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@dueExempt", dueExempt);
                        cmd.Parameters.AddWithValue("@arrears", arrears);
                        cmd.Parameters.AddWithValue("@srcPenalty", srcPenalty);
                        cmd.Parameters.AddWithValue("@srcPaid", srcPaid);

                        object result = cmd.ExecuteScalar();

                        if (result != null && decimal.TryParse(result.ToString(), out decimal parsedPenalty))
                        {
                            penaltyResult = parsedPenalty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // You can log the error or rethrow depending on your project’s error handling policy
                Console.WriteLine($"Error in GetPenaltyHelper: {ex.Message}");
            }

            return penaltyResult;
        }
    }
}