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

            public string BillCode { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public int WithHoldingTaxPercent { get; set; }
            public decimal PenaltyAmount { get; set; }
            public decimal TotalAditionalCharge { get; set; }
            public decimal WithHoldingTaxAmount { get; set; }
            public decimal TaxAmount { get; set; }
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
                    name,
                    address,
                    billcode, 
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
                    taxamount,
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
                                    Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : string.Empty,
                                    Address = reader["address"] != DBNull.Value ? reader["address"].ToString() : string.Empty,
                                    BillCode = reader["billcode"] != DBNull.Value ? reader["billcode"].ToString() : string.Empty,

                                    FromReadingDate = reader["fromreadingdate"] != DBNull.Value ? Convert.ToDateTime(reader["fromreadingdate"]) : DateTime.MinValue,
                                    ToReadingDate = reader["toreadingdate"] != DBNull.Value ? Convert.ToDateTime(reader["toreadingdate"]) : DateTime.MinValue,

                                    PreviousReading = reader["previousreading"] != DBNull.Value ? Convert.ToInt32(reader["previousreading"]) : 0,
                                    PresentReading = reader["presentreading"] != DBNull.Value ? Convert.ToInt32(reader["presentreading"]) : 0,
                                    MeterConsumed = reader["meterconsumed"] != DBNull.Value ? Convert.ToInt32(Math.Floor(Convert.ToDouble(reader["meterconsumed"]))) : 0,

                                    DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.MinValue,
                                    DateBilled = reader["datebilled"] != DBNull.Value ? Convert.ToDateTime(reader["datebilled"]) : DateTime.MinValue,

                                    ArrearsAmount = reader["arrearsamount"] != DBNull.Value ? Convert.ToDecimal(reader["arrearsamount"]) : 0m,
                                    Paid = reader["paid"] != DBNull.Value ? Convert.ToInt32(reader["paid"]) : 0,
                                    WithHoldingTaxPercent = reader["wtpercent"] != DBNull.Value ? Convert.ToInt32(reader["wtpercent"]) : 0,
                                    TotalAditionalCharge = reader["totaladditionalcharge"] != DBNull.Value ? Convert.ToDecimal(reader["totaladditionalcharge"]) : 0m,

                                    TaxAmount = reader["taxamount"] != DBNull.Value ? Convert.ToDecimal(reader["taxamount"]) : 0m,
                                    PenaltyAmount = reader["penaltyamount"] != DBNull.Value ? Convert.ToDecimal(reader["penaltyamount"]) : 0m,
                                    Arrears = reader["arrears"] != DBNull.Value ? Convert.ToInt32(reader["arrears"]) : 0,
                                    Balance = reader["balance"] != DBNull.Value ? Convert.ToDecimal(reader["balance"]) : 0m,

                                    WithHoldingTaxAmount = reader["wtamount"] != DBNull.Value ? Convert.ToDecimal(reader["wtamount"]) : 0m,
                                    // If you need AditionalBillChargeAmount, add it to the query and assign here
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