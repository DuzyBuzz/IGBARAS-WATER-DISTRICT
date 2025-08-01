using System;
using System.Collections.Generic;
using System.Data.OleDb;

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
        /// Returns formatted 2-digit zone numbers for a given district from Access database.
        /// </summary>
        public static List<ZoneItem> GetZoneCodeHelper(int districtNo)
        {
            List<ZoneItem> zoneList = new List<ZoneItem>();

            using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
            {
                conn.Open();

                string query = @"
                    SELECT ZoneCode 
                    FROM Tb_Zone 
                    ORDER BY ZoneCode ASC";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // NOTE: OleDb uses positional parameters with `?`, not named ones.
                    cmd.Parameters.AddWithValue("ZoneCode", districtNo);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int zoneno = Convert.ToInt32(reader["ZoneCode"]);

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
