using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public class ZoneItem
    {
        public string ZoneCode { get; set; } // e.g., "01", "11"

        public override string ToString()
        {
            return ZoneCode; // Shown in ComboBox
        }
    }

    internal static class ZoneHelper
    {
        /// <summary>
        /// Returns formatted 2-digit zone numbers for a given district.
        /// </summary>
        public static List<ZoneItem> GetZoneCodeHelper(int districtNo)
        {
            List<ZoneItem> zoneList = new List<ZoneItem>();

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();

                string query = @"
                    SELECT zoneno 
                    FROM tb_zone 
                    WHERE districtno = @districtno 
                    ORDER BY zoneno ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@districtno", districtNo);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int zoneno = Convert.ToInt32(reader["zoneno"]);

                            // Pad to 2 digits: 1 → "01", 11 → "11"
                            string formattedZoneCode = zoneno.ToString().PadLeft(2, '0');

                            zoneList.Add(new ZoneItem
                            {
                                ZoneCode = formattedZoneCode
                            });
                        }
                    }
                }
            }

            return zoneList;
        }
    }
}
