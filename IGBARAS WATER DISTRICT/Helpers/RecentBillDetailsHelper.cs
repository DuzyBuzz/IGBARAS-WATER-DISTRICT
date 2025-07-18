using MySql.Data.MySqlClient;
using System;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class RecentBillDetailsHelper
    {
        // Data class to return billing details
        public class BillReadingInfo
        {
            public int PreviousReading { get; set; }
            public int MeterConsumed { get; set; }
            public DateTime FromReadingDate { get; set; }
            public DateTime ToReadingDate { get; set; }
            public DateTime DueDate { get; set; }
            public decimal Arrears { get; set; }
            public decimal Penalty { get; set; }
            public decimal AmountPaid { get; set; }
        }

        /// <summary>
        /// Retrieves present reading, to reading date, and billing info from tb_bill for a specific bill_id.
        /// </summary>
        /// <param name="billId">The bill_id to look up.</param>
        /// <returns>A BillReadingInfo object if found; otherwise, null.</returns>
        public static BillReadingInfo GetReadingInfoByBillId(string billId)
        {
            BillReadingInfo readingInfo = null;

            string query = @"
                SELECT 
                    presentreading, 
                    presentmeterconsumed, 
                    fromreadingdate, 
                    toreadingdate, 
                    duedate, 
                    balance, 
                    penaltyamount,
                    amountpaid
                FROM tb_bill 
                WHERE bill_id = @bill_id 
                LIMIT 1";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@bill_id", billId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                readingInfo = new BillReadingInfo
                                {
                                    PreviousReading = reader["presentreading"] != DBNull.Value ? Convert.ToInt32(reader["presentreading"]) : 0,
                                    MeterConsumed = reader["presentmeterconsumed"] != DBNull.Value ? Convert.ToInt32(reader["presentmeterconsumed"]) : 0,
                                    FromReadingDate = reader["fromreadingdate"] != DBNull.Value ? Convert.ToDateTime(reader["fromreadingdate"]) : DateTime.MinValue,
                                    ToReadingDate = reader["toreadingdate"] != DBNull.Value ? Convert.ToDateTime(reader["toreadingdate"]) : DateTime.MinValue,
                                    DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.MinValue,
                                    Arrears = reader["balance"] != DBNull.Value ? Convert.ToDecimal(reader["balance"]) : 0m,
                                    Penalty = reader["penaltyamount"] != DBNull.Value ? Convert.ToDecimal(reader["penaltyamount"]) : 0m,
                                    AmountPaid = reader["amountpaid"] != DBNull.Value ? Convert.ToDecimal(reader["amountpaid"]) : 0m
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error fetching bill reading info: " + ex.Message);
            }

            return readingInfo;
        }
    }
}
