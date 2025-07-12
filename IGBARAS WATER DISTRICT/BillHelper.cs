using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace IGBARAS_WATER_DISTRICT
{
    internal class BillHelper
    {
        public DataTable AllBills { get; private set; }

        private async Task<DataTable> ExecuteQueryAsync(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        await Task.Run(() => adapter.Fill(table));
                        return table;
                    }
                }
            }
        }

        public async Task LoadAllBillsAsync()
        {
            string query = "SELECT * FROM v_billing_summary ORDER BY datebilled DESC";
            AllBills = await ExecuteQueryAsync(query);
        }
    }

}
