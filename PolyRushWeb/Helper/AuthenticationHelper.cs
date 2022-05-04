using Microsoft.AspNetCore.Mvc;

namespace PolyRushWeb.Helper
{
    public class AuthenticationHelper
    {
        private readonly ClientHelper _clientHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult RedirectResult { get; set; }

        public AuthenticationHelper(ClientHelper clientHelper, IHttpContextAccessor httpContextAccessor)
        {
            this._clientHelper = clientHelper;
            _httpContextAccessor = httpContextAccessor;
            RedirectResult = new RedirectResult("~/");
            
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var httpClient = _clientHelper.GetHttpClient();
            var result = await httpClient.GetAsync("/check/");
            if (result.IsSuccessStatusCode) return true;
            return false;
        }
        public void CheckFail()
        {
            var httpContext = _httpContextAccessor.HttpContext!;
            httpContext.Response.Redirect("~/");
            httpContext.Response.Cookies.Delete("token"); 
        }
    }
}
