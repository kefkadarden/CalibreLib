using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using CalibreLib.Models;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;
using CalibreLib.Areas.Identity.Data;
using Microsoft.Graph;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using System.Globalization;
using System.Security.Policy;
using System;

namespace CalibreLib.Controllers
{
    [Authorize]
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

        [HttpGet]
        public async Task<IActionResult> GetPageCount(string? query, string? sortBy = "date", int? pageSize = 30, string? shelf = null
                                                    , string? category = null
                                                    , string? author = null
                                                    , string? publisher = null
                                                    , string? language = null
                                                    , string? rating = null
                                                    , string? series = null)
        {
            var books = await GetBookList(0, query, sortBy, null, shelf, category, author, publisher, language, rating, series, true);
            var count = bookRepository.GetPageCount(books);
            return Json(new { pageCount = count });
        }

        public IActionResult ViewEPub(string BookPath)
        {
            BookPath = "/books/" + BookPath + "";
            return PartialView("~/Views/Shared/Components/EPubViewer.cshtml", BookPath);
        }

        public async Task<IActionResult> SendEPubToReader(int BookID)
        {
            var book = await bookRepository.GetByIDAsync(BookID);

            /*send book to eReader
                1. Get User's eReader email address.
                2. Generate MailKit email with book ePub attachment.
            */
            return Ok();
        }

        public async Task<IActionResult> DownloadBook(int BookID, string Format)
        {
            var book = await bookRepository.GetByIDAsync(BookID);

            var datum = book.Data.FirstOrDefault(x => x.Format == Format);

            BookFileManager fm = new BookFileManager(_env, HttpContext.Request);
            var result = await fm.DownloadBookAsync(book, Format);

            if (result != null)
            {
                return File(result, "application/octet-stream", book.Title + "." + Format);
            }
            return Ok();
        }

        public async Task<IActionResult> BookList(int? pageNumber, string? query, string? sortBy = "date", int? pageSize = 30, string? shelf = null
                                                    , string? category = null
                                                    , string? author = null
                                                    , string? publisher = null
                                                    , string? language = null
                                                    , string? rating = null
                                                    , string? series = null)
        {
            var books = await GetBookList(pageNumber, query, sortBy, pageSize, shelf, category, author, publisher, language, rating, series);
            var model = await bookRepository.GetBookCardModels(books);
            return PartialView("~/Views/Shared/Components/BookCardGridRecords.cshtml", model);
        }

        private async Task<List<Book>> GetBookList(int? pageNumber, string? query, string? sortBy = "date", int? pageSize = 30, string? shelf = null
                                                    , string? category = null
                                                    , string? author = null
                                                    , string? publisher = null
                                                    , string? language = null
                                                    , string? rating = null
                                                    , string? series = null
                                                    , bool restorePageSize = false)
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

            int id;
            EFilterType type = EFilterType.BookCardGrid;
            if (shelf != "null" && shelf != null)
            {
                Int32.TryParse(shelf, out id);
                type = EFilterType.Shelf;
            }
            else if (category != "null" && category != null)
            {
                Int32.TryParse(category, out id);
                type = EFilterType.Categories;
            }
            else if (author != "null" && author != null)
            {
                Int32.TryParse(author, out id);
                type = EFilterType.Authors;
            }
            else if (publisher != "null" && publisher != null)
            {
                Int32.TryParse(publisher, out id);
                type = EFilterType.Publishers;
            }
            else if (language != "null" && language != null)
            {
                Int32.TryParse(language, out id);
                type = EFilterType.Languages;
            }
            else if (rating != "null" && rating != null)
            {
                Int32.TryParse(rating, out id);
                type = EFilterType.Ratings;
            }
            else if (series != "null" && series != null)
            {
                Int32.TryParse(series, out id);
                type = EFilterType.Series;
            }
            else
            {
                id = -1;
                type = EFilterType.BookCardGrid;
            }

            var savedPageSize = bookRepository.PageSize;

            if (pageSize != null)
            {
                bookRepository.PageSize = (int)pageSize;
            }
            else
            {
                var booksAll = await bookRepository.GetAllAsync();
                bookRepository.PageSize = booksAll.Count();
            }
            

            var books =  await bookRepository.GetBooks(pageNumber, orderBy, query, ascending, type, id);

            if (restorePageSize)
                bookRepository.PageSize = savedPageSize;

            return books.ToList();
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
