using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PolyRushLibrary;
using PolyRushWeb.DA;

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
        [HttpGet]
        [Route("{itemid}")]
        public async Task<IActionResult> GetItemByIDAsync(int itemid)
        {
            
            return Ok(await _itemDa.GetItemById(itemid));
        }

        [HttpGet]
        [Route("getitemsfromtype/{type}")]
        public async Task<IActionResult> GetItemFromTypeAsync(ItemType type)
        {
            return Ok(await _itemDa.GetItemsFromTypeAsync(type));
        }


        [HttpGet]
        [Authorize]
        [Route("amounts")]
        public async Task<IActionResult> GetAmounts([FromBody] List<Item> items)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = User.IsInRole("Admin");

            List<int> amounts = new();
            foreach (Item? item in items)
            {
                amounts.Add(await _itemDa.GetAmountAsync(item, id, isAdmin));
            }

            return Ok(amounts);
        }

        [HttpPost]
        [Authorize]
        [Route("buy")]
        public async Task<IActionResult> BuyItem([FromBody] Item? item)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = User.IsInRole("Admin");
            if (await _itemDa.BuyItem(id, item, isAdmin)) return Ok();

            return BadRequest("Not enough coins!");
        }

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


        [HttpGet]
        [Authorize]
        [Route("getowneditemsfromtype/{type}")]
        public async Task<IActionResult> GetOwnedItemsFromTypeAsync(ItemType type)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = User.IsInRole("Admin");
            if (isAdmin) return Ok(await _itemDa.GetItemsFromType(type));
            return Ok(await _itemDa.GetOwnedItemsFromTypeAsync(id, type));
        }

        
        [HttpPost]
        [Authorize]
        [Route("useability")]
        public async Task<IActionResult> Remove1ItemAsync([FromBody] Item item)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = User.IsInRole("Admin");

            return Ok(await _itemDa.Remove1ItemAsync(id, item, isAdmin));
        }
    }
}