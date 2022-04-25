using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PolyRushLibrary;
using PolyRushLibrary.Responses;
using PolyRushWeb.DA;
using PolyRushWeb.Data;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly SecretSettings _settings;
        private readonly polyrushContext _context;

        public AuthenticationController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        SecretSettings settings,
        polyrushContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _settings = settings;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequestRegister registration)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (string.IsNullOrWhiteSpace(registration.Avatar))
            {
                registration.Avatar = ImageToBase64Helper.ConvertImagePathToBase64String("Media/user.png");
            }

            //Make the user6
            User user = new()
            {
                Email = registration.Email,
                UserName = registration.Username,
                Avatar = registration.Avatar,
                Firstname = registration.Firstname,
                Lastname = registration.Lastname,
            };

            //try create the user
            IdentityResult result = await _userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            user = await _userManager.FindByNameAsync(user.UserName);

            //add claim to application user
            await _userManager.AddClaimAsync(user,
            new("registration-date", DateTime.UtcNow.ToString("yy-MM-dd")));


            //Save changes
            await _context.SaveChangesAsync();


            return await Login(registration);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] AuthenticationRequest login)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            //Get the matched email user details
            var applicationUser = _userManager.Users.SingleOrDefault(x => x.Email == login.Email);

            if (applicationUser == null) return Unauthorized();


            var verificationResult = _userManager.PasswordHasher.VerifyHashedPassword(applicationUser, applicationUser.PasswordHash, login.Password);

            //var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false,
            //lockoutOnFailure: false);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                return Unauthorized();
            }
             
            //set last login time claim
            var claims = await _userManager.GetClaimsAsync(applicationUser);
            Claim? claim = claims.FirstOrDefault(x => x.Type == "last-login-date");
            var updatedClaim = new Claim("last-login-date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (claim == null)
            {
                await _userManager.AddClaimsAsync(applicationUser, new Claim[]{ updatedClaim });
            }
            else
            {
                await _userManager.ReplaceClaimAsync(applicationUser, claim, updatedClaim);
            }

            applicationUser.LastLoginTime = DateTime.Now;

            _context.Users.Update(applicationUser);

            //Get the token
            var token = await GenerateJwtTokenAsync(applicationUser);

            //Save changes
            await _context.SaveChangesAsync();

            return Ok(new AuthenticationResponse()
            {
                Token = token
            });
        }

        [Authorize]
        [HttpGet("check")]
        public IActionResult CheckToken()
        {
            return Ok();
        }


        //een nieuwe JWT aanmaken
        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>();

            var userClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);        

            var roleClaims = await _userManager.GetClaimsAsync(user);

            //claims.AddRange(roleClaims);
            //Add role claims to jwt
            foreach (var roleClaim in roleClaims)
            {
                claims.Add(new(ClaimTypes.Role, roleClaim.Value));
            }

            claims.Add(new("id", user.Id.ToString()));
            claims.Add(new("isAdmin", (await _userManager.IsInRoleAsync(user, "ADMIN")).ToString()));

            byte[] key = Encoding.ASCII.GetBytes(_settings.TokenSecret);
            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(7)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
                );

            JwtSecurityTokenHandler tokenHandler = new();

            return tokenHandler.WriteToken(token);
        }
    }
}