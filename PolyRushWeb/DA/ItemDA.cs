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
        //return an item by its id
        public async Task<Item> GetItemById(int id) => await _context.Item.Where(i => i.Iditem == id).FirstOrDefaultAsync();
        //create a useritem
        private async Task CreateUserItemAsync(int id, Item item, bool isAdmin = false)
        {
            Useritem useritem = new() { ItemId = item.Iditem, Amount = isAdmin ? 69420 : 0, UserId = id};
            //add the useritem
            await _context.Useritem.AddAsync(useritem);
            //save it
            await _context.SaveChangesAsync();
        }
        //optional parameter for if the user is an admin
        public async Task<bool> BuyItem(int id, Item item, bool isAdmin = false)
        {
            //if useritem doesn't exist, create it.
            if (!await UserItemExistsAsync(id, item))
                await CreateUserItemAsync(id, item, isAdmin);
            
            //get the discounted price
            int price = await GetDiscountedPriceAsync(item);
            //if the user is an admin, set the price to 0;
            if (isAdmin) price = 0;
            //return false if user doesn't have enough coins
            if (!(await _userDa.HasEnoughCoins(id, price))) return false;

           //get the useritem by user and itemid
            Useritem? useritem = _context.Useritem.FirstOrDefault(ui => ui.UserId == id && ui.ItemId == item.Iditem);

            if (useritem != null)
            {
                //add 1 item
                useritem.Amount++;
                //update the db
                _context.Useritem.Update(useritem);
            }

            //Get the user
            User user = _context.Users.FirstOrDefault(u => u.Id == id)!;
            //remove the price
            user!.Coins -= price;
            //update the user
            _context.Users.Update(user);

            //Save the changes
            await _context.SaveChangesAsync();

            return true;
        }
        //remove a discount
        public async Task DeleteDiscount(int id)
        {
            _context.Discount.Remove(new Discount { Iddiscount = id });
            await _context.SaveChangesAsync();
        }
        //add a discount to the table
        public async Task AddDiscount(Discount discount)
        {
            await _context.Discount.AddAsync(discount);
            await _context.SaveChangesAsync();
        }

        //Check if user has a record with this item.
        private async Task<bool> UserItemExistsAsync(int id, Item item) 
            => await _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem).AnyAsync();
        //get all items from an owned type
        //get useritems and join the items. Select where the userid and typeid are right and select the items
        public async Task<List<Item>> GetOwnedItemsFromTypeAsync(int id, ItemType type) => await _context.Useritem
                .Join(_context.Item, ui => ui.ItemId, i => i.Iditem, (ui, i) => new { Useritem = ui, Item = i })
                .Where(x => x.Useritem.UserId == id && x.Item.ItemTypeId == (int)type).Select(x => x.Item).ToListAsync();
        //get all items from a certain type
        public async Task<List<Item>> GetItemsFromType(ItemType type) 
            => await _context.Item.Where(i => i.ItemTypeId == (int)type).ToListAsync();
        //get how much of an item a user has
        public async Task<int> GetItemAmountAsync(int id, Item item)
        {
            return (await _context.Useritem.Where(ui => ui.UserId == id && ui.ItemId == item.Iditem).FirstOrDefaultAsync())!.Amount;
        }
        //remove 1 from the useritem dependant on the itemid and user id
        public async Task<bool> Remove1ItemAsync(int id, Item item, bool isAdmin = false)
        {
            //if user is admin return true
            if (isAdmin) return true;
            //if user has less then 1 return false
            if (await GetItemAmountAsync(id, item) < 1) return false;
            //update the useritem so it has one less. and updata + save it
            Useritem? useritem = await _context.Useritem.SingleAsync(ui => ui.UserId == id && ui.ItemId == item.Iditem);
            useritem.Amount--;
            _context.Update(useritem);
            await _context.SaveChangesAsync();
            return true;
        }
        //get all items from a discounted type
        public async Task<List<Item>> GetDiscountedItemsFromTypeAsync(ItemType type)
        {
            //get all items
            List<Item> items = await _context.Item.Where(i => i.ItemTypeId == (int)type).ToListAsync();
            //get for each item the discounted price
            foreach (Item item in items)
            {
                item.Price = await GetDiscountedPriceAsync(item);
            }
            //return it
            return items;
            
        }
        //return all discounts
        public async Task<List<Discount>?> GetAllDiscounts() => await _context.Discount.ToListAsync();
        //return all active discounts
        public async Task<List<Discount>?> GetAllActiveDiscounts()
        {
            //get all discounts
            var discounts = await _context.Discount.ToListAsync();
            //only select discounts that are still active
            var activeDiscounts = discounts.Where(d => d.Startdate <= DateTime.Now && d.Enddate > DateTime.Now);
            //return to list
            return activeDiscounts.ToList();
        }

        public async Task EditItem(ItemModel model)
        {
            //edit the item entity
            var item = await _context.Item.FindAsync(model.Iditem)!;
            item!.Name = model.Name;
            item.Price = model.Price;
            //save the changes
            await _context.SaveChangesAsync();
        }
    }
}