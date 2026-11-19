using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Views.Shared.Components
{
    public class RatingComponent : ViewComponent
    {

        public RatingComponent()
        {
        }
        
        public IViewComponentResult Invoke(int RatingValue, int id = -1, string prefix = "", bool isDisabled = true)
        {
            BookCardModel bookCardModel = new BookCardModel()
            {
                RatingValue = RatingValue,
                id = id,
                title = prefix
            };
            ViewData["isDisabled"] = isDisabled;
            return View("~/Views/Shared/Components/RatingPartial.cshtml", bookCardModel);
        }
    }
}
