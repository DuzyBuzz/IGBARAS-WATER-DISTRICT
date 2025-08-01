using System;
using System.Data.OleDb;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal static class SettingsHelper
    {
        public class SystemSettings
        {
            public double TaxPercent { get; set; }
            public double PenaltyPercent { get; set; }
            public int PenaltyDuration { get; set; }
        }
        public static SystemSettings GetSettings()
        {
            var settings = new SystemSettings();

            using (var connection = new OleDbConnection(DbConfig.ConnectionString))
            {
                string query = "SELECT TOP 1 TaxPercent, PenaltyPercent, PenaltyDuration FROM Tb_Settings";

                using (var command = new OleDbCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            settings.TaxPercent = reader["TaxPercent"] != DBNull.Value ? Convert.ToDouble(reader["TaxPercent"]) : 0;
                            settings.PenaltyPercent = reader["PenaltyPercent"] != DBNull.Value ? Convert.ToDouble(reader["PenaltyPercent"]) : 0;
                            settings.PenaltyDuration = reader["PenaltyDuration"] != DBNull.Value ? Convert.ToInt32(reader["PenaltyDuration"]) : 0;
                        }
                    }
                }
            }

            return settings;
        }

        public static double GetTaxBasedOnExemption(int taxExempt)
        {
            if (taxExempt == 1) return 0;

            var settings = GetSettings();
            return settings.TaxPercent;
        }
        public static double GetTaxPercent(int taxExempt)
        {
            if (taxExempt == 1)
                return 0;

            using (var connection = new OleDbConnection(DbConfig.ConnectionString))
            {
                const string query = "SELECT TOP 1 TaxPercent FROM Tb_Settings";

                using (var command = new OleDbCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null && double.TryParse(result.ToString(), out double taxPercent))
                        return taxPercent;
                }
            }

            return 0; // default if not found or error
        }
    }
}
