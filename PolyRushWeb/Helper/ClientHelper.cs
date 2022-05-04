using System.Net.Http.Headers;
using System.Web;
namespace PolyRushWeb.Helper
{
    public class ClientHelper
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public ClientHelper(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContext)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccesor = httpContext;
        }

        public HttpClient GetHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient("api");

            var token = _httpContextAccesor.HttpContext?.Request.Cookies["token"];
            if (token == null) token = "";
          
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return httpClient;
        }
        
    }
}
