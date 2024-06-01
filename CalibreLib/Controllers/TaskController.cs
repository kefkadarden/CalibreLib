using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
