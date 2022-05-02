using System.Data;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.Models;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly polyrushContext _context;

        public APIController(polyrushContext context)
        {
            _context = context;
        }
        //return OK response in the base domain when asked. This can be used to see if the API is online
        [HttpGet]
        [Route("api/")]
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