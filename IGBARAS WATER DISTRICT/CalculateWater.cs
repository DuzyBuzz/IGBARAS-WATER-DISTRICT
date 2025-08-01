using IGBARAS_WATER_DISTRICT.Helpers;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGBARAS_WATER_DISTRICT
{
    internal class CalculateWater
    {
        public class ServiceRateResult
        {
            public decimal MinRate { get; set; }
            public int[] Quantities { get; set; } = new int[5];
            public decimal[] UnitPrices { get; set; } = new decimal[5];
            public decimal[] Amounts { get; set; } = new decimal[5];
            public decimal TotalAmount { get; set; }
        }

        public ServiceRateResult CalculateServiceRate(int serviceRateId, int totalConsumption)
        {
            var result = new ServiceRateResult();

            using (var conn = new OleDbConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Tb_Service WHERE ServiceID = ?";
                using (var cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", serviceRateId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Parse the rates from database
                            decimal minRate = Convert.ToDecimal(reader["MinRate"]);
                            decimal[] prices = new decimal[]
                            {
                                0, // for min 0–10, using MinRate
                                Convert.ToDecimal(reader["Rate11-20"]),
                                Convert.ToDecimal(reader["Rate21-30"]),
                                Convert.ToDecimal(reader["Rate31-40"]),
                                Convert.ToDecimal(reader["Rate41-Above"])
                            };

                            result.MinRate = minRate;
                            result.UnitPrices = prices;

                            int remaining = totalConsumption;

                            // First 10 cu.m uses MinRate
                            int q0 = Math.Min(10, remaining);
                            result.Quantities[0] = q0;
                            result.Amounts[0] = q0 > 0 ? minRate : 0;
                            remaining -= q0;

                            int[] limits = { 10, 10, 10, int.MaxValue };

                            for (int i = 1; i <= 4 && remaining > 0; i++)
                            {
                                int qty = Math.Min(limits[i - 1], remaining);
                                result.Quantities[i] = qty;
                                result.Amounts[i] = qty * prices[i];
                                remaining -= qty;
                            }

                            result.TotalAmount = result.Amounts.Sum();
                        }
                    }
                }
            }

            return result;
        }


    }
}
