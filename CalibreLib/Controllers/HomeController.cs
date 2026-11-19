using CalibreLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CalibreLib.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using CalibreLib.Services;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookService _bookService;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IBookService bookService)
        {
            _logger = logger;
            _userManager = userManager;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            return View();
        }

        [Route("book/{id?}")]
        public async Task<IActionResult> Book(int? id)
        {
            if (id == null)
                return NotFound();

            var _book = await _bookService.GetByIdAsync(id.Value);
            var _bc = await _bookService.GetBookCardModelAsync(_book);
            return View("BookDetailIndex", _bc);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
