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
        public async Task<IActionResult> GetTopUsers(int amount)
        {
            var result = await _leaderboardDa.GetTopUsers(amount);
            return Ok(result);
        }
        [HttpGet]
        [Route("getnextgoals/{amount}/{score}")]
        public async Task<IActionResult> GetNextGoals(int amount, int score)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
        
            List<NextGoalResponse> response = await _leaderboardDa.GetNextGoals(amount, score);
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
            List<NextGoalResponse> response = await _leaderboardDa.GetNextGoals(amount, user.Highscore);
            return Ok(response);
        }

        [HttpGet]
        [Route("random/{username}")]
        public async Task<IActionResult> UpdateRandom(string username)
        {
            await _leaderboardDa.UpdateRandomAsync(username);
            return Ok();
        }
    }
}