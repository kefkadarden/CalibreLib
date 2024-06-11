using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly BookRepository _bookRepository;
        public RatingController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [Route("rating/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _books = await _bookRepository.GetByRatingAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModels(_books);
            return View("RatingBookGrid", _bc);
        }
    }
}
