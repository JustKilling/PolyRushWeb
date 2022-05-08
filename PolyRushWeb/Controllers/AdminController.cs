using System.Collections.Generic;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers()
        {
            var httpClient = _clientHelper.GetHttpClient();

            //Get all users 
            List<UserDTO>? users = JsonConvert.DeserializeObject<List<UserDTO>>(await httpClient.GetStringAsync("User/all"));
            return Json(new {data = users});
        }

        public async Task< IActionResult > Edit(UserEditAdminModel editModel)
        {


            if (ModelState.IsValid)
            {
                var httpClient = _clientHelper.GetHttpClient();

                var response = await httpClient.PostAsJsonAsync("user/update", editModel);

                return View("Index");

            }
            return View();
        }
        public async Task<IActionResult> Deactivate(int id)
        {
            var httpClient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/User/deactivate/{id}");
            await httpClient.SendAsync(request);
            return Ok();
        }
    }
}
