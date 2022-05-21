using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            //Get all users 
            List<UserDTO>? users = JsonConvert.DeserializeObject<List<UserDTO>>(await httpClient.GetStringAsync("/all"));
            
            return View();
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
         

            UserEditAdminModel editModel = JsonConvert.DeserializeObject<User>(response)!.ToUserEditAdminModel();

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
    }
}
