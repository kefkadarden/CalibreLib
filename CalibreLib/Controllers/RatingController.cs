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

            if (id == -1)
            {
                return View(new Rating() { Id = -1, Rating1 = 0 });
            }

            if (rating == null)
                return NotFound();

            return View(rating);
        }
    }
}
