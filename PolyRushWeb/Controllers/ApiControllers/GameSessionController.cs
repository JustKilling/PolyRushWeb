using System;
using System.Linq;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PolyRushAPI.DA;
using PolyRushLibrary;

namespace PolyRushApi.Controllers
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