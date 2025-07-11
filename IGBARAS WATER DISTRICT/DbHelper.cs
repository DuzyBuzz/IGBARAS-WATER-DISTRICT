using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace IGBARAS_WATER_DISTRICT
{
    internal static class DbHelper
    {

        // ✅ Basic method to run SELECT and return a DataTable
        public static DataTable GetTable(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MySQL GetTable failed: " + ex.Message);
                }
            }
        }

        // ✅ Optional: Parameterized version
        public static DataTable GetTable(string query, Dictionary<string, object> parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            return table;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MySQL GetTable (with params) failed: " + ex.Message);
                }
            }
        }

        // ✅ Optional: Execute INSERT / UPDATE / DELETE
        public static int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MySQL ExecuteNonQuery failed: " + ex.Message);
                }
            }
        }
    }
}
