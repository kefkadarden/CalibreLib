using CalibreLib.Areas.Identity.Data;
using CalibreLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Views.Shared.Components
{
    public class ShelfSelectionComponent : ViewComponent
    {
        private UserManager<ApplicationUser> _userManager;

        public ShelfSelectionComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IViewComponentResult Invoke(int BookId)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var model = new ShelfSelectionModel()
            {
                UserShelves = user?.Shelves ?? new List<Shelf>(),
                BookId = BookId
            };
            return View("~/Views/Shelf/ShelfSelection.cshtml", model);
        }
    }
}
