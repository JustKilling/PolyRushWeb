using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private readonly IConfiguration _configuration;
        private readonly AuthenticationHelper _authenticationHelper;
        private static HttpClient _httpClient;
        private static string URI;
        public HomeController(
            ClientHelper clientHelper,
            IConfiguration configuration,
            AuthenticationHelper authenticationHelper)
        {
            _clientHelper = clientHelper;
            _configuration = configuration;
            this._authenticationHelper = authenticationHelper;
            URI = _configuration["Api:Uri"];
        }

        /*public IActionResult Index()
        {
            return View();
        }*/
                   
        public async Task<IActionResult> Index()
        {
            
            if(!await _authenticationHelper.IsAuthenticatedAsync())
            {
                try
                {
                    return Unauthorized();
                }
                finally
                {
                    _authenticationHelper.CheckFail(); //Check if logged in, if not, return to start 
                }
            } 


            //return the page with the user info
            return View("Index", new UserDTO());
        }

        public IActionResult Logout()
        {
            //Set the request header
            _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", HttpContext.Session.GetString("Token"));
            //get the response and result
            _httpClient.PostAsync("logout/", new StringContent(""));
           
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Remove("RefreshToken");
            
            //return to start
            return RedirectToAction(nameof(Index));
        }
       
        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public async Task<IActionResult> LeaderboardAsync()
        {
            // TO DO http client
            var listTopHighscore = new List<UserDTO>();
            ViewData["TopUsers"] = listTopHighscore;
            return View();
        }
        public IActionResult Stats()
        {
            return View();
        }
       
    }
}
