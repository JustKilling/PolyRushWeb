using System;
using MySql.Data.MySqlClient;
using PolyRushAPI.Helper;
using PolyRushLibrary;

namespace PolyRushAPI.DA
{
    public class DiscountDA
    {
        public static int GetDiscountedPrice(Item? i)
        {
            //get the fresh item from the database
            var item = ItemDA.GetItemById(i.Id);
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query =
                @"SELECT DiscountPercentage FROM discount WHERE ItemID = @ItemID AND CURDATE() BETWEEN Startdate AND Enddate";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemID", item.Id);
            var discountPercentage = Convert.ToDecimal(cmd.ExecuteScalar());
            decimal discount = Math.Round(item.Price * (discountPercentage / 100m), 2, MidpointRounding.ToEven);
            decimal discountedPrice = item.Price - discount;

            return (int) Math.Ceiling(discountedPrice);
        }
    }
}