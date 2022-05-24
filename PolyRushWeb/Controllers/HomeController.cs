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
            //Check if logged in, if not, return to start 
            if (!await _authenticationHelper.IsAuthenticatedAsync())
            {
                return RedirectToAction("Index","Login");
            }

            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync("User");

            string? resultString = await result.Content.ReadAsStringAsync();
            UserDTO? user = JsonConvert.DeserializeObject<UserDTO>(resultString);

            //return the page with the user info
            return View("Index", user);
        }

        public IActionResult Logout()
        {
           _authenticationHelper.Logout();

           //return to start
            return RedirectToAction("Index", "Login");
        }
       
        public async Task<IActionResult> Achievements()
        {
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
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await httpClient.GetAsync("leaderboard/stats");
            var content = await response.Content.ReadAsStringAsync();
            return View(JsonConvert.DeserializeObject<StatsModel>(content));
        }
       
    }
}
