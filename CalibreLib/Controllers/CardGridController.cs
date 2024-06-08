using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using CalibreLib.Models;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;
using CalibreLib.Areas.Identity.Data;
using Microsoft.Graph;
using Microsoft.AspNetCore.Identity;

namespace CalibreLib.Controllers
{
    public class CardGridController : Controller
    {

        private BookRepository bookRepository;
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _env;


        public CardGridController(BookRepository bookRepository, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            this.bookRepository = bookRepository;
            _env = env;
            _userManager = userManager;
        }

        public async Task<List<BookCardModel>> GetBookCardModels(IEnumerable<Book> books)
        {
            List<BookCardModel> model = new List<BookCardModel>();
            BookFileManager manager = new BookFileManager(_env,Request.Scheme + "://" + Request.Host);
            List<Identifier> identifiers = await this.bookRepository.GetIdentifiersAsync();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            foreach (var book in books)
            {
                
                var cover = await manager.GetBookCoverAsync(book);
                var bcModel = new BookCardModel()
                {
                    id = book.Id,
                    title = book.Title,
                    Book = book,
                    Authors = book.BookAuthors,
                    Series = book.BookSeries,
                    Languages = book.BookLanguages,
                    Tags = book.BookTags,
                    CoverImage = cover,
                    Rating = book.BookRatings.FirstOrDefault()?.Rating.RatingValue ?? 0,
                    Archived = (user != null) ? user.ArchivedBooks.Where(x => x.Id == book.Id).Any() : false,
                    Read = (user != null) ? user.ReadBooks.Where(x => x.Id == book.Id).Any() : false,
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

        public async Task<IActionResult> UpdateArchivedStatus(int bookid = -1, bool isArchived = false)
        {
            var book = await bookRepository.GetByIDAsync(bookid);

            ArgumentNullException.ThrowIfNull(book);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            ArgumentNullException.ThrowIfNull(user);

            ArchivedBook archiveBook = null!;

            if (user.ArchivedBooks.FirstOrDefault(x => x.BookId == book.Id && x.UserId == user.Id) == null)
            {
                archiveBook = new ArchivedBook()
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    IsArchived = isArchived,
                    LastModified = DateTime.Now
                };
                user.ArchivedBooks.Add(archiveBook);
            }
            else
            {
                archiveBook = user.ArchivedBooks.FirstOrDefault(x => x.BookId == book.Id && x.UserId == user.Id);

                archiveBook.IsArchived = isArchived;
                archiveBook.LastModified = DateTime.Now;
            }

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        public async Task<IActionResult> UpdateReadStatus(int bookid = -1, bool readStatus = false)
        {
            var book = await bookRepository.GetByIDAsync(bookid);

            ArgumentNullException.ThrowIfNull(book);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            ArgumentNullException.ThrowIfNull(user);

            ReadBook readBook = null!;
            if (user.ReadBooks.FirstOrDefault(user => user.BookId == book.Id) == null)
            {
                readBook = new ReadBook()
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    ReadStatus = (readStatus) ? 1 : 0,
                    LastModified = DateTime.Now
                };
                user.ReadBooks.Add(readBook);
            }
            else
            {
                readBook = user.ReadBooks.FirstOrDefault(x => x.BookId == book.Id && x.UserId == user.Id);

                readBook.ReadStatus = (readStatus) ? 1 : 0;
                readBook.LastModified = DateTime.Now;
            }
            
            await _userManager.UpdateAsync(user);

            return Ok();
        }
    }
}
