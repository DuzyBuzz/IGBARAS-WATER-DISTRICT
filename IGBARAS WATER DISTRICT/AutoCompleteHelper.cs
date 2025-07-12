using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace IGBARAS_WATER_DISTRICT
{
    internal static class AutoCompleteHelper
    {
        // Set your connection string ONCE here (you can also move it to a config file)

        /// <summary>
        /// Fills a TextBox with AutoComplete suggestions based on a column from a given table.
        /// </summary>
        /// <param name="tableName">Table name (e.g., tb_bill)</param>
        /// <param name="columnName">Column to use for AutoComplete (e.g., accountno)</param>
        /// <param name="textBox">The TextBox control to apply AutoComplete to</param>
        public static void FillTextBoxWithColumn(string tableName, string columnName, TextBox textBox)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    string query = $"SELECT DISTINCT `{columnName}` FROM `{tableName}` ORDER BY `{columnName}` ASC;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    AutoCompleteStringCollection suggestions = new AutoCompleteStringCollection();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        { 
                            suggestions.Add(reader.GetString(0));
                        }
                    }

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
