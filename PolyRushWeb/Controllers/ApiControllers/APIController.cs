using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.Data;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [ApiController]
    [Route("api")]
    public class APIController : ControllerBase
    {
        private readonly PolyRushWebContext _context;

        public APIController(PolyRushWebContext context)
        {
            _context = context;
        }
        //return OK response in the base domain when asked. This can be used to see if the API is online
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_context.Database.CanConnect());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                throw;
            }
        }
    }
}