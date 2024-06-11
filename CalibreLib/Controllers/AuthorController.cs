using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly BookRepository _bookRepository;
        public AuthorController(BookRepository bookRepository) 
        {
            _bookRepository = bookRepository;
        }

        [Route("author/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _books = await _bookRepository.GetByAuthorAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModels(_books);
            return View("AuthorBookGrid", _bc);
        }
    }
}
