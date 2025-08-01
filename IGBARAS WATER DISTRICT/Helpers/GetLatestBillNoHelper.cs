using System;
using System.Data.OleDb;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class GetLatestBillNoHelper
    {
        /// <summary>
        /// Retrieves the latest bill_id from Tb_Billing table for a given account number (MS Access version).
        /// </summary>
        /// <param name="accountNo">The account number (e.g., "1022-c").</param>
        /// <returns>The latest bill_id as a string. Returns null if not found or error.</returns>
        public static string GetLatestBillNo(string accountNo)
        {
            string latestBillNo = null;

            string query = @"
                SELECT TOP 1 BillNo 
                FROM Tb_Billing 
                WHERE AccountNo = ? 
                ORDER BY BillNo DESC";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        // Use positional parameter (?) for Access
                        cmd.Parameters.AddWithValue("?", accountNo);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            latestBillNo = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error in GetlatestBillNo: " + ex.Message);
            }

            return latestBillNo;
        }
    }
}
