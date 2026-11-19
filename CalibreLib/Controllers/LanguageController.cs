using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class LanguageController : Controller
    {
        private readonly IMetadataService _metadataService;
        public LanguageController(IMetadataService metadataService)
        {
            _metadataService =  metadataService;
        }


        [Route("language/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var language = await _metadataService.GetLanguageByIdAsync(id.Value);

            if (language == null)
                return NotFound();

            return View(language);
        }
    }
}
