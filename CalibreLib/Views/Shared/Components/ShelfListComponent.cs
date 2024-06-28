using CalibreLib.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Views.Shared.Components
{
    public class ShelfListComponent : ViewComponent
    {
        private UserManager<ApplicationUser> _userManager;

        public ShelfListComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IViewComponentResult Invoke()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            return View("~/Views/Shelf/ShelfList.cshtml", user?.Shelves);
        }
    }
}
