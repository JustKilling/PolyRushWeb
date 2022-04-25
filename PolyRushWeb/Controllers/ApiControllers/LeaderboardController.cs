using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushLibrary;
using PolyRushLibrary.Responses;
using PolyRushWeb.DA;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly UserDA _userDa;
        private readonly LeaderboardDA _leaderboardDa;

        public LeaderboardController(UserDA userDa, LeaderboardDA leaderboardDa)
        {
            _userDa = userDa;
            _leaderboardDa = leaderboardDa;
        }
        [HttpGet]
        [Route("{amount}")]
        public IActionResult GetTopUsers(int amount)
        {
            return Ok(_leaderboardDa.GetTopUsers(amount));
        }
        [HttpGet]
        [Route("getnextgoals/{amount}/{score}")]
        public IActionResult GetNextGoals(int amount, int score)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
        
            List<NextGoalResponse> response = _leaderboardDa.GetNextGoals(amount, score);
            return Ok(response);
        }
        [HttpGet]
        [Authorize]
        [Route("getnextgoals/{amount}")]
        public async Task<IActionResult> GetNextGoals(int amount)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            User? user = await _userDa.GetById(id);
            if (user == null) return BadRequest();
            List<NextGoalResponse> response = _leaderboardDa.GetNextGoals(amount, user.Highscore);
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