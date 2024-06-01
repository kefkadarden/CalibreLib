using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
