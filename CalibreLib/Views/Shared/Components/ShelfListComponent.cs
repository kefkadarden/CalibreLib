using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Views.Shared.Components
{
    public class ShelfListComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Views/Shelf/ShelfList.cshtml");
        }
    }
}
