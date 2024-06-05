using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using CalibreLib.Models;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;

namespace CalibreLib.Controllers
{
    public class CardGridController : Controller
    {

        private BookRepository bookRepository;
        private IWebHostEnvironment _env;


        public CardGridController(BookRepository bookRepository, IWebHostEnvironment env)
        {
            this.bookRepository = bookRepository;
            _env = env;
        }

        public async Task<List<BookCardModel>> GetBookCardModels(IEnumerable<Book> books)
        {
            List<BookCardModel> model = new List<BookCardModel>();
            BookFileManager manager = new BookFileManager(_env,Request.Scheme + "://" + Request.Host);
            foreach (var book in books)
            {
                
                var cover = await manager.GetBookCoverAsync(book);
                var bcModel = new BookCardModel()
                {
                    id = book.Id,
                    title = book.Title,
                    Author = String.Join(", ", book.BookAuthors.Select(x => x.Author.Name)),
                    CoverImage = cover,
                    Rating = book.BookRatings.FirstOrDefault()?.Rating.RatingValue ?? 0,
                };
                model.Add(bcModel);
            }

            return model;
        }

        [HttpGet]
        public async Task<IActionResult> GetPageCount()
        {
            var count = await bookRepository.GetPageCountAsync();
            return Json(new { pageCount = count });
        }

        public async Task<IActionResult> BookList(int? pageNumber, string? sortBy = "date", int? pageSize = 30)
        {
            bool ascending = true;
            if (sortBy.EndsWith("desc"))
                ascending = false;

            Func<Book, object> orderBy = book =>
            {
                switch (sortBy)
                {
                    case "title":
                        return book.Title;
                    case "date":
                        return book.Pubdate;
                    case "author":
                        return book.AuthorSort;
                    default:
                        return book.Title;
                }
            };

            if (pageSize != null)
                bookRepository.PageSize = (int)pageSize;

            var books = bookRepository.GetBooks(pageNumber, orderBy, ascending);
            var model = await GetBookCardModels(books);
            return PartialView("~/Views/Shared/Components/BookCardGridRecords.cshtml", model);
        }
    }
}
