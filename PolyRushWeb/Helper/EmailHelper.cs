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
            //get current host name
            string link = _server.Features.Get<IServerAddressesFeature>()?.Addresses.First() + $"/Login/ResetPassword?token={resetPasswordToken}&email={user.Email}";
            await _fluentEmail
                .To(user.Email)
                .Subject("Poly Rush | Forgot password")
                .Body("Forgot password token: " + link)
                .SendAsync();
        }
    }
}
