*using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PolyRushWeb.Helper
{
    public class Secure : AuthorizeAttribute, IAuthorizationFilter
    {
        /// Checks to see if the user is authenticated and has a valid session object
        

        public void OnAuthorization(AuthorizationFilterContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException("httpContext");

            // Make sure the user is authenticated.
            if (httpContext.User.Identity.IsAuthenticated == false) return false;

            // This will check my session variable and a few other things.
            return Helpers.SecurityHelper.IsSignedIn();
        }
    }
}*/