using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class SeriesController : Controller
    {
        private readonly IMetadataService _metadataService;
        public SeriesController(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [Route("series/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var series = await _metadataService.GetSeriesByIdAsync(id.Value);

            if (series == null)
                return NotFound();

            return View(series);
        }
    }
}
