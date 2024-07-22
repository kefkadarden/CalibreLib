using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Views.Shared.Components
{
    public class RatingComponent : ViewComponent
    {
        private MetadataDBContext _metadataDBContext;

        public RatingComponent(MetadataDBContext metadataDBContext)
        {
            _metadataDBContext = metadataDBContext;
        }
        public IViewComponentResult Invoke(int RatingValue)
        {
            BookCardModel bookCardModel = new BookCardModel()
            {
                Rating = RatingValue
            };
            return View("~/Views/Shared/Components/RatingPartial.cshtml", bookCardModel);
        }
    }
}
