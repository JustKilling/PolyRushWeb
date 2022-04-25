using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PolyRushAPI.DA;
using PolyRushLibrary;

namespace PolyRushAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        [HttpGet]
        [Route("{itemid}")]
        public IActionResult GetItemByID(int itemid)
        {
            
            return Ok(ItemDA.GetItemById(itemid));
        }

        [HttpGet]
        [Route("getitemsfromtype/{type}")]
        public IActionResult GetItemFromType(ItemType type)
        {
            string result = JsonConvert.SerializeObject(ItemDA.GetItemsFromType(type));

            return Ok(result);
        }


        [HttpGet]
        [Authorize]
        [Route("amounts")]
        public IActionResult GetAmounts([FromBody] List<Item> items)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            return Ok(items.Select(x => ItemDA.GetAmount(x, id, isAdmin)).ToList());
        }

        [HttpPost]
        [Authorize]
        [Route("buy")]
        public IActionResult BuyItem([FromBody] Item? item)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            if (ItemDA.BuyItem(id, item, isAdmin)) return Ok();

            return BadRequest("Not enough coins!");
        }

        [HttpGet]
        [Authorize]
        [Route("getdiscounteditemsfromtype/{type}")]
        public IActionResult GetDiscountedItemsFromType(ItemType type)
        {
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            var items = ItemDA.GetDiscountedItemsFromType(type);
            if (!isAdmin) return Ok(items);
            //if an admin, put the price to 0
            foreach (var item in items)
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
            if (isAdmin) return Ok(ItemDA.GetItemsFromType(type, getImage));
            return Ok(ItemDA.GetOwnedItemsFromType(id, type, getImage));
        }
        [HttpGet]
        [Authorize]
        [Route("getowneditemsfromtype/{type}")]
        public IActionResult GetOwnedItemsFromType(ItemType type)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            if (isAdmin) return Ok(ItemDA.GetItemsFromType(type, true));
            return Ok(ItemDA.GetOwnedItemsFromType(id, type, true));
        }
        
        
        [HttpPost]
        [Authorize]
        [Route("useability")]
        public IActionResult Remove1Item([FromBody] Item item)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            bool isAdmin = bool.Parse(User.Claims.First(i => i.Type == "isAdmin").Value);
            
            return Ok(ItemDA.Remove1Item(id, item, isAdmin));
        }
    }
}