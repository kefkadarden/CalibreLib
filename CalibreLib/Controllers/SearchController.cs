using CalibreLib.Data;
using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Advanced()
        {
            return View(new SearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> Advanced(SearchModel model)
        {
            return View("Index", model);
        }
    }
}
