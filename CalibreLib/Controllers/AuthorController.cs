using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly BookRepository _bookRepository;
        private readonly MetadataDBContext _metadataDBContext;
        public AuthorController(BookRepository bookRepository, MetadataDBContext metadataDBContext) 
        {
            _bookRepository = bookRepository;
            _metadataDBContext = metadataDBContext;
        }

        [Route("author/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var author = _metadataDBContext.Authors.FirstOrDefault(x => x.Id == id);
            if (author == null)
                return NotFound();

            return View(author);

        }
    }
}
