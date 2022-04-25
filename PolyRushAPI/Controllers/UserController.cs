using Microsoft.AspNetCore.Mvc;
using PolyRushLibrary;

namespace PolyRushApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public User GetUser(int id)
        {
            
        }
    }
}