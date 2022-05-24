using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using PolyRushLibrary;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers
{
    [Secure(true)]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ClientHelper _clientHelper;

        public AdminController(
            UserManager<User> userManager,
            ClientHelper clientHelper)
        {
            _userManager = userManager;
            _clientHelper = clientHelper;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Discount()
        {
            HttpClient httpClient = _clientHelper.GetHttpClient();
            var response = await httpClient.GetAsync("item/getdiscounts");
            if (!response.IsSuccessStatusCode)
                return View(new List<Discount>());
            //Get all discounts 
            List<Discount>? discounts = JsonConvert.DeserializeObject<List<Discount>?>(await response.Content.ReadAsStringAsync());
                  
            //if discounts is null, return an empty list
            return View(discounts ?? new List<Discount>());
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers()
        {
            HttpClient httpClient = _clientHelper.GetHttpClient();
          
            //Get all users 
            List<UserDTO>? users = JsonConvert.DeserializeObject<List<UserDTO>>(await httpClient.GetStringAsync("User/all"));
            return Json(new {data = users});
        }

        public async Task< IActionResult > Edit(int id)
        {
            HttpClient httpClient = _clientHelper.GetHttpClient();

            string response = await httpClient.GetStringAsync($"user/{id}");

            var user = JsonConvert.DeserializeObject<User>(response)!;

            bool isUserAdmin = await _userManager.IsInRoleAsync(user, "ADMIN");
            UserEditAdminModel editModel = user.ToUserEditAdminModel(isUserAdmin);

            return View(editModel);
        } 
        public async Task< IActionResult > EditUser(UserEditAdminModel editModel)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = _clientHelper.GetHttpClient();

                HttpResponseMessage response = await httpClient.PostAsJsonAsync("user/update", editModel);

                return View("Index");

            }
            return View(nameof(Edit));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"User/deactivate/{id}");
            await httpClient.SendAsync(request);
            return View(nameof(Index));
        }
        public async Task<IActionResult> Activate(int id)
        {
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"User/activate/{id}");
            await httpClient.SendAsync(request);
            return View(nameof(Index));
        }
        public async Task<IActionResult> DiscountCreateView()
        {
            //return the view with a discount with present date if discount id is 0.
            //DiscountModel discountModel = discount.Iddiscount == 0 ?
            //    new DiscountModel { Startdate = DateTime.Now, Enddate = DateTime.Now.AddDays(7) } : discount

            var httpClient = _clientHelper.GetHttpClient();

            //Get Items for select
            var response = await httpClient.GetAsync("item/all");

            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(await response.Content.ReadAsStringAsync())!;
            List<SelectListItem> selItems = new();
            foreach (Item item in items)
            {
                selItems.Add(new SelectListItem
                {
                    Value = item.Iditem.ToString(),
                    Text = item.Name
                });
            }
            ViewBag.Items = selItems;

            return View();
        }
        //method for creating a discount
        public async Task<IActionResult> CreateDiscount(DiscountModel discount)
        {
            if (!ModelState.IsValid) return View(nameof(DiscountCreateView), discount);
            //go to the discount action, so show the list again after a discount has been added
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"item/discount");
            //create a json payload
            request.Content = new StringContent(JsonConvert.SerializeObject(discount.ToDiscount()), UnicodeEncoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);
            return RedirectToAction(nameof(Discount));
        }
        //method for deleting a discount
        public async Task<IActionResult> DeleteDiscount(int id)
        {
          
            //go to the discount action, so show the list again after a discount has been added
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"item/deletediscount/{id}");
            var response = await httpClient.SendAsync(request);
            return RedirectToAction(nameof(Discount));
        }
        public async Task<IActionResult> Items()
        {
            HttpClient httpClient = _clientHelper.GetHttpClient();
            var response = await httpClient.GetAsync("item/itemitemtype");

            if (!response.IsSuccessStatusCode)
                return View(new List<ItemItemTypeModel>());
            //Get all itemitemtypes 
        
            List<ItemItemTypeModel> items= JsonConvert.DeserializeObject<List<ItemItemTypeModel>>(await response.Content.ReadAsStringAsync());
            return View(items);
        }
    }
}
