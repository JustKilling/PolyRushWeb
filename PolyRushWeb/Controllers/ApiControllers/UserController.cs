using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushAPI.DA;

namespace PolyRushApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet("ishighscore/{score}")]
        [Authorize]
        public IActionResult IsHighscore(int score)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok(UserDA.GetById(id)?.Highscore < score);
        }
        

        [HttpGet]
        public IActionResult Get()
        {
            //id ophalen uit jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok(UserDA.GetById(id));
        }

        [HttpGet]
        [Route("coins")]
        public IActionResult GetCoins()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(UserDA.GetCoins(id));
        }
        
        [HttpPost]
        [Route("removecoins/{amount}")]
        public IActionResult RemoveCoins(int amount)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(UserDA.RemoveCoins(id, amount));
        }
        [HttpPost]
        [Route("removecoins")]
        public IActionResult RemoveCoins()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(UserDA.RemoveCoins(id));
        }
    }
}