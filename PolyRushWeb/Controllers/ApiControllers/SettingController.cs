using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.DA;

namespace PolyRushWeb.Controllers.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly SettingDA _settingDa;

        public SettingController(SettingDA settingDa)
        {
            _settingDa = settingDa;
        }

        [HttpPost("music/{strEnable}")]
        public async Task<IActionResult> SetMusicAsync(string strEnable)
        {
            int intEnable;
            try
            {
                intEnable = Convert.ToInt16(strEnable);
            }
            catch
            {
                try
                {
                    intEnable = Convert.ToInt16(Convert.ToBoolean(strEnable));
                }
                catch
                {
                    return BadRequest("Invalid format: Please provide a boolean (0=false or 1=true)");
                }
            }

            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            await _settingDa.SetSettingAsync(id, EnumSetting.Music, intEnable);
            return Ok();
        }

        [HttpPost("sfx/{strEnable}")]
        public async Task<IActionResult> SetSfxAsync(string strEnable)
        {
            int intEnable;
            try
            {
                intEnable = Convert.ToInt16(strEnable);
            }
            catch
            {
                try
                {
                    intEnable = Convert.ToInt16(Convert.ToBoolean(strEnable));
                }
                catch
                {
                    return BadRequest("Invalid format: Please provide a boolean (0=false or 1=true)");
                }
            }

            //get user id from jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            await _settingDa.SetSettingAsync(id, EnumSetting.Sfx, intEnable);
            return Ok();
        }

        [HttpPost("mastervolume/{volume}")]
        public async Task<IActionResult> SetMasterVolumeAsync(int volume)
        {
            //check if the volume is in a correct format
            if (volume is < 0 or > 100)
                return BadRequest("Invalid format: Please provide an integer between 0 and 100");
            //gather the id from the jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            await _settingDa.SetSettingAsync(id, EnumSetting.MasterVolume, volume);
            return Ok();
        }

        [HttpGet("sfx")]
        public async Task<IActionResult> GetSfx()
        {
            //id ophalen uit jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);

            return Ok(await _settingDa.GetUserSetting(id, EnumSetting.Sfx));
        }

        [HttpGet("music")]
        public async Task<IActionResult> GetMusic()
        {
            //id ophalen uit jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _settingDa.GetUserSetting(id, EnumSetting.Music));
        }

        [HttpGet("mastervolume")]
        public async Task<IActionResult> GetMasterVolume()
        {
            //id ophalen uit jwt
            int id = int.Parse(User.Claims.First(i => i.Type == "id").Value);
            return Ok(await _settingDa.GetUserSetting(id, EnumSetting.MasterVolume));
        }
    }
}