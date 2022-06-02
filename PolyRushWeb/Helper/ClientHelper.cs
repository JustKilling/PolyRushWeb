using System.Net.Http.Headers;
using System.Web;
namespace PolyRushWeb.Helper
{
    public class ClientHelper
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccesor;
        //constructor that injects the dependencies
        public ClientHelper(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContext)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccesor = httpContext;
        }
        //method to get a httpclient with authorizatioin header
        public HttpClient GetHttpClient()
        {
            HttpClient? httpClient = _httpClientFactory.CreateClient("api");

            string? token = _httpContextAccesor.HttpContext?.Request.Cookies["token"] ?? "";

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return httpClient;
        }
        
    }
}
