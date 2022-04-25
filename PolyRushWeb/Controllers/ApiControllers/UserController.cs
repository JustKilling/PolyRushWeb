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
        [Authorize]
        public async Task<IActionResult> IsHighscore(int score)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok((await _userDa.GetById(id))?.Highscore < score);
        }
        

        [HttpGet]
        public IActionResult Get()
        {
            //id ophalen uit jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok(_userDa.GetById(id));
        }

        [HttpGet]
        [Route("coins")]
        public IActionResult GetCoins()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(_userDa.GetCoinsAsync(id));
        }
        
        [HttpPost]
        [Route("removecoins/{amount}")]
        public IActionResult RemoveCoins(int amount)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(_userDa.RemoveCoins(id, amount));
        }
        [HttpPost]
        [Route("removecoins")]
        public IActionResult RemoveCoins()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(_userDa.RemoveCoins(id));
        }
    }
}