using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CalibreLib.Controllers
{
    public class ArchivedController : Controller
    {
        private readonly BookRepository _bookRepository;

        public ArchivedController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            Func<Book, object> orderBy = book =>
            {
                return book.Title;
            };
            var books = await _bookRepository.GetBooks(0, orderBy ,null, true, Models.EFilterType.Archived);
            var bookCards = await _bookRepository.GetBookCardModels(books);
            return View(bookCards);

        }
    }
}
