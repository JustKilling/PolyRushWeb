using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushAPI.DA;

namespace PolyRushApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        [HttpPost("music/{strEnable}")]
        public IActionResult SetMusic(string strEnable)
        {
            bool enable;
            try
            {
                int intEnable = Convert.ToInt16(strEnable);
                enable = Convert.ToBoolean(intEnable);
            }
            catch (Exception e)
            {
                return BadRequest("Invalid format: Please provide a boolean (0=false or 1=true)");
            }
            return Ok(enable);
        }
        
        [HttpPost("sfx/{strEnable}")]
        public IActionResult SetSfx(string strEnable)
        {
            bool enable;
            try
            {
                int intEnable = Convert.ToInt16(strEnable);
                enable = Convert.ToBoolean(intEnable);
            }
            catch (Exception e)
            {
                return BadRequest("Invalid format: Please provide a boolean (0=false or 1=true)");
            }
            
            return Ok(enable);
        }
        

    }
}