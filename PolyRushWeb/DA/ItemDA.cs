using Microsoft.EntityFrameworkCore;
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
        public async Task<int> GetDiscountedPriceAsync(Item i)
        {
            //get the fresh item from the database
            Item item = await GetItemById(i.Iditem)!;

            //get discounted percentage between dates
            var query = _context.Discount.Where(d => d.ItemId == i.Iditem && (d.Startdate < DateTime.Now && DateTime.Now < d.Enddate));
            if (!query.Any()) return item.Price;

            //select only highest percentage.
            var discountResult = query.Select(d => d.DiscountPercentage).MaxAsync();

            decimal discountPercentage = Convert.ToDecimal(discountResult);
            decimal discount = Math.Round(item.Price * (discountPercentage / 100m), 2, MidpointRounding.ToEven);
            decimal discountedPrice = item.Price - discount;

            return (int)Math.Ceiling(discountedPrice);
        }


        public async Task<int> GetAmountAsync(Item item, int id, bool isAdmin = false)
        {
            if (isAdmin) return 9999;

            try
            {
                int result = await _context.Useritem.Where(ui => ui.UserId == id)
              .Join(_context.Item, ui => ui.ItemId, i => i.Iditem, (ui, i) => new { Useritem = ui, Item = i })
              .Where(x => x.Item.Iditem == item.Iditem).Select(x => x.Useritem.Amount).FirstOrDefaultAsync();
                return result;
            }
            catch
            {
                return 0;
            }

        }

        public async Task<List<Item>> GetItemsFromTypeAsync(ItemType type) => await _context.Item.Where(i => i.ItemTypeId == (int)type).ToListAsync();

        public async Task<Item> GetItemById(int id) => await _context.Item.Where(i => i.Iditem == id).FirstOrDefaultAsync();

        private async Task CreateUserItemAsync(int id, Item item, bool isAdmin = false)
        {

            Useritem useritem = new Useritem { ItemId = item.Iditem, Amount = isAdmin ? 69420 : 0, UserId = id};
            await _context.Useritem.AddAsync(useritem);
            await _context.SaveChangesAsync();
        }
        //optional parameter for if the user is an admin
        public async Task<bool> BuyItem(int id, Item item, bool isAdmin = false)
        {
            if (!await UserItemExistsAsync(id, item))
                await CreateUserItemAsync(id, item, isAdmin);
            
                        //get the discounted price
            int price = await GetDiscountedPriceAsync(item);
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
        private async Task<bool> UserItemExistsAsync(int id, Item item) 
            => await _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem).AnyAsync();

        public async Task<List<Item>> GetOwnedItemsFromTypeAsync(int id, ItemType type, bool getImage) => await _context.Useritem
                .Join(_context.Item, ui => ui.ItemId, i => i.Iditem, (ui, i) => new { Useritem = ui, Item = i })
                .Where(x => x.Useritem.UserId == id && x.Item.ItemTypeId == (int)type).Select(x => x.Item).ToListAsync();
        public async Task<List<Item>> GetItemsFromType(ItemType type, bool getImage) 
            => await _context.Item.Where(i => i.ItemTypeId == (int)type).ToListAsync();
        public async Task<int> GetItemAmountAsync(int id, Item item)
        {
            return (await _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem).FirstOrDefaultAsync())!.Amount;
        }

        public async Task<bool> Remove1ItemAsync(int id, Item item, bool isAdmin = false)
        {
            if (isAdmin) return true;
            if (await GetItemAmountAsync(id, item) < 1) return false;

            await _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem)
                .UpdateFromQueryAsync(ui => new Useritem { Amount = ui.Amount +1});

            //MySqlConnection conn = DatabaseConnector.MakeConnection();

            //string query = "UPDATE useritem SET Amount = Amount - 1 WHERE UserID=@UserID AND ItemID = @ItemID";
            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@UserID", id);
            //cmd.Parameters.AddWithValue("@ItemID", item.Iditem);

            //try
            //{
            //    cmd.ExecuteReader();
            //}
            //finally
            //{
            //    conn.Close();
            //}

            return true;
        }

        public async Task<List<Item>> GetDiscountedItemsFromTypeAsync(ItemType type)
        {
            var items = await _context.Item.Where(i => i.ItemTypeId == (int)type).ToListAsync();

            foreach (var item in items)
            {
                item.Price = await GetDiscountedPriceAsync(item);
            }

            return items;
            //MySqlConnection conn = DatabaseConnector.MakeConnection();
            //string query = @"SELECT * FROM item WHERE ItemTypeID = @ItemTypeID";

            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@ItemTypeID", (int) type);
            //MySqlDataReader? reader = cmd.ExecuteReader();

            //List<Item?> items = new();

            //while (reader.Read())
            //{
            //    Item? item = Create(reader);
            //    if (item == null) throw new NullReferenceException();
            //    item.Price = GetDiscountedPriceAsync(item);
            //    items.Add(item);
            //}

            //conn.Close();
            //reader.Close();
            //return items;
        }
      
        //private  Item? Create(MySqlDataReader reader)
        //{
        //    return new()
        //    {
        //        Iditem = Convert.ToInt32(reader["IDItem"]),
        //        Name = reader["Name"].ToString()!,
        //        Icon = reader["Icon"].ToString()!,
        //        Price = Convert.ToInt32(reader["Price"]),
        //        ItemTypeId = Convert.ToInt32(reader["ItemTypeID"])
        //    };
        //}
        //private  Item? CreateWithoutIcon(MySqlDataReader reader)
        //{
        //    return new()
        //    {
        //        Iditem = Convert.ToInt32(reader["IDItem"]),
        //        Name = reader["Name"].ToString()!,
        //        Icon = "",
        //        Price = Convert.ToInt32(reader["Price"]),
        //        ItemTypeId = Convert.ToInt32(reader["ItemTypeID"])
        //    };
        //}

        
    }
}