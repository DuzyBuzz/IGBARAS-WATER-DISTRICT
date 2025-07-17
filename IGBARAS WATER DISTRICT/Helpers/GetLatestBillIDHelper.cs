using MySql.Data.MySqlClient;
using System;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class GetLatestBillIDHelper
    {
        /// <summary>
        /// Retrieves the latest bill_id from tb_bill table for a given account number.
        /// Uses DBConfig.ConnectionString.
        /// </summary>
        /// <param name="accountNo">The account number (e.g., "1022-c").</param>
        /// <returns>The latest bill_id as a string. Returns null if not found or error.</returns>
        public static string GetLatestBillId(string accountNo)
        {
            string latestBillId = null;

            // ✅ SQL to get the highest bill_id for a specific account number
            string query = @"
                SELECT bill_id 
                FROM tb_bill 
                WHERE accountno = @accountno 
                ORDER BY bill_id DESC 
                LIMIT 1";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // 🛡️ Prevent SQL injection with parameter
                        cmd.Parameters.AddWithValue("@accountno", accountNo);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            latestBillId = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error in GetLatestBillId: " + ex.Message);
            }

            return latestBillId;
        }
    }
}
