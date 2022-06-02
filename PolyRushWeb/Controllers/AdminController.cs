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
    
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ClientHelper _clientHelper;
        private readonly AuthenticationHelper _authenticationHelper;

        //constructor that injects the dependencies
        public AdminController(
            UserManager<User> userManager,
            ClientHelper clientHelper,
            AuthenticationHelper authenticationHelper)
        {
            _userManager = userManager;
            _clientHelper = clientHelper;
            _authenticationHelper = authenticationHelper;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            return View();
        }
        public async Task<IActionResult> Discount()
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 

            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage response = await httpClient.GetAsync("item/getactivediscounts");
            if (!response.IsSuccessStatusCode)
                return View(new List<Discount>());
            //Get all discounts 
            List<Discount>? discounts = JsonConvert.DeserializeObject<List<Discount>?>(await response.Content.ReadAsStringAsync());
                  
            //if discounts is null, return an empty list
            return View(discounts ?? new List<Discount>());
        }
        public async Task<IActionResult> GetUsers()
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient httpClient = _clientHelper.GetHttpClient();

            //Get all users 
            List<UserDTO>? users = JsonConvert.DeserializeObject<List<UserDTO>>(await httpClient.GetStringAsync("User/all"));
            return Json(new {data = users});
        }

        public async Task< IActionResult > Edit(int id)
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient httpClient = _clientHelper.GetHttpClient();

            string response = await httpClient.GetStringAsync($"user/{id}");

            User user = JsonConvert.DeserializeObject<User>(response)!;

            bool isUserAdmin = await _userManager.IsInRoleAsync(user, "ADMIN");
            UserEditAdminModel editModel = user.ToUserEditAdminModel(isUserAdmin);

            return View(editModel);
        } 
        public async Task< IActionResult > EditUser(UserEditAdminModel editModel)
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            if (ModelState.IsValid)
            {
                HttpClient httpClient = _clientHelper.GetHttpClient();

                HttpResponseMessage response = await httpClient.PostAsJsonAsync("user/updateadmin", editModel);

                return View("Index");

            }
            return View(nameof(Edit));
        }
        public async Task< IActionResult > EditItem(int id)
        {
            HttpClient httpClient = _clientHelper.GetHttpClient();
            var response = await httpClient.GetStringAsync($"Item/{id}");
            Item item = JsonConvert.DeserializeObject<Item>(response);
            var itemModel = new ItemModel()
            {
                Iditem = item.Iditem,
                Name = item.Name,
                Price = item.Price
            };
            return View(itemModel);
        }
        public async Task< IActionResult > EditItemAction(ItemModel model)
        {
            //make sure model is valid
            if (ModelState.IsValid)
            {
                HttpClient httpClient = _clientHelper.GetHttpClient();

                HttpResponseMessage response = await httpClient.PostAsJsonAsync("item/edit", model);

                return RedirectToAction(nameof(Items));

            }
            return View(nameof(EditItem));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"User/deactivate/{id}");
            await httpClient.SendAsync(request);
            return View(nameof(Index));
        }
        public async Task<IActionResult> Activate(int id)
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"User/activate/{id}");
            await httpClient.SendAsync(request);
            return View(nameof(Index));
        }
        public async Task<IActionResult> DiscountCreateView()
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            
            HttpClient httpClient = _clientHelper.GetHttpClient();

            //Get Items for select
            HttpResponseMessage response = await httpClient.GetAsync("item/all");

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
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 

            if (!ModelState.IsValid) return View(nameof(DiscountCreateView), discount);
            //go to the discount action, so show the list again after a discount has been added
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"item/discount");
            //create a json payload
            request.Content = new StringContent(JsonConvert.SerializeObject(discount.ToDiscount()), UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.SendAsync(request);
            return RedirectToAction(nameof(Discount));
        }
        //method for deleting a discount
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 

            //go to the discount action, so show the list again after a discount has been added
            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, $"item/deletediscount/{id}");
            HttpResponseMessage response = await httpClient.SendAsync(request);
            return RedirectToAction(nameof(Discount));
        }
        public async Task<IActionResult> Items()
        {
            if (!await _authenticationHelper.IsAdminAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 

            HttpClient httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage response = await httpClient.GetAsync("item/itemitemtype");

            if (!response.IsSuccessStatusCode)
                return View(new List<ItemItemTypeModel>());
            //Get all itemitemtypes 
            List<ItemItemTypeModel> items= JsonConvert.DeserializeObject<List<ItemItemTypeModel>>(await response.Content.ReadAsStringAsync());
            return View(items);
        }

    }
}
