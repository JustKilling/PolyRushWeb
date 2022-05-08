using System;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
                RedirectToAction(nameof(Login));
            }
            return View("Login");
        }
        public IActionResult Register()
        {
            return View();
        }
        public async Task<IActionResult> Login(LoginInputModel request)
        {
            if (await _authenticationHelper.IsAuthenticatedAsync())
            {
                return RedirectToAction("Index", "Home");
            }


            HttpClient? client = _clientHelper.GetHttpClient();
            HttpResponseMessage? response = await client.PostAsJsonAsync("login/", request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                AuthenticationResponse authenticationResponse =
                    JsonConvert.DeserializeObject<AuthenticationResponse>(responseString)!;

                var cookieOptions = new CookieOptions
                {
                    Secure = true,
                    Expires = DateTime.Now.AddDays(7),
                };

                HttpContext.Response.Cookies.Append("Token", authenticationResponse.Token, cookieOptions);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> Logout()
        //{
        //    throw new NotImplementedException();
        //}
        //public async Task<IActionResult> Login(LoginInputModel loginInput)
        //{

        //    if (ModelState.IsValid)
        //    {

        //        //var result = await _signInManager.PasswordSignInAsync(loginInput.Email, loginInput.Password, loginInput.RememberMe, lockoutOnFailure: false);
        //        //if (result.Succeeded)
        //        //{
        //        //    return RedirectToAction(nameof(Test));

        //        //}
        //        //if (result.RequiresTwoFactor)
        //        //{
        //        //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = "./", RememberMe = loginInput.RememberMe });
        //        //}
        //        //if (result.IsLockedOut)
        //        //{
        //        //    return RedirectToPage("./Lockout");
        //        //}
        //        //else
        //        //{
        //        //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //        //    return View();
        //        //}

        //    }
        //    return View();

        //}

        //[Authorize]
        //public IActionResult Test()
        //{
        //    return Content("Works");
        //}

        //public async Task<IActionResult> RegisterUser(RegisterModel registerModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //var user = CreateUser();
        //        //await _userStore.SetUserNameAsync(user, registerModel.Email, CancellationToken.None);
        //        //await _emailStore.SetEmailAsync(user, registerModel.Email, CancellationToken.None);
        //        //var result = await _userManager.CreateAsync(user, registerModel.Password);

        //        //if (result.Succeeded)
        //        //{
        //        //    _logger.LogInformation("User created a new account with password.");

        //        //    var userId = await _userManager.GetUserIdAsync(user);
        //        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //        //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //        //    var callbackUrl = Url.Page(
        //        //        "/Account/ConfirmEmail",
        //        //        pageHandler: null,
        //        //        values: new { area = "Identity", userId = userId, code = code, returnUrl = "./" },
        //        //        protocol: Request.Scheme);

        //        //    await _emailSender.SendEmailAsync(registerModel.Email, "Confirm your email",
        //        //        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //        //    if (_userManager.Options.SignIn.RequireConfirmedAccount)
        //        //    {
        //        //        return RedirectToPage("RegisterConfirmation", new { email = registerModel.Email, returnUrl = "./" });
        //        //    }
        //        //    else
        //        //    {
        //        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        //        return RedirectToAction("Index", "Home");
        //        //    }
        //        //}
        //        //foreach (var error in result.Errors)
        //        //{
        //        //    ModelState.AddModelError(string.Empty, error.Description);
        //        //}

        //    }
        //    return View("Register");
        //}
        //private User CreateUser()
        //{
        //    try
        //    {
        //        return Activator.CreateInstance<User>();
        //    }
        //    catch
        //    {
        //        throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
        //            $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
        //            $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        //    }
        //}
        //private IUserEmailStore<User> GetEmailStore()
        //{
        //    //if (!_userManager.SupportsUserEmail)
        //    //{
        //    //    throw new NotSupportedException("The default UI requires a user store with email support.");
        //    //}
        //    //return (IUserEmailStore<User>)_userStore;
        //    return null;
        //}
    }
}