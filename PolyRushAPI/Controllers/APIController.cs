using System;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PolyRushAPI.Helper;

namespace PolyRushAPI.Controllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
        //return OK response in the base domain when asked. This can be used to see if the API is online
        [HttpGet]
        [Route("api/")]
        public IActionResult Get()
        {
            MySqlConnection? con = null;
            try
            {
                con = DatabaseConnector.MakeConnection();
                return Ok(true);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
                throw;
            }
            finally
            {
                con?.Close();
            }
        }
    }
}