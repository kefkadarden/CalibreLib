using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly BookRepository _bookRepository;
        private readonly MetadataDBContext _metadataDBContext;
        public RatingController(BookRepository bookRepository, MetadataDBContext metadataDBContext)
        {
            _bookRepository = bookRepository;
            _metadataDBContext = metadataDBContext;
        }

        [Route("rating/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var rating = _metadataDBContext.Ratings.FirstOrDefault(x => x.Id == id);

            if (rating == null)
                return NotFound();

            return View(rating);
        }
    }
}
