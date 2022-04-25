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
        public IActionResult GetItemByID(int itemid)
        {
            
            return Ok(_itemDa.GetItemById(itemid));
        }

        [HttpGet]
        [Route("getitemsfromtype/{type}")]
        public IActionResult GetItemFromType(ItemType type)
        {
            string result = JsonConvert.SerializeObject(_itemDa.GetItemsFromType(type));

            return Ok(result);
        }


        [HttpGet]
        [Authorize]
        [Route("amounts")]
        public IActionResult GetAmounts([FromBody] List<Item> items)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            return Ok(items.Select(x => _itemDa.GetAmount(x, id, isAdmin)).ToList());
        }

        [HttpPost]
        [Authorize]
        [Route("buy")]
        public async Task<IActionResult> BuyItem([FromBody] Item? item)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            if (await _itemDa.BuyItem(id, item, isAdmin)) return Ok();

            return BadRequest("Not enough coins!");
        }

        [HttpGet]
        [Authorize]
        [Route("getdiscounteditemsfromtype/{type}")]
        public IActionResult GetDiscountedItemsFromType(ItemType type)
        {
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            List<Item?> items = _itemDa.GetDiscountedItemsFromType(type);
            if (!isAdmin) return Ok(items);
            //if an admin, put the price to 0
            foreach (Item? item in items)
            {
                item.Price = 0;
            }
            return Ok(items);
        }

        //method with and without specifying if you want the image or not
        [HttpGet]
        [Authorize]
        [Route("getowneditemsfromtype/{type}/{getImage}")]
        public IActionResult GetOwnedItemsFromType(ItemType type, bool getImage)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            if (isAdmin) return Ok(_itemDa.GetItemsFromType(type, getImage));
            return Ok(_itemDa.GetOwnedItemsFromType(id, type, getImage));
        }
        [HttpGet]
        [Authorize]
        [Route("getowneditemsfromtype/{type}")]
        public IActionResult GetOwnedItemsFromType(ItemType type)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            if (isAdmin) return Ok(_itemDa.GetItemsFromType(type, true));
            return Ok(_itemDa.GetOwnedItemsFromType(id, type, true));
        }
        
        
        [HttpPost]
        [Authorize]
        [Route("useability")]
        public IActionResult Remove1Item([FromBody] Item item)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            
            return Ok(_itemDa.Remove1Item(id, item, isAdmin));
        }
    }
}