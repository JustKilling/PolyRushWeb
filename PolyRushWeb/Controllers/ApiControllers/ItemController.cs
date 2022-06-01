using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PolyRushLibrary;
using PolyRushWeb.DA;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ItemDA _itemDa;

        public ItemController(ItemDA itemDa)
        {
            _itemDa = itemDa;
        }
        //Get item item types, thats an object that holds the item and its type
        [HttpGet]
        [Route("itemitemtype")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetItemItemTypes()
        {

            return Ok(await _itemDa.GetItemItemTypes());
        }
        //Get an item by its id
        [HttpGet]
        [Route("{itemid}")]
        public async Task<IActionResult> GetItemByIDAsync(int itemid)
        {
            
            return Ok(await _itemDa.GetItemById(itemid));
        }
        //return all items
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllItems()
        {

            return Ok(await _itemDa.GetAllItemsAsync());
        }
        //get all items from type
        [HttpGet]
        [Route("getitemsfromtype/{type}")]
        public async Task<IActionResult> GetItemFromTypeAsync(ItemType type)
        {
            return Ok(await _itemDa.GetItemsFromTypeAsync(type));
        }

        //get how much items a user has per given item.
        [HttpGet]
        [Authorize]
        [Route("amounts")]
        public async Task<IActionResult> GetAmounts([FromBody] List<Item> items)
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = User.IsInRole("Admin");

            List<int> amounts = new();
            foreach (Item? item in items)
            {
                amounts.Add(await _itemDa.GetAmountAsync(item, id, isAdmin));
            }

            string? _ = JsonConvert.SerializeObject(amounts);
            return Ok(_);
        }
        //method to try and buy and item, if not enough coins return a badrequest
        [HttpPost]
        [Authorize]
        [Route("buy")]
        public async Task<IActionResult> BuyItem([FromBody] Item? item)
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = User.IsInRole("Admin");
            if (await _itemDa.BuyItem(id, item, isAdmin)) return Ok();

            return BadRequest("Not enough coins!");
        }
        //get all discounted items from typ
        [HttpGet]
        [Authorize]
        [Route("getdiscounteditemsfromtype/{type}")]
        public async Task<IActionResult> GetDiscountedItemsFromTypeAsync(ItemType type)
        {
            bool isAdmin = User.IsInRole("Admin");
            List<Item> items = await _itemDa.GetDiscountedItemsFromTypeAsync(type);
            if (!isAdmin) return Ok(items);
            //if an admin, put the price to 0
            foreach (Item? item in items)
            {
                item.Price = 0;
            }
            return Ok(items);
        }


        //method to get all owned items from a certain type
        [HttpGet]
        [Authorize]
        [Route("getowneditemsfromtype/{type}")]
        public async Task<IActionResult> GetOwnedItemsFromTypeAsync(ItemType type)
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            //Return all if admin
            bool isAdmin = User.IsInRole("Admin");
            if (isAdmin) return Ok(await _itemDa.GetItemsFromType(type));
            
            return Ok(await _itemDa.GetOwnedItemsFromTypeAsync(id, type));
        }
        //method to get all discounts
        [HttpGet]
        [AllowAnonymous]
        [Route("getdiscounts")]
        public async Task<IActionResult> GetDiscounts()
        {
            List<Discount>? discounts = await _itemDa.GetAllDiscounts();
            if (discounts == null) return BadRequest("No discounts found!");
            return Ok(discounts);
        }
        //method to get all active discounts
        [HttpGet]
        [AllowAnonymous]
        [Route("getactivediscounts")]
        public async Task<IActionResult> GetActiveDiscounts()
        {
            List<Discount>? discounts = await _itemDa.GetAllActiveDiscounts();
            if (discounts == null) return BadRequest("No active discounts found!");
            return Ok(discounts);
        }

        //method to add a discount
        [HttpPost]
        [Route("discount")]
        public async Task<IActionResult> Discount(Discount discount)
        {
            await _itemDa.AddDiscount(discount);
            return Ok();
        }
        //methed to delete a discount by its id
        [HttpPost]
        [Route("deletediscount/{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Discount(int id)
        {
            await _itemDa.DeleteDiscount(id);
            return Ok();
        }
        //method to edit an item
        [HttpPost]
        [Route("edit")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Edititem(ItemModel model)
        {
            await _itemDa.EditItem(model);
            return Ok();
        }

        //method that removes one ability from the user
        [HttpPost]
        [Authorize]
        [Route("useability")]
        public async Task<IActionResult> Remove1ItemAsync([FromBody] Item item)
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = User.IsInRole("Admin");

            return Ok(await _itemDa.Remove1ItemAsync(id, item, isAdmin));
        }
    }
}