using Microsoft.AspNetCore.Mvc;

namespace PolyRushWeb.Controllers
{
    public class FileController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}