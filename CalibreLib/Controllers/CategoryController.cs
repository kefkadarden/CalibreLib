using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly BookRepository _bookRepository;
        public CategoryController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }


        [Route("category/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _books = await _bookRepository.GetByTagAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModels(_books);
            return View("CategoryBookGrid", _bc);
        }
    }
}
