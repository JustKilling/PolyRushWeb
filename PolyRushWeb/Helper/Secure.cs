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
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _client?.Dispose();
            _client = null;
        }
    }
}