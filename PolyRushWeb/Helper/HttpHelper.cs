using Microsoft.AspNetCore.Http;

namespace PolyRushWeb.Helper
{
    public class HttpHelper
    {
        private static IHttpContextAccessor _accessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
        }

        public static HttpContext HttpContext => _accessor.HttpContext ?? throw new NullReferenceException("No http context");
    }
}