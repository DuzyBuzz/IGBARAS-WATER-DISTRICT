using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal class GetPenaltyHelper
    {
        /// <summary>
        /// Computes the penalty using the GETPENALTY() MySQL function.
        /// It finds the latest paid bill and uses the first unpaid bill that follows it as the basis for penalty.
        /// </summary>
        /// <param name="accountNo">Account number to evaluate</param>
        /// <param name="billCharge">Total amount charged for the bill</param>
        /// <param name="dueExempt">1 if exempted from due, 0 otherwise</param>
        /// <param name="srcPenalty">Existing penalty (usually from the database)</param>
        /// <param name="srcPaid">1 if the current bill is already paid, 0 otherwise</param>
        /// <param name="usedDueDate">[OUT] The actual due date used for penalty calculation</param>
        /// <param name="usedBill_id">[OUT] The bill_id of the unpaid bill used for penalty</param>
        /// <param name="usedBillCode">[OUT] The billcode of the unpaid bill used for penalty</param>
        /// <returns>Returns the computed penalty as a decimal value</returns>
        public static decimal GetPenalty(
            string accountNo,
            decimal billCharge,
            int dueExempt,
            decimal srcPenalty,
            int srcPaid,
            out DateTime? usedDueDate,
            out int usedBill_id,
            out string usedBillCode)
        {
            decimal penaltyResult = 0;
            DateTime dueGracePeriod = DateTime.Today; // fallback if nothing is found
            int arrears = 0;
            usedDueDate = null;
            usedBill_id = 0;
            usedBillCode = string.Empty;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // ✅ STEP 1: Get the latest paid due date for the account
                    DateTime? latestPaidDueDate = null;

                    string latestPaidQuery = @"
                        SELECT duedate
                        FROM tb_bill
                        WHERE accountno = @accountno
                          AND paid = 1
                        ORDER BY duedate DESC
                        LIMIT 1;";

                    using (MySqlCommand latestPaidCmd = new MySqlCommand(latestPaidQuery, conn))
                    {
                        latestPaidCmd.Parameters.AddWithValue("@accountno", accountNo);

                        object result = latestPaidCmd.ExecuteScalar();
                        if (result != null && DateTime.TryParse(result.ToString(), out DateTime latest))
                        {
                            latestPaidDueDate = latest;
                        }
                    }

                    // ✅ STEP 2: Find the next unpaid bill AFTER the latest paid one
                    // This determines where to base the penalty calculation
                    string unpaidQuery = @"
                        SELECT tb_bill.bill_id, tb_bill.billcode, tb_bill.duedate, tb_billsettings.graceperiod
                        FROM tb_bill
                        JOIN tb_billsettings ON tb_bill.districtno = tb_billsettings.districtno
                        WHERE tb_bill.accountno = @accountno
                          AND tb_bill.paid = 0
                          AND (@latestPaidDueDate IS NULL OR tb_bill.duedate > @latestPaidDueDate)
                        ORDER BY tb_bill.duedate ASC
                        LIMIT 1;";

                    using (MySqlCommand unpaidCmd = new MySqlCommand(unpaidQuery, conn))
                    {
                        unpaidCmd.Parameters.AddWithValue("@accountno", accountNo);
                        unpaidCmd.Parameters.AddWithValue("@latestPaidDueDate",
                            latestPaidDueDate.HasValue ? (object)latestPaidDueDate.Value : DBNull.Value);

                        using (MySqlDataReader reader = unpaidCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 🟢 Unpaid bill found after the latest paid one
                                int billId = reader.GetInt32("bill_id");
                                string billCode = reader.GetString("billcode");
                                DateTime dueDate = reader.GetDateTime("duedate");
                                int graceDays = reader.GetInt32("graceperiod");

                                dueGracePeriod = dueDate.AddDays(graceDays);
                                usedDueDate = dueDate;
                                usedBill_id = billId;
                                usedBillCode = billCode;
                                arrears = 1;
                            }
                        }
                    }

                    // ✅ STEP 3: Call MySQL function GETPENALTY()
                    string funcQuery = @"
                        SELECT GETPENALTY(
                            @billCharge,
                            @dueGracePeriod,
                            @dueExempt,
                            @arrears,
                            @srcPenalty,
                            @srcPaid
                        );";

                    using (MySqlCommand funcCmd = new MySqlCommand(funcQuery, conn))
                    {
                        funcCmd.Parameters.AddWithValue("@billCharge", billCharge);
                        funcCmd.Parameters.AddWithValue("@dueGracePeriod", dueGracePeriod.ToString("yyyy-MM-dd"));
                        funcCmd.Parameters.AddWithValue("@dueExempt", dueExempt);
                        funcCmd.Parameters.AddWithValue("@arrears", arrears);
                        funcCmd.Parameters.AddWithValue("@srcPenalty", srcPenalty);
                        funcCmd.Parameters.AddWithValue("@srcPaid", srcPaid);

                        object result = funcCmd.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal parsedPenalty))
                        {
                            penaltyResult = parsedPenalty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetPenaltyHelper] Error: {ex.Message}");
            }

            return penaltyResult;
        }
    }
}