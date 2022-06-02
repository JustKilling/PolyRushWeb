using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.DA;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly AchievementDA _achievementDa;
        //constructor that injects the dependencies
        public AchievementController(AchievementDA achievementDa)
        {
            _achievementDa = achievementDa;
        }

        //method to get achievement based on its id
        [HttpPost]
        [Route("{achievementId}")]
        public async Task<IActionResult> AddAchievementWithId(int achievementId)
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok(await _achievementDa.AddAchievementAsync(id, achievementId));
        }

        //method to get all achievements that a user has.
        [HttpGet]
        public async Task<IActionResult> Achievements()
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            List<Achievement>? achievements = await _achievementDa.GetAchievements(id);
            return Ok(achievements);
        }

        //method to get the achievement by its id, if its not found, return a badrequest
        [HttpGet]
        [Route("{achievementId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get (int achievementId)
        {
            Achievement? achievement = await _achievementDa.GetAchievement(achievementId);
            if (achievement == null) return BadRequest("Achievement with that id not found!");
            return Ok(achievement);
        }

    }
}
