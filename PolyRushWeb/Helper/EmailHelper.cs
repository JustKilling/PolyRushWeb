using FluentEmail.Core;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Extensions;
using PolyRushWeb.Models;

namespace PolyRushWeb.Helper
{
    public class EmailHelper
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly IServer _server;
        private readonly IHttpContextAccessor _contextAccessor;
       
        //constructor with dependency injection
        public EmailHelper(IFluentEmail fluentEmail, IServer server)
        {
            _fluentEmail = fluentEmail;
            _server = server; ;
            
        }
        //method to send a forget password mail with its token
        public async Task SendForgotPasswordEmailAsync(User user, string resetPasswordToken)
        {
            try
            {
                //get current host name
                string link = _server.Features.Get<IServerAddressesFeature>()?.Addresses.First() 
                    //make the query string
                    + $"/Login/ResetPassword?email={user.Email}&token={resetPasswordToken}";
                
                //make the email body
                string? body = $"<h1>PolyRush</h1>" +
                               $"<p>Dear {user.Firstname} {user.Lastname},<p>" +
                               $"<p>You forgot your password, no problem! You can create a new password if you click on the <a href='{link}' target='_blank'>link</a> below!</p>" +
                               $"{link}" +
                               $"<p>If you did not request to change your password, please contact staff at polyrush@hotmail.com</p>";
                //send the mail
                await _fluentEmail
                    .To(user.Email)
                    .Subject("Poly Rush | Forgot password")
                    .Body(body)
                    .SendAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
         
        }
    }
}
