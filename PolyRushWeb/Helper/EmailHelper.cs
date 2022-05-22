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
       

        public EmailHelper(IFluentEmail fluentEmail, IServer server)
        {
            _fluentEmail = fluentEmail;
            _server = server; ;
            
        }

        public async Task SendForgotPasswordEmail(User user, string resetPasswordToken)
        {
            try
            {
                //get current host name
                string link = _server.Features.Get<IServerAddressesFeature>()?.Addresses.First() 
                    + $"/Login/ResetPassword?email={user.Email}&token={resetPasswordToken}";
                Console.WriteLine(link);

                var body = $"Dear {user.Firstname} {user.Lastname},\n" +
                           $"You forgot your password, no problem! You can create a new password if you click on the link below!\n\n" +
                           $"{link}\n\n" +
                           $"If you did not request to change your password, please contact staff at polyrush@hotmail.com";

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
