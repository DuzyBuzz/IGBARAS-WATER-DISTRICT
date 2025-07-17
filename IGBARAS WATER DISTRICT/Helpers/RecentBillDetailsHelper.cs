using MySql.Data.MySqlClient;
using System;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class RecentBillDetailsHelper
    {
        // Internal class to return reading info
        public class BillReadingInfo
        {
            public int PreviousReading { get; set; }
            public int MeterConsumed { get; set; }
            public DateTime ReadingDate { get; set; }
            public float Balance { get; set; }
        }

        /// <summary>
        /// Retrieves present reading, to reading date from tb_bill for a specific bill_id.
        /// </summary>
        /// <param name="billId">The bill_id to look up.</param>
        /// <returns>A BillReadingInfo object if found; otherwise, null.</returns>
        public static BillReadingInfo GetReadingInfoByBillId(string billId)
        {
            BillReadingInfo readingInfo = null;

            string query = @"
                SELECT presentreading, toreadingdate, meterconsumed, balance
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
                                    PreviousReading = reader.GetInt32("presentreading"),
                                    MeterConsumed = reader.GetInt32("meterconsumed"),
                                    ReadingDate = reader.GetDateTime("toreadingdate"),
                                    Balance = reader.GetFloat("balance"),
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
