using MySql.Data.MySqlClient;
using System;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class RecentBillDetailsHelper
    {
        // Data class to return billing details
        public class BillReadingInfo
        {

            public DateTime FromReadingDate { get; set; }
            public DateTime ToReadingDate { get; set; }
            public int PreviousReading { get; set; }
            public int PresentReading { get; set; }
            public int MeterConsumed { get; set; }

            public DateTime DueDate { get; set; }
            public DateTime DateBilled { get; set; }
            public decimal ArrearsAmount { get; set; }
            public int Paid { get; set; }


            public int WithHoldingTaxPercent { get; set; }
            public decimal PenaltyAmount { get; set; }
            public decimal WithHoldingTaxAmount { get; set; }
            public decimal AditionalBillChargeAmount { get; set; }
            public int Arrears { get; set; }
            public decimal Balance { get; set; }

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
                    fromreadingdate, 
                    toreadingdate, 
                    previousreading, 
                    presentreading, 
                    meterconsumed,
                    duedate,
                    datebilled, 
                    arrearsamount,
                    paid,
                    penaltyamount,
                    wtamount,
                    totaladditionalcharge,
                    wtpercent,
                    arrears,
                    balance     
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
                                    FromReadingDate = reader["fromreadingdate"] != DBNull.Value ? Convert.ToDateTime(reader["fromreadingdate"]) : DateTime.MinValue,
                                    ToReadingDate = reader["toreadingdate"] != DBNull.Value ? Convert.ToDateTime(reader["toreadingdate"]) : DateTime.MinValue,

                                    PreviousReading = reader["previousreading"] != DBNull.Value ? Convert.ToInt32(reader["previousreading"]) : 0,
                                    PresentReading = reader["presentreading"] != DBNull.Value ? Convert.ToInt32(reader["presentreading"]) : 0,
                                    MeterConsumed = reader["meterconsumed"] != DBNull.Value ? Convert.ToInt32(reader["meterconsumed"]) : 0,

                                    DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.MinValue,
                                    DateBilled = reader["datebilled"] != DBNull.Value ? Convert.ToDateTime(reader["datebilled"]) : DateTime.MinValue,

                                    ArrearsAmount = reader["arrearsamount"] != DBNull.Value ? Convert.ToDecimal(reader["arrearsamount"]) : 0m,
                                    Paid = reader["paid"] != DBNull.Value ? Convert.ToInt32(reader["paid"]) : 0,
                                    WithHoldingTaxPercent = reader["wtpercent"] != DBNull.Value ? Convert.ToInt32(reader["wtpercent"]) : 0,
                                    AditionalBillChargeAmount = reader["paid"] != DBNull.Value ? Convert.ToDecimal(reader["paid"]) : 0,

                                    PenaltyAmount = reader["penaltyamount"] != DBNull.Value ? Convert.ToDecimal(reader["penaltyamount"]) : 0m,
                                    Arrears = reader["arrears"] != DBNull.Value ? Convert.ToInt32(reader["arrears"]) : 0,
                                    WithHoldingTaxAmount = reader["wtamount"] != DBNull.Value ? Convert.ToInt32(reader["wtamount"]) : 0,
                                    Balance = reader["balance"] != DBNull.Value ? Convert.ToDecimal(reader["balance"]) : 0m







                                    // for the billing 

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