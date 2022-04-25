using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PolyRushWeb.Helper;

namespace PolyRushWeb.Controllers.ApiControllers
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
                return Ok(con.State == ConnectionState.Open);
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