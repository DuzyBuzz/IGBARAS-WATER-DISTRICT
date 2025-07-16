using System;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal static class AutoCompleteHelper
    {
        /// <summary>
        /// Fills a TextBox with AutoComplete suggestions based on one or more columns from a given table or view.
        /// </summary>
        /// <param name="tableName">Table or view name (e.g., v_account_detailes)</param>
        /// <param name="columnNames">Array of column names to pull suggestions from (e.g., new[] { "accountno", "fullname" })</param>
        /// <param name="textBox">The TextBox control to apply AutoComplete to</param>
        public static void FillTextBoxWithColumns(string tableName, string[] columnNames, TextBox textBox)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // Join column names into SQL SELECT
                    string selectColumns = string.Join(", ", columnNames.Select(col => $"`{col}`"));
                    string query = $"SELECT DISTINCT {selectColumns} FROM `{tableName}`";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    // Collection for autocomplete suggestions
                    AutoCompleteStringCollection suggestions = new AutoCompleteStringCollection();
                    HashSet<string> seen = new HashSet<string>(); // Prevent duplicates

                    while (reader.Read())
                    {
                        foreach (var col in columnNames)
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(col)))
                            {
                                string value = reader[col]?.ToString();
                                if (!string.IsNullOrWhiteSpace(value) && seen.Add(value))
                                {
                                    suggestions.Add(value);
                                }
                            }
                        }
                    }

                    // Apply to textbox
                    textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    textBox.AutoCompleteCustomSource = suggestions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load auto-complete:\n" + ex.Message, "AutoComplete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
