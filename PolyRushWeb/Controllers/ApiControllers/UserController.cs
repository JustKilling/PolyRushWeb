using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.DA;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDA _userDa;
        private readonly IWebHostEnvironment _env;

        //constructor that injects the dependencies
        public UserController(UserDA userDa, IWebHostEnvironment env)
        {
            _userDa = userDa;
            _env = env;
        }
        [HttpGet("ishighscore/{score}")]
        public async Task<IActionResult> IsHighscore(int score)
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok((await _userDa.GetByIdAsync(id))?.Highscore < score);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _userDa.GetUsers());
        }

        [HttpPost("deleteaccount")]
        public async Task<IActionResult> DeleteAccount()
        {
         
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            await _userDa.DisableAccountAsync(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
         {
            //id ophalen uit jwt
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            UserDTO? user = await _userDa.GetByIdAsync(id);
            //check if user with that id exists
            if (user == null) return BadRequest("User not found!");

            return Ok(user);
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateUser(UserDTO user)
        {
            if (await _userDa.UpdateUser(user))
            {
                return Ok();
            }
            return BadRequest("Error updating user!");
        }

        [HttpPost]
        [Route("updateimage")]
        public async Task<IActionResult> UpdateImage(ImageModel model)
        {
            //convert to bytes
            byte[] imageBytes = Convert.FromBase64String(model.ImageString);

            //id ophalen uit jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            //create a bitmap from the bytes
            Bitmap bm = new(new MemoryStream(imageBytes));
            //Get the path
            string path = Path.Combine(_env.WebRootPath, "img", "user", id + ".png");
            //save image to path
            bm.SavePNG(path);
            //upload image to image server
            await ImageHelper.UploadAvatar(model.ImageString, id);

            return Ok();
        } 
        [HttpGet]
        [Route("isemailinuse")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse([FromQuery] string email)
        {
            return Ok(await _userDa.IsEmailInUse(email.Trim()));
        } 
        [HttpGet]
        [Route("isusernameinuse")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUsernameInUse([FromQuery] string username)
        {
            return Ok(await _userDa.IsUsernameInUse(username.Trim()));
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
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _userDa.GetCoinsAsync(id));
        }

        [HttpPost]
        [Route("updateadmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(UserEditAdminModel user)
        {
            if (await _userDa.UpdateUser(user))
            {
                return Ok();
            }
            return BadRequest("Error updating user!");
        }
        [HttpPost]
        [Route("deactivate")]
        public async Task<IActionResult> Deactivate()
        {
            //get user id from jwt
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
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _userDa.RemoveCoins(id, amount));
        }
        [HttpPost]
        [Route("removecoins")]
        public async Task<IActionResult> RemoveCoins()
        {
            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _userDa.RemoveCoins(id));
        }
    }
}