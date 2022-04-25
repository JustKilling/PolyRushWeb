using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PolyRushAPI.DA;
using PolyRushAPI.Models;
using PolyRushAPI.TokenValidators;
using PolyRushLibrary;

namespace PolyRushAPI.Helper
{
    public interface IAuthenticationHelper
    {
        (bool success, string content) Register(string username, string password, string firstname, string lastname,
            string email, string avatar);

        (bool success, string accessToken, string refreshToken) Login(string username, string password);
        (bool success, string accessToken, string refreshToken) Refresh(RefreshRequest refreshRequest);
    }

    public class AuthenticationHelper : IAuthenticationHelper
    {
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly SecretSettings _settings;

        public AuthenticationHelper(SecretSettings settings, RefreshTokenValidator refreshTokenValidator)
        {
            _settings = settings;
            _refreshTokenValidator = refreshTokenValidator;
        }

        public (bool success, string content) Register(string username, string password, string firstname,
            string lastname, string email, string avatar)
        {
            //check if username or email exists
            if (UserDA.UserExists(username) != null) return (false, "Username not available");
            if (UserDA.UserExists(email) != null) return (false, "Email not available");
            User user = new()
            {
                Username = username, Password = password, Firstname = firstname, Lastname = lastname, Email = email,
                Avatar = avatar
            };
            user.ProvideSaltAndHash();
            UserDA.AddUser(user);
            return (true, "");
        }

        public (bool success, string accessToken, string refreshToken) Login(string usernameoremail, string password)
        {
            User? user = UserDA.UserExists(usernameoremail);

            //if user was not found, return invalid
            if (user == null) return (false, "Invalid details!", "Invalid");

            //if password is invalid, return invalid
            if (user.Password != HashAndSaltHelper.ComputeHash(password, user.Salt))
                return (false, "Invalid details!", "Invalid");

            //if user is not active, return invalid
            if (!user.IsActive) return (false, "Invalid details!", "Invalid");

            return (true, GenerateJwtToken(AssembleClaimsIdentity(user)), GenerateJwtRefreshToken());
        }

        public (bool success, string accessToken, string refreshToken) Refresh(RefreshRequest refreshRequest)
        {
            bool isValidRefreshToken = _refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken) return (false, "Invalid refreshtoken", "Invalid");

            RefreshToken refreshToken = UserDA.GetRefreshToken(refreshRequest);
            if (refreshToken == null) return (false, "Refreshtoken not found!", "Invalid");

            User user = UserDA.GetById(refreshToken.User.IDUser);
            if (user == null) return (false, "User not found!", "Invalid");

            string refeshToken = GenerateJwtRefreshToken();
            UserDA.SetRefreshToken(user.Username, refeshToken);
            return (true, GenerateJwtToken(AssembleClaimsIdentity(user)), refeshToken);
        }

        private ClaimsIdentity AssembleClaimsIdentity(User user)
        {
            return new ClaimsIdentity(new[]
            {
                new Claim("id", user.IDUser.ToString()),
                new Claim("isAdmin", user.IsAdmin.ToString())
            });
        }

        //een nieuwe JWT aanmaken
        private string GenerateJwtToken(ClaimsIdentity subject)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_settings.TokenSecret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = subject,
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateJwtRefreshToken()
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_settings.RefreshTokenSecret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Expires = DateTime.Now.AddMonths(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}