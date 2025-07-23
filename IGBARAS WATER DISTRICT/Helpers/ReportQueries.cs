using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class ReportQueries
    {
        private static readonly string connectionString = DbConfig.ConnectionString;

        public static async Task<DataTable> GetBillingSummaryAsync(string groupBy)
        {
            string groupFormat = groupBy.ToLower() switch
            {
                "daily" => "DATE(datebilled)",
                "monthly" => "DATE_FORMAT(datebilled, '%Y-%m')",
                "yearly" => "YEAR(datebilled)",
                _ => "DATE(datebilled)"
            };

            string labelColumn = groupBy switch
            {
                "Daily" => "Billing Date",
                "Monthly" => "Billing Month",
                "Yearly" => "Billing Year",
                _ => "Billing Date"
            };

            string query = $@"
                SELECT 
                    {groupFormat} AS `{labelColumn}`,
                    COUNT(*) AS `Number of Bills`,
                    SUM(totalbillcharge) AS `Total Charges`,
                    SUM(amountpaid) AS `Total Paid`,
                    SUM(balance) AS `Total Balance`
                FROM tb_bill
                WHERE datebilled IS NOT NULL
                GROUP BY {groupFormat}
                ORDER BY {groupFormat} DESC;
            ";

            return await RunQueryAsync(query);
        }

        public static async Task<DataTable> GetCollectionSummaryAsync(string groupBy)
        {
            string groupFormat = groupBy.ToLower() switch
            {
                "daily" => "DATE(datebilled)",
                "monthly" => "DATE_FORMAT(datebilled, '%Y-%m')",
                "yearly" => "YEAR(datebilled)",
                _ => "DATE(datebilled)"
            };

            string labelColumn = groupBy switch
            {
                "Daily" => "Collection Date",
                "Monthly" => "Collection Month",
                "Yearly" => "Collection Year",
                _ => "Collection Date"
            };

            string query = $@"
                SELECT 
                    {groupFormat} AS `{labelColumn}`,
                    COUNT(*) AS `Number of Payments`,
                    SUM(amountpaid) AS `Total Collected`
                FROM tb_bill
                WHERE amountpaid > 0
                GROUP BY {groupFormat}
                ORDER BY {groupFormat} DESC;
            ";

            return await RunQueryAsync(query);
        }

        public static async Task<DataTable> GetPartiallyPaidBillsAsync()
        {
            string query = @"
                SELECT 
                    billcode,
                    accountno,
                    name,
                    amountpaid,
                    totalbillcharge,
                    balance,
                    datebilled
                FROM tb_bill
                WHERE partiallypaid = 1
                ORDER BY datebilled DESC;
            ";

            return await RunQueryAsync(query);
        }

        public static async Task<DataTable> GetPenaltyRevenueByMonthAsync()
        {
            string query = @"
                SELECT 
                    DATE_FORMAT(datebilled, '%Y-%m') AS `Month`,
                    COUNT(*) AS `Bills With Penalty`,
                    SUM(penaltyamount) AS `Total Penalty Revenue`
                FROM tb_bill
                WHERE penaltyamount > 0
                GROUP BY DATE_FORMAT(datebilled, '%Y-%m')
                ORDER BY `Month` DESC;
            ";

            return await RunQueryAsync(query);
        }

        public static async Task<DataTable> GetDisconnectionCandidatesAsync()
        {
            string query = @"
                SELECT 
                    accountno,
                    name,
                    address,
                    balance,
                    disconnectiondate,
                    datebilled
                FROM tb_bill
                WHERE disconnectiondate IS NOT NULL AND balance > 0
                ORDER BY disconnectiondate DESC;
            ";

            return await RunQueryAsync(query);
        }

        public static async Task<DataTable> GetOutstandingBalancesAsync()
        {
            string query = @"
                SELECT 
                    accountno,
                    name,
                    address,
                    SUM(balance) AS `Total Outstanding Balance`,
                    MAX(datebilled) AS `Last Billed`
                FROM tb_bill
                WHERE balance > 0
                GROUP BY accountno, name, address
                ORDER BY `Total Outstanding Balance` DESC;
            ";

            return await RunQueryAsync(query);
        }

        private static async Task<DataTable> RunQueryAsync(string query)
        {
            var dt = new DataTable();

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            using (var adapter = new MySqlDataAdapter(cmd))
            {
                await conn.OpenAsync();
                adapter.Fill(dt);
            }

            return dt;
        }
    }
}
