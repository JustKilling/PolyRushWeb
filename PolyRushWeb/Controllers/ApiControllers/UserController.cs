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
            UserDTO? user = await _userDa.GetByIdAsync(id);
            //check if user with that id exists
            if (user == null) return BadRequest("User not found!");

            return Ok(user);
        }
       

        //for admin to get a user with id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAsync(int id)
        {
            UserDTO? user = await _userDa.GetByIdAsync(id);
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
        [Route("update")]
        public async Task<IActionResult> UpdateUser(UserEditAdminModel userDto)
        {
            if (await _userDa.UpdateUser(userDto))
            {
                return Ok();
            }
            return BadRequest("Error updating user!");
        }
        [HttpPost]
        [Route("deactivate")]
        public async Task<IActionResult> Deactivate()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            await _userDa.DeactivateAsync(id);
            return Ok();
        }
        //activate given id
        [HttpPost]
        [Route("activate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(int id)
        {
            await _userDa.DeactivateAsync(id, false);
            return Ok();
        }
        //deactivate given id
        [HttpPost]
        [Route("deactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _userDa.DeactivateAsync(id);
            return Ok();
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