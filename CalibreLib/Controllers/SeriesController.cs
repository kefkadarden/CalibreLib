using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class SeriesController : Controller
    {
        private readonly BookRepository _bookRepository;
        private readonly MetadataDBContext _metadataDBContext;
        public SeriesController(BookRepository bookRepository, MetadataDBContext metadataDBContext)
        {
            _bookRepository = bookRepository;
            _metadataDBContext = metadataDBContext;
        }

        [Route("series/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var series = _metadataDBContext.Series.FirstOrDefault(x => x.Id == id);

            if (series == null)
                return NotFound();

            return View(series);
        }
    }
}
