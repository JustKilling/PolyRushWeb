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

        public AchievementController(AchievementDA achievementDa)
        {
            _achievementDa = achievementDa;
        }

        [HttpPost]
        [Route("{achievementId}")]
        public async Task<IActionResult> AddAchievementWithId(int achievementId)
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok(await _achievementDa.AddAchievementAsync(id, achievementId));
        }

        [HttpGet]
        public async Task<IActionResult> Achievements()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            var achievements = await _achievementDa.GetAchievements(id);
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
