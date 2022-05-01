using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.DA;

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

            return Ok((await _userDa.GetById(id))?.Highscore < score);
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
            var user = await _userDa.GetById(id);
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