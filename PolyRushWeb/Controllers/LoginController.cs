using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PolyRushLibrary;
using PolyRushLibrary.Responses;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;


namespace PolyRushWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<RegisterModel> _logger;
        private readonly AuthenticationHelper _authenticationHelper;
        private readonly ClientHelper _clientHelper;

        //constructor that injects the dependencies
        public LoginController(
            ClientHelper clientHelper,
            ILogger<RegisterModel> logger,
            AuthenticationHelper authenticationHelper)

        {
            _logger = logger;
            _authenticationHelper = authenticationHelper;
            _clientHelper = clientHelper;
        }
        // GET
        public async Task<IActionResult> Index()
        {
            if (await _authenticationHelper.IsAuthenticatedAsync())
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View("Login");

        }
        public IActionResult Register()
        {
            return View();
        }
        public async Task<IActionResult> Login(LoginInputModel request)
        {
            if (ModelState.IsValid)
            {
                HttpClient? client = _clientHelper.GetHttpClient();
                HttpResponseMessage? response = await client.PostAsJsonAsync("login/", request);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    AuthenticationResponse authenticationResponse =
                        JsonConvert.DeserializeObject<AuthenticationResponse>(responseString)!;

                    CookieOptions cookieOptions = new()
                    {
                        Secure = true,
                        Expires = DateTime.Now.AddDays(7),
                    };

                    HttpContext.Response.Cookies.Append("Token", authenticationResponse.Token, cookieOptions);
                    return RedirectToAction("Index", "Home");
                }
            }
         
            ModelState.AddModelError("Invalid", "Invalid Details");
            HttpContext.Session.SetInt32("IsInvalidLogin", 1);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> IsEmailInUse(string email)
        {
            HttpClient? client = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await client.GetAsync($"user/isemailinuse?email={email}");
            if (response.IsSuccessStatusCode)
            {
                var isUsed = Convert.ToBoolean(await response.Content.ReadAsStringAsync());
                return Json(!isUsed);
            }

            //if something is wrong, prevent user from registering.
            return Json(false);
        }
        public async Task<IActionResult> IsUsernameInUse(string username)
            {
                HttpClient? client = _clientHelper.GetHttpClient();
                HttpResponseMessage? response = await client.GetAsync($"user/isusernameinuse?username={username}");
                if (response.IsSuccessStatusCode)
                {
                    var isUsed = Convert.ToBoolean(await response.Content.ReadAsStringAsync());
                    return Json(!isUsed);
                }

                //if something is wrong, prevent user from registering.
                return Json(false);
            }

        public async Task<IActionResult> RegisterUser(RegisterModel request)
        {
            HttpClient? client = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await client.PostAsJsonAsync("register/", request);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Login), request);
            }
            return RedirectToAction(nameof(Register));
        }
        public IActionResult ForgotPassword()
        {
            return View();
        } 
        public async Task<IActionResult> SendForgotPasswordMail(ForgotPasswordModel model)
        {
            HttpClient? client = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await client.GetAsync($"forgot-password/{model.Email}");
            return RedirectToAction(nameof(Login));
        }
        public IActionResult ResetPassword([FromQuery] string email, [FromQuery] string token)
        {
            
            return View(new ResetPasswordModel(){Token = token, Email = email});
            HttpContext?.Session.SetInt32("IsInvalidLogin", 0);

        }
        public async Task<IActionResult> ResetPasswordAction(ResetPasswordModel resetPassword)
        {
            HttpClient? client = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await client.PostAsJsonAsync($"reset-password", resetPassword);
            
            if(response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            return Content(await response.Content.ReadAsStringAsync());
        }
    }
}