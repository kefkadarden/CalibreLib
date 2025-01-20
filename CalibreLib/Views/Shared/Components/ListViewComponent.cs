using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Views.Shared.Components
{
    public class ListViewComponent : ViewComponent
    {
        private MetadataDBContext _metadataDBContext;

        public ListViewComponent(MetadataDBContext metadataDBContext)
        {
            _metadataDBContext = metadataDBContext;
        }
        public IViewComponentResult Invoke(EFilterType type, string filter = "All")
        {
            List<ListViewModel> list = new List<ListViewModel>();
            switch (type)
            {
                case EFilterType.Authors:
                    foreach (var author in _metadataDBContext.Authors.AsEnumerable().Where(x => filter == "All" 
                        || x.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        var m = new ListViewModel();
                        m.Title = author.Name;
                        m.Id = author.Id;
                        m.Count = author.BookAuthors.Count;
                        m.Url = $"/author/{author.Id}";
                        m.FilterType = type;
                        list.Add(m);
                    }
                    break;
                case EFilterType.Ratings:
                    list.Add(new ListViewModel() { Title = "0", Id = -1, Count = _metadataDBContext.Books.Where(x => x.BookRatings.Count() == 0).Count(), Url = $"/rating/-1", FilterType = type });
                    foreach (var rating in _metadataDBContext.Ratings.ToList().OrderBy(x => x.RatingValue))
                    {
                        var m = new ListViewModel();
                        m.Title = rating.RatingValue?.ToString() ?? "0";
                        m.Id = rating.Id;
                        m.Count = rating.BookRatings.Count;
                        m.Url = $"/rating/{rating.Id}";
                        m.FilterType = type;
                        list.Add(m);
                    }
                    break;
                case EFilterType.Categories:
                    foreach (var tag in _metadataDBContext.Tags.AsEnumerable().Where(x => filter == "All" 
                        || x.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        var m = new ListViewModel();
                        m.Title = tag.Name;
                        m.Id = tag.Id;
                        m.Count = tag.BookTags.Count;
                        m.Url = $"/category/{tag.Id}";
                        m.FilterType = type;
                        list.Add(m);
                    }
                    break;
                case EFilterType.Series:
                    foreach (var series in _metadataDBContext.Series.AsEnumerable().Where(x => filter == "All"
                        || x.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        var m = new ListViewModel();
                        m.Title = series.Name;
                        m.Id = series.Id;
                        m.Count = series.BookSeries.Count;
                        m.Url = $"/series/{series.Id}";
                        m.FilterType = type;
                        list.Add(m);
                    }
                    break;
                case EFilterType.Languages:
                    foreach (var lang in _metadataDBContext.Languages.AsEnumerable().Where(x => filter == "All"
                        || x.LangCode.StartsWith(filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        var m = new ListViewModel();
                        m.Title = lang.LangCode;
                        m.Id = lang.Id;
                        m.Count = lang.BookLanguages.Count;
                        m.Url = $"/language/{lang.Id}";
                        m.FilterType = type;
                        list.Add(m);
                    }
                    break;
                case EFilterType.Publishers:
                    foreach (var publisher in _metadataDBContext.Publishers.AsEnumerable().Where(x => filter == "All" 
                        || x.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        var m = new ListViewModel();
                        m.Title = publisher.Name;
                        m.Id = publisher.Id;
                        m.Count = publisher.BookPublishers.Count;
                        m.Url = $"/publisher/{publisher.Id}";
                        m.FilterType = type;
                        list.Add(m);
                    }
                    break;
            }
            return View("~/Views/Shared/Components/ListViewPartial.cshtml", list);
        }

    }
}
