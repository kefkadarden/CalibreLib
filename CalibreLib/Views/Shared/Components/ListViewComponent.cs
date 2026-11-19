using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using CalibreLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Views.Shared.Components
{
    public class ListViewComponent : ViewComponent
    {
        private readonly IMetadataService _metadataService;
        private readonly IBookService _bookService;
        
        public ListViewComponent(IMetadataService metadataService, IBookService bookService)
        {
            _metadataService = metadataService;
            _bookService = bookService;
        }
        public async Task<IViewComponentResult> InvokeAsync(EFilterType type, string filter = "All", string sortBy = "")
        {
            List<ListViewModel> list = new List<ListViewModel>();
            var authors = await _metadataService.GetAuthorsAsync();
            var publishers = await _metadataService.GetPublishersAsync();
            var ratings = await _metadataService.GetRatingsAsync();
            var tags = await _metadataService.GetTagsAsync();
            var mSeries = await _metadataService.GetSeriesAsync();
            var languages = await _metadataService.GetLanguagesAsync();
            
            switch (type)
            {
                case EFilterType.AuthorsReversed:
                    foreach (var author in authors.AsEnumerable().Where(x => filter == "All"
                        || x.Sort.StartsWith(filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        var m = new ListViewModel();
                        m.Title = author.Sort;
                        m.Id = author.Id;
                        m.Count = author.BookAuthors.Count;
                        m.Url = $"/author/{author.Id}";
                        m.FilterType = type;
                        list.Add(m);
                    }
                    break;
                case EFilterType.Authors:
                    foreach (var author in authors.AsEnumerable().Where(x => filter == "All"
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
                    var books = await _bookService.GetBooksAsync(new BookQueryParameters()
                    {
                        PageSize = Int32.MaxValue
                    });
                    list.Add(new ListViewModel() { Title = "0", Id = -1, Count = books.Items.Where(x => x.BookRatings.Count() == 0).Count(), Url = $"/rating/-1", FilterType = type });
                    foreach (var rating in ratings.OrderBy(x => x.RatingValue))
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
                    foreach (var tag in tags.AsEnumerable().Where(x => filter == "All"
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
                    foreach (var series in mSeries.AsEnumerable().Where(x => filter == "All"
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
                    foreach (var lang in languages.AsEnumerable().Where(x => filter == "All"
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
                    foreach (var publisher in publishers.AsEnumerable().Where(x => filter == "All"
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
            if (sortBy == "title")
                list = list.OrderBy(x => x.Title).ToList();
            else if (sortBy == "titledesc")
                list = (List<ListViewModel>)list.OrderByDescending(x => x.Title).ToList();
            else
                list = list.OrderBy(x => x.Title).ToList();

            return View("~/Views/Shared/Components/ListViewPartial.cshtml", list);
        }

    }
}
