using CalibreLib.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace CalibreLib.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookRepository _bookRepository;

        public SearchController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public async Task<IActionResult> Index(string? query)
        {
            var books = await _bookRepository.GetByQueryAsync(query);
            var bc = await _bookRepository.GetBookCardModels(books);
            return View(bc);
        }
    }
}
