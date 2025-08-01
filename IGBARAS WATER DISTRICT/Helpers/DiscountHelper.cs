using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public class DiscountItem
    {
        public int DiscountID { get; set; }
        public string DiscountName { get; set; }
        public int DiscountPercent { get; set; }

        public override string ToString()
        {
            return $"{DiscountName} - {DiscountPercent}%";
        }
    }

    public static class DiscountHelper
    {
        public static List<DiscountItem> LoadDiscounts()
        {
            List<DiscountItem> discounts = new List<DiscountItem>();

            using (var connection = new OleDbConnection(DbConfig.ConnectionString))
            {
                string query = "SELECT DiscountID, DiscountName, Discount FROM Tb_Discount";
                using (var command = new OleDbCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            discounts.Add(new DiscountItem
                            {
                                DiscountID = reader.GetInt32(0),
                                DiscountName = reader.GetString(1),
                                DiscountPercent = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }

            return discounts;
        }

        public static void PopulateDiscounts(ComboBox comboBox)
        {
            var discountList = LoadDiscounts();
            comboBox.Items.Clear();
            comboBox.Items.AddRange(discountList.ToArray());
        }

        public static DiscountItem GetSelectedDiscount(ComboBox comboBox)
        {
            return comboBox.SelectedItem as DiscountItem;
        }
    }
}
