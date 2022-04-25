using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushAPI.DA;
using PolyRushLibrary.Responses;

namespace PolyRushApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        [HttpGet]
        [Route("{amount}")]
        public IActionResult GetTopUsers(int amount)
        {
            return Ok(LeaderboardDA.GetTopUsers(amount));
        }
        [HttpGet]
        [Route("getnextgoals/{amount}/{score}")]
        public IActionResult GetNextGoals(int amount, int score)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
        
            var response = LeaderboardDA.GetNextGoals(amount, score);
            return Ok(response);
        }
        [HttpGet]
        [Authorize]
        [Route("getnextgoals/{amount}")]
        public IActionResult GetNextGoals(int amount)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            var user = UserDA.GetById(id);
            if (user == null) return BadRequest();
            var response = LeaderboardDA.GetNextGoals(amount, user.Highscore);
            return Ok(response);
        }
        
        // [HttpGet]
        // [Route("random/{username}")]
        // public IActionResult GetTopUsers(string username)
        // {
        //     LeaderboardDA.UpdateRandom(username);
        //     return Ok();
        // }
    }
}