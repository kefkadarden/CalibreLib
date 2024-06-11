using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class SeriesController : Controller
    {
        private readonly BookRepository _bookRepository;
        public SeriesController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [Route("series/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _books = await _bookRepository.GetBySeriesAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModels(_books);
            return View("SeriesBookGrid", _bc);
        }
    }
}
