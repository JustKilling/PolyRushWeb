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
using System.Drawing;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;


namespace PolyRushWeb.Controllers.ApiControllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly SecretSettings _settings;
        private readonly PolyRushWebContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly EmailHelper _emailHelper;

        public AuthenticationController(UserManager<User> userManager,
        SecretSettings settings,
        PolyRushWebContext context,
        IWebHostEnvironment env,
        EmailHelper emailHelper
        )
        {
            _userManager = userManager;
            _settings = settings;
            _context = context;
            _env = env;
            _emailHelper = emailHelper;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequestRegister registration)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (string.IsNullOrWhiteSpace(registration.Avatar))
            {
                registration.Avatar = ImageHelper.ConvertImagePathToBase64String("Media/user.png");
            }

            //Make the user
            User user = new()
            {
                Email = registration.Email,
                UserName = registration.Username,
                Firstname = registration.Firstname,
                Lastname = registration.Lastname,
                IsActive = true,
            };

            //try create the user
            IdentityResult result = await _userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError? error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            //upload image
            string path = Path.Combine(_env.WebRootPath, "img", "user", user.Id.ToString() + ".png");
            using (MemoryStream ms = new(Convert.FromBase64String(registration.Avatar)))
            {
                Bitmap bm = new(ms);
                await bm.SavePNG100(path);
            }

            user = await _userManager.FindByNameAsync(user.UserName);

            //add claim to application user
            await _userManager.AddClaimAsync(user,
            new Claim("registration-date", DateTime.UtcNow.ToString("yy-MM-dd")));


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
            User? applicationUser = _userManager.Users.SingleOrDefault(x => x.Email == login.Email);

            if (applicationUser == null) return Unauthorized();
            if (!applicationUser.IsActive) return Unauthorized();

            PasswordVerificationResult verificationResult = _userManager.PasswordHasher.VerifyHashedPassword(applicationUser, applicationUser.PasswordHash, login.Password);

            //var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false,
            //lockoutOnFailure: false);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                return Unauthorized();
            }
             
            //set last login time claim
            IList<Claim>? claims = await _userManager.GetClaimsAsync(applicationUser);
            Claim? claim = claims.FirstOrDefault(x => x.Type == "last-login-date");
            Claim? updatedClaim = new("last-login-date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
            string? token = await GenerateJwtTokenAsync(applicationUser);

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

        [Authorize(Roles = "Admin")]
        [HttpGet("checkadmin")]
        public IActionResult CheckAdminToken()
        {
            return Ok();
        }

        [HttpGet("forgot-password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest("User not found!");
            string? resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            _emailHelper.SendForgotPasswordEmail(user, resetPasswordToken);
            return Ok();
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ForgotPassword(ResetPasswordModel resetPassword)
        {
            User? user = await _userManager.FindByEmailAsync(resetPassword.Email);
            //in case query string transformed the + in the token to a space.
            resetPassword.Token = resetPassword.Token.Replace(" ", "+");
            var test = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
            if (test.Succeeded) return Ok();
            return BadRequest(test.Errors);
        }

        //een nieuwe JWT aanmaken
        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            List<Claim>? claims = new();

            IList<Claim>? userClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);        
            
            IList<string>? roleClaims = await _userManager.GetRolesAsync(user);

            //Add role claims to jwt
            foreach (string? roleClaim in roleClaims)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleClaim));
            }

            claims.Add(new Claim("id", user.Id.ToString()));

           
            claims.Add(new Claim("isAdmin", (await _userManager.IsInRoleAsync(user, "ADMIN")).ToString()));

            byte[] key = Encoding.ASCII.GetBytes(_settings.TokenSecret);
            JwtSecurityToken? token = new(
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(7)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
                );

            JwtSecurityTokenHandler tokenHandler = new();

            return tokenHandler.WriteToken(token);
        }
    }
}