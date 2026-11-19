using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly IMetadataService _metadataService;
        public RatingController(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [Route("rating/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();


            var rating = await _metadataService.GetRatingByIdAsync(id.Value);

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
