using CalibreLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    public class SearchController : Controller
    {
        public async Task<IActionResult> Index(string? query)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Advanced()
        {
            return View(new SearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> Advanced(SearchModel model)
        {
            return View("Index", model);
        }
    }
}
