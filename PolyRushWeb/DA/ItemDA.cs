using Microsoft.EntityFrameworkCore;
using PolyRushWeb.Data;
using PolyRushWeb.Models;


namespace PolyRushWeb.DA
{
    public class ItemDA
    {
        private readonly UserDA _userDa;
        private readonly PolyRushWebContext _context;

        public ItemDA(UserDA userDa, PolyRushWebContext context)
        {
            _userDa = userDa;
            _context = context;
      
        }
        public async Task<int> GetDiscountedPriceAsync(Item i)
        {
            //get the fresh item from the database
            Item item = await GetItemById(i.Iditem)!;

            //get discounted percentage between dates
            IQueryable<Discount> query = _context.Discount.Where(d => d.ItemId == i.Iditem && (d.Startdate < DateTime.Now && DateTime.Now < d.Enddate));
            if (!query.Any()) return item.Price;

            //select only highest percentage.
            Task<int> discountResult = query.Select(d => d.DiscountPercentage).MaxAsync();

            decimal discountPercentage = Convert.ToDecimal(discountResult);
            decimal discount = Math.Round(item.Price * (discountPercentage / 100m), 2, MidpointRounding.ToEven);
            decimal discountedPrice = item.Price - discount;

            return (int)Math.Ceiling(discountedPrice);
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _context.Item.ToListAsync();
        }

        public async Task<List<ItemItemTypeModel>> GetItemItemTypes()
        {
            //return items with there corresponding types.
            List<ItemItemTypeModel> items = await _context.Itemtype
                .Join(_context.Item, it => it.IditemType, i => i.ItemTypeId,
                (it, i) => new ItemItemTypeModel() { Item = i, ItemType = it }).ToListAsync();
            return items;
        }

        public async Task<int> GetAmountAsync(Item item, int id, bool isAdmin = false)
        {
            if (isAdmin) return 9999;

            if (!await UserItemExistsAsync(id, item))
                await CreateUserItemAsync(id, item, isAdmin);

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

            Useritem useritem = new() { ItemId = item.Iditem, Amount = isAdmin ? 69420 : 0, UserId = id};
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

           
            Useritem? useritem = _context.Useritem.FirstOrDefault(ui => ui.UserId == id && ui.ItemId == item.Iditem);

            if (useritem != null)
            {
                useritem.Amount++;
                _context.Useritem.Update(useritem);
            }

            //MySqlConnection conn = DatabaseConnector.MakeConnection();
            ////increment useritem amount
            //string query = "UPDATE useritem SET Amount = Amount + 1 WHERE UserID = @UserID AND ItemID = @ItemID";
            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@UserID", id);
            //cmd.Parameters.AddWithValue("@ItemID", item.Iditem);
            //cmd.ExecuteNonQuery();

            //substract price

            User? user = _context.Users.FirstOrDefault(u => u.Id == id);
            user.Coins -= price;
            _context.Users.Update(user);

            //query = "UPDATE user SET Coins = Coins - @Amount WHERE IDUser = @IDUser";
            //cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@Amount", price);
            //cmd.Parameters.AddWithValue("@IDUser", id);
            //cmd.ExecuteNonQuery();
            //conn.Close();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task DeleteDiscount(int id)
        {
            _context.Discount.Remove(new Discount { Iddiscount = id });
            await _context.SaveChangesAsync();
        }

        public async Task AddDiscount(Discount discount)
        {
            await _context.Discount.AddAsync(discount);
            await _context.SaveChangesAsync();
        }

        //Check if user has a record with this item.
        private async Task<bool> UserItemExistsAsync(int id, Item item) 
            => await _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem).AnyAsync();

        public async Task<List<Item>> GetOwnedItemsFromTypeAsync(int id, ItemType type) => await _context.Useritem
                .Join(_context.Item, ui => ui.ItemId, i => i.Iditem, (ui, i) => new { Useritem = ui, Item = i })
                .Where(x => x.Useritem.UserId == id && x.Item.ItemTypeId == (int)type).Select(x => x.Item).ToListAsync();
        public async Task<List<Item>> GetItemsFromType(ItemType type) 
            => await _context.Item.Where(i => i.ItemTypeId == (int)type).ToListAsync();
        public async Task<int> GetItemAmountAsync(int id, Item item)
        {
            return (await _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem).FirstOrDefaultAsync())!.Amount;
        }

        public async Task<bool> Remove1ItemAsync(int id, Item item, bool isAdmin = false)
        {
            if (isAdmin) return true;
            if (await GetItemAmountAsync(id, item) < 1) return false;

            Useritem? useritem = await _context.Useritem.SingleAsync(ui => ui.UserId == id && ui.ItemId == item.Iditem);
            useritem.Amount--;
            _context.Update(useritem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Item>> GetDiscountedItemsFromTypeAsync(ItemType type)
        {
            List<Item> items = await _context.Item.Where(i => i.ItemTypeId == (int)type).ToListAsync();

            foreach (Item item in items)
            {
                item.Price = await GetDiscountedPriceAsync(item);
            }

            return items;
            
        }

        public async Task<List<Discount>?> GetAllDiscounts()
        {
            return await _context.Discount.ToListAsync();
        }
        public async Task<List<Discount>?> GetAllActiveDiscounts()
        {
            var discounts = _context.Discount.Where(d => d.Startdate < DateTime.Now && d.Enddate > DateTime.Now);
            return await discounts.AnyAsync() ? await discounts.ToListAsync() : null;
        }
    }
}