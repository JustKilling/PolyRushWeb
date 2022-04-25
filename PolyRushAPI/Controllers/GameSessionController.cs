using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushAPI.DA;
using PolyRushLibrary;

namespace PolyRushAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GameSessionController : ControllerBase
    {
        //return OK response in the base domain when asked. This can be used to see if the API is online
        [HttpPost]
        public void PutGameSession([FromBody] GameSession session)
        {            
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            session.UserID = id;
            GameSessionDA.UploadGameSession(id, session);
        }
        
    }
}