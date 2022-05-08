using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.DA;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDA _userDa;

        public UserController(UserDA userDa)
        {
            _userDa = userDa;
        }
        [HttpGet("ishighscore/{score}")]
        public async Task<IActionResult> IsHighscore(int score)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok((await _userDa.GetByIdAsync(id))?.Highscore < score);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _userDa.GetUsers());
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
         {
            //id ophalen uit jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            User? user = await _userDa.GetByIdAsync(id);
            //check if user with that id exists
            if (user == null) return BadRequest("User not found!");
            return Ok(user);
        }

        [HttpGet]
        [Route("coins")]
        public async Task<IActionResult> GetCoins()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _userDa.GetCoinsAsync(id));
        }

        [HttpPatch]
        public async Task<IActionResult> PatchAsync(UserDTO userDto)
        {
            var user = await _userDa.GetByIdAsync(userDto.ID);
            user.IsAdmin = userDto.IsAdmin;
            user.Highscore = userDto.Highscore;
            user.Coins = userDto.Coins;
            user.Coinsgathered = userDto.Coinsgathered;
            user.Coinsspent = userDto.Coinsspent;
            user.Firstname = userDto.Firstname;
            user.Lastname = userDto.Lastname;
            user.SeesAds = userDto.SeesAds;

            await _userManager.SetUserNameAsync(user, editModel.Username);
            await _userManager.SetEmailAsync(user, editModel.Email);

            //change the password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _userManager.ResetPasswordAsync(user, token, editModel.Password);
        }

        [HttpPost]
        [Route("removecoins/{amount}")]
        public async Task<IActionResult> RemoveCoins(int amount)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _userDa.RemoveCoins(id, amount));
        }
        [HttpPost]
        [Route("removecoins")]
        public async Task<IActionResult> RemoveCoins()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _userDa.RemoveCoins(id));
        }
    }
}