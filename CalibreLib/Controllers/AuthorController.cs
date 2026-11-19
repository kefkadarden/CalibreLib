using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly IMetadataService _metadataService;
        public AuthorController(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [Route("author/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue)
                return View();

            var author = await _metadataService.GetAuthorByIdAsync(id.Value);
            
            if (author == null)
                return NotFound();

            return View(author);

        }
    }
}
