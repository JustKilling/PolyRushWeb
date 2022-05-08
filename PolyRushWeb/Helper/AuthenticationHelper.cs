using Microsoft.AspNetCore.Mvc;

namespace PolyRushWeb.Helper
{
    public class AuthenticationHelper
    {
        private readonly ClientHelper _clientHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult RedirectResult { get; set; }
        public string? FailView { get; set; }

        public AuthenticationHelper(
            ClientHelper clientHelper, 
            IHttpContextAccessor httpContextAccessor)
        {
            this._clientHelper = clientHelper;
            _httpContextAccessor = httpContextAccessor;
            RedirectResult = new RedirectResult("~/");
            
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync("check/");
            if (result.IsSuccessStatusCode) return true;
            return false;
        }
        public string Fail()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext!;
            httpContext.Response.Cookies.Append("Token", "");
            httpContext.Response.Redirect("");
            return "default";
        }

        public void Logout()
        {
            Fail();
        }

        public async Task<bool> IsAdmin()
        {
            HttpClient? httpClient = _clientHelper.GetHttpClient();
            HttpResponseMessage? result = await httpClient.GetAsync("checkadmin/");
            if (result.IsSuccessStatusCode) return true;
            return false;
        }
    }
}
