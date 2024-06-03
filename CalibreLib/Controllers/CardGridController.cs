using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using CalibreLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    public class CardGridController : Controller
    {

        private BookRepository bookRepository;

        public CardGridController(BookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public List<BookCardModel> GetBookCardModels(IEnumerable<Book> books)
        {
            List<BookCardModel> model = new List<BookCardModel>();
            foreach (var book in books)
            {
                var bcModel = new BookCardModel()
                {
                    id = book.Id,
                    title = book.Title,
                    Author = String.Join(", ", book.BookAuthors.Select(x => x.Author.Name)),
                    //CoverPath = "cover.jpg", //TODO: Need to have backend file explorer that builds whether a book has a cover based on there being a cover.jpg in the book directory.
                    rating = book.BookRatings.FirstOrDefault()?.Rating.RatingValue ?? 0,
                };
                model.Add(bcModel);
            }

            return model;
        }

        public IActionResult BookList(int? pageNumber, string? sortBy = "date")
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

            var books = bookRepository.GetBooks(pageNumber, orderBy, ascending);
            var model = GetBookCardModels(books);
            return PartialView("~/Views/Shared/Components/BookCardGridRecords.cshtml", model);
        }
    }
}
