using CalibreLib.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class ShelfController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        public ShelfController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var shelf = user.Shelves.FirstOrDefault(x => x.Id == id);
            return View(shelf);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        public async Task<IActionResult> ShelfList()
        {
            return View();
        }
    }
}
