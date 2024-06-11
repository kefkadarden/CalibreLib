using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    public class ShelfController : Controller
    {

        public IActionResult Index(int id)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        public async Task<IActionResult> ShelfList()
        {
            return View();
        }
    }
}
