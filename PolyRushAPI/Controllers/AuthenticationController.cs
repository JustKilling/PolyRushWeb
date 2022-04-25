using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushAPI.DA;
using PolyRushAPI.Helper;
using PolyRushLibrary;
using PolyRushLibrary.Responses;

namespace PolyRushAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationHelper _authenticationHelper;


        public AuthenticationController(IAuthenticationHelper authenticationHelper)
        {
            _authenticationHelper = authenticationHelper;
        }

        [HttpPost("register")]
        public IActionResult Register(AuthenticationRequestRegister request)
        {
            try
            {
             Console.WriteLine(request);
            (bool success, string content) = _authenticationHelper.Register(request.Username, request.Password,
                request.Firstname, request.Lastname, request.Username, request.Avatar);
            if (!success) return BadRequest(content);

            return Login(request);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }


        [HttpPost("login")]
        public IActionResult Login(AuthenticationRequest request)
        {
            (bool success, string token, string refreshToken) =
                _authenticationHelper.Login(request.Email, request.Password);
            if (!success) return BadRequest(token);

            UserDA.SetRefreshToken(request.Email, refreshToken);

            return Ok(new AuthenticationResponse
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest rr)
        {
            (bool success, string accessToken, string refreshToken) = _authenticationHelper.Refresh(rr);
            if (!success) return BadRequest(accessToken);

            return Ok(new AuthenticationResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
        
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Refresh()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            UserDA.Logout(id);
            return Ok();
        }
    }
}