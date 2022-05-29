using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PolyRushLibrary;
using PolyRushLibrary.Responses;
using PolyRushWeb.Controllers.ApiControllers;
using PolyRushWeb.DA;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClientHelper _clientHelper;
        private readonly AuthenticationHelper _authenticationHelper;

        public HomeController(
            ClientHelper clientHelper,
            AuthenticationHelper authenticationHelper)
        {
            _clientHelper = clientHelper;
            this._authenticationHelper = authenticationHelper;
        }

                   
        public async Task<IActionResult> Index()
        {
            if (!await _authenticationHelper.IsAuthenticatedAsync()) { return RedirectToAction("Logout", "Home"); } //Check if logged in, if not, logout 
            
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync("User");

            string? resultString = await result.Content.ReadAsStringAsync();
            UserDTO? user = JsonConvert.DeserializeObject<UserDTO>(resultString);

            //return the page with the user info
            return View("Index", user);
        }

        public async Task<IActionResult> Profile()
        {
            if (!await _authenticationHelper.IsAuthenticatedAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync("User");

            string? resultString = await result.Content.ReadAsStringAsync();
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(resultString)!;
            
            return View(new UserEditModel()
            {
                Id = user.ID,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Username
            });
        } 
        public async Task<IActionResult> EditProfile(UserEditModel model)
        {
            if (!await _authenticationHelper.IsAuthenticatedAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            if (ModelState.IsValid)
            {
                HttpClient? httpClient = _clientHelper.GetHttpClient();

                UserDTO user = new()
                {
                    ID = model.Id,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Username = model.Username,
                    Email = model.Email,
                };
                HttpRequestMessage request = new(HttpMethod.Post, $"user/update");
                request.Content = new StringContent(JsonConvert.SerializeObject(user), UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.SendAsync(request);
                Console.WriteLine(JsonConvert.SerializeObject(response));
                

                if (model.Image == null) return RedirectToAction(nameof(Profile), model.Id);
                //post image
                byte[] image = new byte[model.Image.Length];
                int bRead = await model.Image.OpenReadStream().ReadAsync(image);
                string base64Image = Convert.ToBase64String(image);


                HttpClient httpClient2 = _clientHelper.GetHttpClient();
                ImageModel img = new() { ImageString = base64Image };
                HttpResponseMessage response2 = await httpClient2.PostAsJsonAsync("user/updateimage", img);

            }

            return RedirectToAction(nameof(Profile), model.Id); 
        }
        public IActionResult Logout()
        {
            _authenticationHelper.Logout();

           //return to start
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveAccount()
        {

            HttpClient httpclient = _clientHelper.GetHttpClient();
            HttpRequestMessage request = new(HttpMethod.Post, "user/deleteaccount");
            var response = await httpclient.SendAsync(request);

            if (!response.IsSuccessStatusCode) return BadRequest();

            _authenticationHelper.Logout();
            return Ok();

        }

        public async Task<IActionResult> Achievements()
        {
            if (!await _authenticationHelper.IsAuthenticatedAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await httpClient.GetAsync($"Achievement");

            if (!response.IsSuccessStatusCode) return View();

            List<Achievement> achievements = JsonConvert.DeserializeObject<List<Achievement>>(await response.Content.ReadAsStringAsync())!;

            return View(achievements);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public async Task<IActionResult> LeaderboardAsync()
        {
            if (!await _authenticationHelper.IsAuthenticatedAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await httpClient.GetAsync($"Leaderboard/{10}");

            List<UserDTO>? topUsers =
                JsonConvert.DeserializeObject<List<UserDTO>>(await response.Content.ReadAsStringAsync());

            response = await httpClient.GetAsync($"Leaderboard/playtime/{10}");

            List<UserPlaytime>? topPlaytimes = 
                JsonConvert.DeserializeObject<List<UserPlaytime>>(await response.Content.ReadAsStringAsync());

            ViewData["TopUsers"] = topUsers;
            ViewData["TopPlaytimes"] = topPlaytimes;

            return View();
        }
        public async Task<IActionResult> Stats()
        {
            if (!await _authenticationHelper.IsAuthenticatedAsync()) { return RedirectToAction("Logout", "Home"); } //Check if admin, if not, logout 
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await httpClient.GetAsync("leaderboard/stats");
            string content = await response.Content.ReadAsStringAsync();
            return View(JsonConvert.DeserializeObject<StatsModel>(content));
        }
       
    }
}
