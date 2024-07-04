using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class PublisherController : Controller
    {
        private readonly BookRepository _bookRepository;
        private readonly MetadataDBContext _metadataDBContext;
        public PublisherController(BookRepository bookRepository, MetadataDBContext metadataDBContext)
        {
            _bookRepository = bookRepository;
            _metadataDBContext = metadataDBContext;
        }

        [Route("publisher/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var publisher = _metadataDBContext.Publishers.FirstOrDefault(x => x.Id == id);

            if (publisher == null)
                return NotFound();

            return View(publisher);
        }
    }
}
