using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class LanguageController : Controller
    {
        private readonly BookRepository _bookRepository;
        public LanguageController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }


        [Route("language/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _books = await _bookRepository.GetByLanguageAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModels(_books);
            return View("LanguageBookGrid", _bc);
        }
    }
}
