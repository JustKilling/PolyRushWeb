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

        //constructor that injects the dependencies
        public LeaderboardController(UserDA userDa, LeaderboardDA leaderboardDa)
        {
            _userDa = userDa;
            _leaderboardDa = leaderboardDa;
        }
        //get a specified amount of the top users
        [HttpGet]
        [Route("{amount}")]
        public async Task<IActionResult> GetTopUsers(int amount)
        {
            List<UserDTO>? result = await _leaderboardDa.GetTopUsers(amount);
            return Ok(result);
        }
        //get an amount of next higher goals, give the score that needs to be the lower one
        [HttpGet]
        [Route("getnextgoals/{amount}/{score}")]
        public async Task<IActionResult> GetNextGoals(int amount, int score)
        {
          
            List<NextGoalResponse> response = await _leaderboardDa.GetNextGoals(amount, score);
            return Ok(response);
        }
        //get an amount of next higher goals, it uses the player highscore
        [HttpGet]
        [Authorize]
        [Route("getnextgoals/{amount}")]
        public async Task<IActionResult> GetNextGoals(int amount)
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            UserDTO? user = await _userDa.GetByIdAsync(id);
            if (user == null) return BadRequest();
            List<NextGoalResponse> response = await _leaderboardDa.GetNextGoals(amount, user.Highscore);
            return Ok(response);
        }

        //method to randomise a user, testing purposes only.
        [HttpGet]
        [Route("random/{username}")]
        public async Task<IActionResult> UpdateRandom(string username)
        {
            await _leaderboardDa.UpdateRandomAsync(username);
            return Ok();
        }
        //method to return top playtimes
        [HttpGet]
        [Route("playtime/{amount}")]
        public async Task<IActionResult> GetTopPlayTimes(int amount = 10)
        {
            return Ok(await _leaderboardDa.GetTopPlaytimeAsync(amount));
        }
        [HttpGet]
        [Route("stats")]
        [Authorize]
        public async Task<IActionResult> GetUserStats()
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            //return user stats
            return Ok(await _leaderboardDa.GetUserStats(id));
        }
    }
}