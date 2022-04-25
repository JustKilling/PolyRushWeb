using Microsoft.AspNetCore.Mvc;

namespace PolyRushWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}