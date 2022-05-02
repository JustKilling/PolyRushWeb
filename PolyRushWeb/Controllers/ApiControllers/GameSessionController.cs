using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.DA;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GameSessionController : ControllerBase
    {
        private readonly GameSessionDA _gameSessionDa;

        public GameSessionController(GameSessionDA gameSessionDa)
        {
            _gameSessionDa = gameSessionDa;
        }
        //return OK response in the base domain when asked. This can be used to see if the API is online
        [HttpPost]
        public async Task PutGameSessionAsync([FromBody] Gamesession session)
        {            
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            session.UserId = id;
            await _gameSessionDa.UploadGameSession(session);
        }
        
    }
}