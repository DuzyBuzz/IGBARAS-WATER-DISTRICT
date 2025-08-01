using System;
using System.Data;
using System.Data.OleDb;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class RecentBillDetailsHelper
    {
        public class BillingInfo
        {
            public string BillNo { get; set; }
            public string AccountNo { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public double PrevReading { get; set; }
            public double PresentReading { get; set; }
            public DateTime DueDate { get; set; }
            public DateTime DateCreated { get; set; }
            public double Penalty { get; set; }
            public double Tax { get; set; }
            public string DiscountName { get; set; }
            public double Discount { get; set; }
            public double OtherDiscount { get; set; }
            public double DiscountAmount { get; set; }
            public double Balance { get; set; }
            public double ServiceConnectionFee { get; set; }
            public string Others1 { get; set; }
            public double OthersAmount1 { get; set; }
            public string Others2 { get; set; }
            public double OthersAmount2 { get; set; }
            public double AmountPaid { get; set; }
            public bool IsArrears { get; set; }
            public bool IsFullyPaid { get; set; }
        }

        public BillingInfo GetBillByBillNo(string billNo)
        {
            if (string.IsNullOrWhiteSpace(billNo))
                return null;

            using (var connection = new OleDbConnection(DbConfig.ConnectionString))
            {
                const string query = @"
                    SELECT 
                        b.*, 
                        p.Balance
                    FROM 
                        Tb_Billing AS b
                    LEFT JOIN 
                        Tb_Payments AS p 
                        ON b.BillNo = p.CurrentBillNo
                    WHERE 
                        b.BillNo = ?
                ";

                using (var command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", billNo);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BillingInfo
                            {
                                BillNo = reader["BillNo"]?.ToString(),
                                AccountNo = reader["AccountNo"]?.ToString(),
                                DateFrom = reader["DateFrom"] != DBNull.Value ? Convert.ToDateTime(reader["DateFrom"]) : DateTime.MinValue,
                                DateTo = reader["DateTo"] != DBNull.Value ? Convert.ToDateTime(reader["DateTo"]) : DateTime.MinValue,
                                PrevReading = reader["PrevReading"] != DBNull.Value ? Convert.ToDouble(reader["PrevReading"]) : 0,
                                PresentReading = reader["PresentReading"] != DBNull.Value ? Convert.ToDouble(reader["PresentReading"]) : 0,
                                DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                DateCreated = reader["DateCreated"] != DBNull.Value ? Convert.ToDateTime(reader["DateCreated"]) : DateTime.MinValue,
                                Penalty = reader["Penalty"] != DBNull.Value ? Convert.ToDouble(reader["Penalty"]) : 0,
                                Tax = reader["Tax"] != DBNull.Value ? Convert.ToDouble(reader["Tax"]) : 0,
                                DiscountName = reader["DiscountName"]?.ToString(),
                                Discount = reader["Discount"] != DBNull.Value ? Convert.ToDouble(reader["Discount"]) : 0,
                                OtherDiscount = reader["OtherDiscount"] != DBNull.Value ? Convert.ToDouble(reader["OtherDiscount"]) : 0,
                                DiscountAmount = reader["DiscountAmount"] != DBNull.Value ? Convert.ToDouble(reader["DiscountAmount"]) : 0,
                                Balance = reader["Balance"] != DBNull.Value ? Convert.ToDouble(reader["Balance"]) : 0,
                                ServiceConnectionFee = reader["ServiceConnectionFee"] != DBNull.Value ? Convert.ToDouble(reader["ServiceConnectionFee"]) : 0,
                                Others1 = reader["Others1"]?.ToString(),
                                OthersAmount1 = reader["OthersAmount1"] != DBNull.Value ? Convert.ToDouble(reader["OthersAmount1"]) : 0,
                                Others2 = reader["Others2"]?.ToString(),
                                OthersAmount2 = reader["OthersAmount2"] != DBNull.Value ? Convert.ToDouble(reader["OthersAmount2"]) : 0,
                                AmountPaid = reader["AmountPaid"] != DBNull.Value ? Convert.ToDouble(reader["AmountPaid"]) : 0,
                                IsArrears = reader["Is_Arrears"] != DBNull.Value && Convert.ToBoolean(reader["Is_Arrears"]),
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
