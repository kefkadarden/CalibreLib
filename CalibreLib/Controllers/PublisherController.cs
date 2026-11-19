using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class PublisherController : Controller
    {
        private readonly IMetadataService _metadataService;
        public PublisherController(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [Route("publisher/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var publisher = await _metadataService.GetPublisherByIdAsync(id.Value);

            if (publisher == null)
                return NotFound();

            return View(publisher);
        }
    }
}
