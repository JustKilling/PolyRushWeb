using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PolyRushAPI.Models;

namespace PolyRushAPI.TokenValidators
{
    public class RefreshTokenValidator
    {
        private readonly SecretSettings _secretSettings;

        public RefreshTokenValidator(SecretSettings secretSettings)
        {
            _secretSettings = secretSettings;
        }

        public bool Validate(string refreshToken)
        {
            TokenValidationParameters parameters = new()
            {
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretSettings.RefreshTokenSecret)),
                ValidateIssuerSigningKey = true,
                //don't validate client
                ValidateAudience = false,
                ValidateIssuer = false
            };
            JwtSecurityTokenHandler handler = new();
            try
            {
                handler.ValidateToken(refreshToken, parameters, out SecurityToken securityToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}