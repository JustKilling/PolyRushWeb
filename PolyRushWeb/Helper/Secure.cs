using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using PolyRushLibrary;
using PolyRushLibrary.Responses;

namespace PolyRushWeb.Helper
{
    [AttributeUsage(AttributeTargets.All)]

    public class Secure : Attribute, IActionFilter
    {
        private HttpClient? _client;
        private bool isAdmin;
        public Secure(bool admin = false)
        {
            isAdmin = admin;
            SetHttpClient();
        }

        private void SetHttpClient()
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            SetHttpClient();
            string? refreshToken = HttpHelper.HttpContext.Session.GetString("RefreshToken");
            HttpResponseMessage result = _client.PostAsync("refresh", new StringContent(
                JsonConvert.SerializeObject(new RefreshRequest
                {
                    RefreshToken = refreshToken
                }), Encoding.UTF8, "application/json")).Result;
            if (!result.IsSuccessStatusCode)
            {
                //if no permission, log the user out.
                context.Result = new ViewResult{ViewName = "Login"};
                context.HttpContext.Session.Remove("Token");
                context.HttpContext.Session.Remove("RefreshToken");
                return;
            }
            
            AuthenticationResponse response =
                JsonConvert.DeserializeObject<AuthenticationResponse>(result.Content.ReadAsStringAsync().Result);
            context.HttpContext.Session.SetString("Token", response.Token);
            context.HttpContext.Session.SetString("RefreshToken", response.RefreshToken);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _client?.Dispose();
            _client = null;
        }
    }
}