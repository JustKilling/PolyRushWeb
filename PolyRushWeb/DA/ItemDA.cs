using System;
using System.Collections.Generic;
using Helper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PolyRushLibrary;

namespace PolyRushAPI.DA
{
    public static class ItemDA
    {
        public static int GetAmount(Item item, int id, bool isAdmin = false)
        {
            if (isAdmin) return 9999;
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query =
                @"SELECT useritem.Amount FROM useritem 
                INNER JOIN item on useritem.ItemID = item.IDItem 
                WHERE UserID = @UserID and IDItem = @IDItem";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@IDItem", item.Id);
            cmd.Parameters.AddWithValue("UserID", id);
            try
            {
                int amount = Convert.ToInt32(cmd.ExecuteScalar());
                return amount;
            }
            catch
            {
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<Item?> GetItemsFromType(ItemType type)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query = @"SELECT * FROM item WHERE ItemTypeID = @ItemTypeID";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);
            MySqlDataReader? reader = cmd.ExecuteReader();

            List<Item?> items = new();

            while (reader.Read()) items.Add(Create(reader));
            conn.Close();

            return items;
        }

        public static Item? GetItemById(int id)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                "SELECT * FROM item WHERE IDItem = @ItemID";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemID", id);

            try
            {
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return Create(reader);
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        private static void CreateUserItem(int id, Item? item, bool isAdmin = false)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                "INSERT INTO useritem (UserID, ItemID, Amount) VALUES (@userid, @itemid, @amount)";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@userid", id);
            cmd.Parameters.AddWithValue("@itemid", item.Id);
            //If user is an admin, provide a very high number.
            cmd.Parameters.AddWithValue("@amount", isAdmin ? 69420 : 0);
            
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        //optional parameter for if the user is an admin
        public static bool BuyItem(int id, Item? item, bool isAdmin = false)
        {
            if (!UserItemExists(id, item))
                CreateUserItem(id, item, isAdmin);
            
            //get the discounted price
            int price = DiscountDA.GetDiscountedPrice(item);
            //if the user is an admin, set the price to 0;
            if (isAdmin) price = 0;
            if (!UserDA.HasEnoughCoins(id, price)) return false;

            MySqlConnection conn = DatabaseConnector.MakeConnection();
            //increment useritem amount
            string query = "UPDATE useritem SET Amount = Amount + 1 WHERE UserID = @UserID AND ItemID = @ItemID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemID", item.Id);
            cmd.ExecuteNonQuery();

            //substract price

            query = "UPDATE user SET Coins = Coins - @Amount WHERE IDUser = @IDUser";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Amount", price);
            cmd.Parameters.AddWithValue("@IDUser", id);
            cmd.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        //Check if user has a record with this item.
        private static bool UserItemExists(int id, Item? item)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query = "SELECT COUNT(IDUserItem) from useritem WHERE UserID = @UserID AND ItemID = @ItemID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemID", item.Id);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) >= 1;
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<Item?> GetOwnedItemsFromType(int id, ItemType type, bool getImage)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            //check to get the icon
            string query = getImage ? "SELECT * from useritem INNER JOIN item ON ItemID = IDItem WHERE UserID = @UserID AND ItemTypeID = @ItemTypeID" : "SELECT IDUserItem, UserID, ItemID, Amount, IDItem, ItemTypeID, Name, Price from useritem INNER JOIN item ON ItemID = IDItem WHERE UserID = @UserID AND ItemTypeID = @ItemTypeID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);

            var items = new List<Item?>();
            var reader = cmd.ExecuteReader();
            
            while (reader.Read()) items.Add(getImage ? Create(reader) : CreateWithoutIcon(reader));
            reader.Close();
            conn.Close();
            return items;
        }
        public static List<Item?> GetItemsFromType(ItemType type, bool getImage)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            //check to get the icon
            string query = getImage ? "SELECT * from item WHERE ItemTypeID = @ItemTypeID" : "SELECT IDItem, ItemTypeID, Name, Price from item WHERE ItemTypeID = @ItemTypeID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);

            var items = new List<Item?>();
            var reader = cmd.ExecuteReader();
            
            while (reader.Read()) items.Add(getImage ? Create(reader) : CreateWithoutIcon(reader));
            reader.Close();
            conn.Close();
            return items;
        }
        public static int GetItemAmount(int id, Item item)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "SELECT Amount FROM useritem WHERE UserID=@UserID AND ItemID = @ItemID";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemID", item.Id);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                conn.Close();
            }

        }

        public static bool Remove1Item(int id, Item item, bool isAdmin = false)
        {
            if (isAdmin) return true;
            if (GetItemAmount(id, item) < 1) return false;
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "UPDATE useritem SET Amount = Amount - 1 WHERE UserID=@UserID AND ItemID = @ItemID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemID", item.Id);

            try
            {
                cmd.ExecuteReader();
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        public static List<Item?> GetDiscountedItemsFromType(ItemType type)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query = @"SELECT * FROM item WHERE ItemTypeID = @ItemTypeID";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);
            MySqlDataReader? reader = cmd.ExecuteReader();

            List<Item?> items = new();

            while (reader.Read())
            {
                var item = Create(reader);
                item.Price = DiscountDA.GetDiscountedPrice(item);
                items.Add(item);
            }

            conn.Close();
            reader.Close();
            return items;
        }

        private static Item? Create(MySqlDataReader reader)
        {
            return new Item
            {
                Id = Convert.ToInt32(reader["IDItem"]),
                Name = reader["Name"].ToString(),
                Icon = reader["Icon"].ToString(),
                Price = Convert.ToInt32(reader["Price"]),
                Type = (ItemType) Convert.ToInt32(reader["ItemTypeID"])
            };
        }
        private static Item? CreateWithoutIcon(MySqlDataReader reader)
        {
            return new Item
            {
                Id = Convert.ToInt32(reader["IDItem"]),
                Name = reader["Name"].ToString(),
                Icon = "",
                Price = Convert.ToInt32(reader["Price"]),
                Type = (ItemType) Convert.ToInt32(reader["ItemTypeID"])
            };
        }

        
    }
}