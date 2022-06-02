using Microsoft.EntityFrameworkCore;
using PolyRushWeb.Data;
using PolyRushWeb.Models;


namespace PolyRushWeb.DA
{
    public class ItemDA
    {
        private readonly UserDA _userDa;
        private readonly PolyRushWebContext _context;
        //constructor that injects the dependencies
        public ItemDA(UserDA userDa, PolyRushWebContext context)
        {
            _userDa = userDa;
            _context = context;
        }
        //get the discounted price of an item
        public async Task<int> GetDiscountedPriceAsync(Item i)
        {
            //get the fresh item from the database
            Item item = await GetItemById(i.Iditem)!;

            //get discounted percentage between dates
            IQueryable<Discount> query = _context.Discount.Where(d => d.ItemId == i.Iditem && (d.Startdate < DateTime.Now && DateTime.Now < d.Enddate));
            if (!query.Any()) return item.Price;

            //select only highest percentage if there would be multiple discounts at the same time.
            int discountResult = await query.Select(d => d.DiscountPercentage).MaxAsync();

            //calculated the discounted price
            decimal discountPercentage = Convert.ToDecimal(discountResult);
            decimal discount = Math.Round(item.Price * (discountPercentage / 100m), 2, MidpointRounding.ToEven);
            decimal discountedPrice = item.Price - discount;
            //return the rounded up price
            return (int)Math.Ceiling(discountedPrice);
        }
        //return all items
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
            //return that an admin has 9999 of an item
            if (isAdmin) return 9999;

            //if useritem doesn't exist create it.
            if (!await UserItemExistsAsync(id, item))
                await CreateUserItemAsync(id, item, isAdmin);

            try
            {
                //get useritems inner join the items, select where the useritem is equal to the item id and select the amount the user has from the useritem table.
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
        //return all items from a type
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

            
            User user = _context.Users.FirstOrDefault(u => u.Id == id)!;
            user!.Coins -= price;
            _context.Users.Update(user);


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
            var discounts = await _context.Discount.ToListAsync();
            //only select discounts that are still active
            var activeDiscounts = discounts.Where(d => d.Startdate <= DateTime.Now && d.Enddate > DateTime.Now);
            
            return activeDiscounts.ToList();
        }

        public async Task EditItem(ItemModel model)
        {
            //edit the item entity
            var item = await _context.Item.FindAsync(model.Iditem)!;
            item!.Name = model.Name;
            item.Price = model.Price;
            await _context.SaveChangesAsync();
        }
    }
}