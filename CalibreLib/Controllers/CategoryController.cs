using CalibreLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IMetadataService _metadataService;

        public CategoryController(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }


        [Route("category/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue)
                return View();

            var tag = await _metadataService.GetTagByIdAsync(id.Value);

            if (tag == null)
                return NotFound();

            return View(tag);
        }
    }
}
