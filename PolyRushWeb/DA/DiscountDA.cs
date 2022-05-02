using System;
using PolyRushLibrary;
using PolyRushWeb.Helper;

namespace PolyRushWeb.DA
{
    public class DiscountDA
    {
        //private readonly ItemDA _itemDa;
        //public DiscountDA(ItemDA itemDa)
        //{
        //    _itemDa = itemDa;
        //}
        //public int GetDiscountedPrice(Item? i)
        //{
        //    //get the fresh item from the database
        //    Item? item = _itemDa.GetItemById(i.Iditem);
        //    MySqlConnection conn = DatabaseConnector.MakeConnection();
        //    string query =
        //        @"SELECT DiscountPercentage FROM discount WHERE ItemID = @ItemID AND CURDATE() BETWEEN Startdate AND Enddate";
        //    MySqlCommand cmd = new(query, conn);
        //    cmd.Parameters.AddWithValue("@ItemID", item.Iditem);
        //    decimal discountPercentage = Convert.ToDecimal(cmd.ExecuteScalar());
        //    decimal discount = Math.Round(item.Price * (discountPercentage / 100m), 2, MidpointRounding.ToEven);
        //    decimal discountedPrice = item.Price - discount;

        //    return (int) Math.Ceiling(discountedPrice);
        //}
    }
}