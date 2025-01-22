using CalibreLib.Data;
using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace CalibreLib.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookRepository _bookRepository;
        private readonly MetadataDBContext _metadataDBContext;
        private readonly ShelfRepository _shelfRepo;

        public SearchController(BookRepository bookRepository, MetadataDBContext metadataDBContext, ShelfRepository shelfRepo)
        {
            _bookRepository = bookRepository;
            _metadataDBContext = metadataDBContext;
            _shelfRepo = shelfRepo;
        }
        public async Task<IActionResult> Index(string? query)
        {
            var books = await _bookRepository.GetByQueryAsync(query);
            var bc = await _bookRepository.GetBookCardModels(books);
            return View(bc);
        }

        [HttpGet]
        public async Task<IActionResult> Advanced()
        {
            return View(new SearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> Advanced(SearchModel model)
        {
            //Convert Search Model to query string
            var books = await _bookRepository.GetByQueryAsync(model);
            var bc = await _bookRepository.GetBookCardModels(books);
            return View("Index", bc);
        }
    }
}
