using FluentEmail.Core;
using PolyRushWeb.Models;

namespace PolyRushWeb.Helper
{
    public class EmailHelper
    {
        private readonly IFluentEmail _fluentEmail;
 

        public EmailHelper(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
   
        }

        public async Task SendForgotPasswordEmail(User user, string resetPasswordToken)
        {
            await _fluentEmail
                .To(user.Email)
                .Subject("Poly Rush | Forgot password")
                .Body("Forgot password token: " + resetPasswordToken)
                .SendAsync();
        }
    }
}
