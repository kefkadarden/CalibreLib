using System.Diagnostics;
using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using CalibreLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookRepository _bookRepository;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            BookRepository bookRepository
        )
        {
            _logger = logger;
            _userManager = userManager;
            _bookRepository = bookRepository;
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

            var _book = await _bookRepository.GetByIDAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModel(_book);
            return View("BookDetailIndex", _bc);
        }

        //[Route("author/{id?}")]
        //public async Task<IActionResult> Author(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var _books = await _bookRepository.GetByAuthorAsync((int)id);
        //    var _bc = await _bookRepository.GetBookCardModels(_books);
        //    return View("AuthorBookGrid", _bc);
        //}

        //[Route("publisher/{id?}")]
        //public async Task<IActionResult> Publisher(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var _books = await _bookRepository.GetByPublisherAsync((int)id);
        //    var _bc = await _bookRepository.GetBookCardModels(_books);
        //    return View("PublisherBookGrid", _bc);
        //}

        //[Route("series/{id?}")]
        //public async Task<IActionResult> Series(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var _books = await _bookRepository.GetBySeriesAsync((int)id);
        //    var _bc = await _bookRepository.GetBookCardModels(_books);
        //    return View("SeriesBookGrid", _bc);
        //}

        //[Route("language/{id?}")]
        //public async Task<IActionResult> Language(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var _books = await _bookRepository.GetByLanguageAsync((int)id);
        //    var _bc = await _bookRepository.GetBookCardModels(_books);
        //    return View("LanguageBookGrid", _bc);
        //}

        //[Route("category/{id?}")]
        //public async Task<IActionResult> Category(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var _books = await _bookRepository.GetByTagAsync((int)id);
        //    var _bc = await _bookRepository.GetBookCardModels(_books);
        //    return View("CategoryBookGrid", _bc);
        //}

        //[Route("rating/{id?}")]
        //public async Task<IActionResult> Rating(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var _books = await _bookRepository.GetByRatingAsync((int)id);
        //    var _bc = await _bookRepository.GetBookCardModels(_books);
        //    return View("RatingBookGrid", _bc);
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("cookie-policy")]
        public IActionResult CookiePolicy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
