using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class PublisherController : Controller
    {
        private readonly BookRepository _bookRepository;
        public PublisherController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [Route("publisher/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _books = await _bookRepository.GetByPublisherAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModels(_books);
            return View("PublisherBookGrid", _bc);
        }
    }
}
