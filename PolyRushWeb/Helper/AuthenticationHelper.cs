using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PolyRushWeb.Helper
{
    public class AuthenticationHelper
    {
        private readonly ClientHelper _clientHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult RedirectResult { get; set; }

        //constructor that injects the dependencies
        public AuthenticationHelper(
            ClientHelper clientHelper, 
            IHttpContextAccessor httpContextAccessor)
        {
            this._clientHelper = clientHelper;
            _httpContextAccessor = httpContextAccessor;            
        }
        //check if user is authenticated
        public async Task<bool> IsAuthenticatedAsync()
        {
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync("check/");
            if (result.IsSuccessStatusCode) return true;
            return false;
        }
        public string Fail()
        {
            //if method is called remove the token and redirect to the homepage
            HttpContext? httpContext = _httpContextAccessor.HttpContext!;
            httpContext.Response.Cookies.Append("Token", "");
            httpContext.Response.Redirect("");
            return "default";
        }

        public void Logout()
        {
            Fail();
        }
        //check if user is admin
        public async Task<bool> IsAdminAsync()
        {
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync("checkadmin/");
            if (result.IsSuccessStatusCode) return true;
            return false;
        }
        //get the current authenticated user
        public async Task<UserDTO> GetAuthenticatedUserAsync()
        {
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync($"User/");
            if (!result.IsSuccessStatusCode) return new UserDTO();
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(await result.Content.ReadAsStringAsync())!;
            return user;
        }
    }
}
