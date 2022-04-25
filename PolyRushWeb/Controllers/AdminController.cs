using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PolyRushLibrary;
using PolyRushWeb.DA;
using PolyRushWeb.Helper;

namespace PolyRushWeb.Controllers
{
    [Secure(true)]
    public class AdminController : Controller
    {
        private readonly UserDA _userDA;

        public AdminController(UserDA userDA)
        {
            _userDA = userDA;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult GetUsers()
        {
            //Get all users and convert to DTO
            var users = _userDA.GetUsers().Result.Select(x => (UserDTO)x).ToList();
            return Json(new {data = users});
        }

        public IActionResult Edit(int id)
        {
            return View();
        }
        public async Task<IActionResult> Deactivate(int id)
        {
            await _userDA.Deactivate(id);
            return View(nameof(Index));
        }
    }
}
