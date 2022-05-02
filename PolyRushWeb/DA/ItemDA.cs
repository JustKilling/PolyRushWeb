using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PolyRushLibrary;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{
    public class ItemDA
    {
        private readonly UserDA _userDa;
        private readonly polyrushContext _context;

        public ItemDA(UserDA userDa, polyrushContext context)
        {
            _userDa = userDa;
            _context = context;

        }
        public int GetDiscountedPrice(Item i)
        {
      
            //get the fresh item from the database
            Item item = GetItemById(i.Iditem)!;
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query =
                @"SELECT DiscountPercentage FROM discount WHERE ItemID = @ItemID AND CURDATE() BETWEEN Startdate AND Enddate";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemID", item!.Iditem);
            decimal discountPercentage = Convert.ToDecimal(cmd.ExecuteScalar());
            decimal discount = Math.Round(item.Price * (discountPercentage / 100m), 2, MidpointRounding.ToEven);
            decimal discountedPrice = item.Price - discount;

            return (int)Math.Ceiling(discountedPrice);
        }


        public  int GetAmount(Item item, int id, bool isAdmin = false)
        {
            if (isAdmin) return 9999;
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query =
                @"SELECT useritem.Amount FROM useritem 
                INNER JOIN item on useritem.ItemID = item.IDItem 
                WHERE UserID = @UserID and IDItem = @IDItem";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@IDItem", item.Iditem);
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

        public  List<Item?> GetItemsFromType(ItemType type)
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

        public  Item? GetItemById(int id)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                "SELECT * FROM item WHERE IDItem = @ItemID";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemID", id);

            try
            {
                MySqlDataReader? reader = cmd.ExecuteReader();
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

        private async Task CreateUserItemAsync(int id, Item item, bool isAdmin = false)
        {
            Useritem useritem = new Useritem { ItemId = item.Iditem, Amount = isAdmin ? 69420 : 0, UserId = id};
            await _context.Useritem.AddAsync(useritem);
            await _context.SaveChangesAsync();
            //MySqlConnection conn = DatabaseConnector.MakeConnection();

            //string query =
            //    "INSERT INTO useritem (UserID, ItemID, Amount) VALUES (@userid, @itemid, @amount)";

            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@userid", id);
            //cmd.Parameters.AddWithValue("@itemid", item!.Iditem);
            ////If user is an admin, provide a very high number.
            //cmd.Parameters.AddWithValue("@amount", isAdmin ? 69420 : 0);
            
            //cmd.ExecuteNonQuery();
            //conn.Close();
        }
        //optional parameter for if the user is an admin
        public async Task<bool> BuyItem(int id, Item item, bool isAdmin = false)
        {
            if (!UserItemExists(id, item))
                await CreateUserItemAsync(id, item, isAdmin);
            
                        //get the discounted price
            int price = GetDiscountedPrice(item);
            //if the user is an admin, set the price to 0;
            if (isAdmin) price = 0;
            if (!(await _userDa.HasEnoughCoins(id, price))) return false;

           
            var useritem = _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem).FirstOrDefault();
            useritem.Amount++;
            _context.Useritem.Update(useritem);

            //MySqlConnection conn = DatabaseConnector.MakeConnection();
            ////increment useritem amount
            //string query = "UPDATE useritem SET Amount = Amount + 1 WHERE UserID = @UserID AND ItemID = @ItemID";
            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@UserID", id);
            //cmd.Parameters.AddWithValue("@ItemID", item.Iditem);
            //cmd.ExecuteNonQuery();

            //substract price

            var user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            user.Coins -= price;
            _context.Users.Update(user);

            //query = "UPDATE user SET Coins = Coins - @Amount WHERE IDUser = @IDUser";
            //cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@Amount", price);
            //cmd.Parameters.AddWithValue("@IDUser", id);
            //cmd.ExecuteNonQuery();
            //conn.Close();

            return true;
        }

        //Check if user has a record with this item.
        private  bool UserItemExists(int id, Item? item)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query = "SELECT COUNT(IDUserItem) from useritem WHERE UserID = @UserID AND ItemID = @ItemID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemID", item.Iditem);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) >= 1;
            }
            finally
            {
                conn.Close();
            }
        }

        public  List<Item?> GetOwnedItemsFromType(int id, ItemType type, bool getImage)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            //check to get the icon
            string query = getImage ? "SELECT * from useritem INNER JOIN item ON ItemID = IDItem WHERE UserID = @UserID AND ItemTypeID = @ItemTypeID" : "SELECT IDUserItem, UserID, ItemID, Amount, IDItem, ItemTypeID, Name, Price from useritem INNER JOIN item ON ItemID = IDItem WHERE UserID = @UserID AND ItemTypeID = @ItemTypeID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);

            List<Item?> items = new();
            MySqlDataReader? reader = cmd.ExecuteReader();
            
            while (reader.Read()) items.Add(getImage ? Create(reader) : CreateWithoutIcon(reader));
            reader.Close();
            conn.Close();
            return items;
        }
        public  List<Item?> GetItemsFromType(ItemType type, bool getImage)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            //check to get the icon
            string query = getImage ? "SELECT * from item WHERE ItemTypeID = @ItemTypeID" : "SELECT IDItem, ItemTypeID, Name, Price from item WHERE ItemTypeID = @ItemTypeID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);

            List<Item?> items = new();
            MySqlDataReader? reader = cmd.ExecuteReader();
            
            while (reader.Read()) items.Add(getImage ? Create(reader) : CreateWithoutIcon(reader));
            reader.Close();
            conn.Close();
            return items;
        }
        public  int GetItemAmount(int id, Item item)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "SELECT Amount FROM useritem WHERE UserID=@UserID AND ItemID = @ItemID";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemID", item.Iditem);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                conn.Close();
            }

        }

        public  bool Remove1Item(int id, Item item, bool isAdmin = false)
        {
            if (isAdmin) return true;
            if (GetItemAmount(id, item) < 1) return false;
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "UPDATE useritem SET Amount = Amount - 1 WHERE UserID=@UserID AND ItemID = @ItemID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@ItemID", item.Iditem);

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

        public  List<Item?> GetDiscountedItemsFromType(ItemType type)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query = @"SELECT * FROM item WHERE ItemTypeID = @ItemTypeID";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);
            MySqlDataReader? reader = cmd.ExecuteReader();

            List<Item?> items = new();

            while (reader.Read())
            {
                Item? item = Create(reader);
                if (item == null) throw new NullReferenceException();
                item.Price = GetDiscountedPrice(item);
                items.Add(item);
            }

            conn.Close();
            reader.Close();
            return items;
        }
      
        private  Item? Create(MySqlDataReader reader)
        {
            return new()
            {
                Iditem = Convert.ToInt32(reader["IDItem"]),
                Name = reader["Name"].ToString()!,
                Icon = reader["Icon"].ToString()!,
                Price = Convert.ToInt32(reader["Price"]),
                ItemTypeId = Convert.ToInt32(reader["ItemTypeID"])
            };
        }
        private  Item? CreateWithoutIcon(MySqlDataReader reader)
        {
            return new()
            {
                Iditem = Convert.ToInt32(reader["IDItem"]),
                Name = reader["Name"].ToString()!,
                Icon = "",
                Price = Convert.ToInt32(reader["Price"]),
                ItemTypeId = Convert.ToInt32(reader["ItemTypeID"])
            };
        }

        
    }
}