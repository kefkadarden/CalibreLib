using System.IO.Compression;
using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using CalibreLib.Models;
using CalibreLib.Models.MailService;
using CalibreLib.Models.Metadata;
using CalibreLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class CardGridController : Controller
    {
        private BookRepository bookRepository;
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _env;
        private readonly IMailService _mailService;

        public CardGridController(
            BookRepository bookRepository,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            IMailService mailService
        )
        {
            this.bookRepository = bookRepository;
            _env = env;
            _userManager = userManager;
            _mailService = mailService;
        }

        public IActionResult ViewEPub(string BookPath)
        {
            BookPath = "/books/" + BookPath + "";
            return PartialView("~/Views/Shared/Components/EPubViewer.cshtml", BookPath);
        }

        public async Task<IActionResult> SendToReader(int BookID, string Format)
        {
            var book = await bookRepository.GetByIDAsync(BookID);

            var _user = await _userManager.GetUserAsync(User);

            if (_user == null)
                return Unauthorized();

            var emailAddress = _user.EReaderEmail;

            if (emailAddress == null)
                return BadRequest("EReaderEmail not set for user");

            BookFileManager fm = new BookFileManager(_env, HttpContext.Request);
            var response = await fm.DownloadBookAsync(book, Format.ToLower());
            if (response == null)
                return BadRequest("Response from downloading book empty");

            Stream attachment = new MemoryStream(response);

            await _mailService.SendMailAsync(
                new MailData()
                {
                    EmailAddress = emailAddress,
                    Body = "",
                    Subject = book.Title,
                    Attachment = attachment,
                    AttachmentName = book.Title + $".{Format.ToLower()}",
                    IsBodyHtml = false,
                }
            );

            return Ok($"Success! Book queued for sending to {emailAddress}");
        }

        public async Task<IActionResult> DownloadBook(int BookID, string Format)
        {
            var book = await bookRepository.GetByIDAsync(BookID);

            //var datum = book.Data.FirstOrDefault(x => x.Format == Format);

            BookFileManager fm = new BookFileManager(_env, HttpContext.Request);
            var result = await fm.DownloadBookAsync(book, Format);

            if (result != null)
            {
                return File(result, "application/octet-stream", book.Title + "." + Format);
            }
            return NotFound();
        }

        public async Task<IActionResult> DownloadShelf(int ShelfID, string format)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var zipName = $"Books_{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            if (user == null)
                return Unauthorized();

            var shelf = user.Shelves.Find(x => x.Id.Equals(ShelfID));

            if (shelf == null)
                return BadRequest();
            var bookIds = shelf.BookShelves.Select(x => x.BookId).ToList();
            var books = await bookRepository.GetByBookListAsync(bookIds);
            var booksFiltered = books
                .SelectMany(x => x.Data)
                .Where(x => x.Format?.ToLower() == format.ToLower())
                .ToList();

            if (booksFiltered.Count() == 0)
                return NotFound();

            using (MemoryStream ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    booksFiltered.ForEach(book =>
                    {
                        var entry = zip.CreateEntry(book.Book.Title + "." + format);
                        BookFileManager fm = new BookFileManager(_env, HttpContext.Request);
                        var result = fm.DownloadBookAsync(book.Book, format).Result;

                        if (result != null)
                        {
                            using (var fileStream = new MemoryStream(result))
                            {
                                using (var entryStream = entry.Open())
                                {
                                    fileStream.CopyTo(entryStream);
                                }
                            }
                        }
                    });
                }

                return File(ms.ToArray(), "application/zip", zipName);
            }
        }

        [HttpGet]
        public async Task<IActionResult> BookList(
            int? pageNumber,
            string? sortBy = "date",
            string? type = null,
            int? id = null,
            string? searchModel = null,
            string? query = null
        )
        {
            SearchModel searchModel2 = new SearchModel();
            if (searchModel != null)
            {
                searchModel2 = JsonConvert.DeserializeObject<SearchModel>(searchModel);
            }

            if (!Enum.TryParse<EFilterType>(type, true, out EFilterType eType))
                eType = EFilterType.BookCardGrid;

            var books = await GetBookList(pageNumber, sortBy, eType, id, searchModel2, query);
            var model = await bookRepository.GetBookCardModels(books);
            return PartialView("~/Views/Shared/Components/BookCardGridRecords.cshtml", model);
        }

        private async Task<List<Book>> GetBookList(
            int? pageNumber,
            string? sortBy = "date",
            EFilterType type = EFilterType.BookCardGrid,
            int? id = null,
            SearchModel? searchModel = null,
            string? query = null
        )
        {
            bool ascending = true;
            if (sortBy != null && sortBy.EndsWith("desc"))
                ascending = false;

            Func<Book, object?> orderBy = book =>
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

            var books = await bookRepository.GetBooks(
                pageNumber,
                orderBy,
                query,
                ascending,
                type,
                id,
                searchModel
            );
            return books.ToList();
        }

        public async Task<IActionResult> UpdateArchivedStatus(
            int bookid = -1,
            bool isArchived = false
        )
        {
            var book = await bookRepository.GetByIDAsync(bookid);

            ArgumentNullException.ThrowIfNull(book);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            ArgumentNullException.ThrowIfNull(user);

            if (
                user.ArchivedBooks.FirstOrDefault(x => x.BookId == book.Id && x.UserId == user.Id)
                == null
            )
            {
                var archiveBook = new ArchivedBook()
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    IsArchived = isArchived,
                    LastModified = DateTime.Now,
                };
                user.ArchivedBooks.Add(archiveBook);
            }
            else
            {
                var archiveBook = user.ArchivedBooks.FirstOrDefault(x =>
                    x.BookId == book.Id && x.UserId == user.Id
                );

                archiveBook?.IsArchived = isArchived;
                archiveBook?.LastModified = DateTime.Now;
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

            if (
                user.ReadBooks.FirstOrDefault(x => x.BookId == book.Id && x.UserId == user.Id)
                == null
            )
            {
                var readBook = new ReadBook()
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    ReadStatus = (readStatus) ? 1 : 0,
                    LastModified = DateTime.Now,
                };
                user.ReadBooks.Add(readBook);
            }
            else
            {
                var readBook = user.ReadBooks.FirstOrDefault(x =>
                    x.BookId == book.Id && x.UserId == user.Id
                );

                readBook?.ReadStatus = (readStatus) ? 1 : 0;
                readBook?.LastModified = DateTime.Now;
            }

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpGet]
        public IActionResult LoadListViewComponent(
            EFilterType type,
            string filter = "All",
            string sortBy = ""
        )
        {
            return ViewComponent(
                "List",
                new
                {
                    type,
                    filter,
                    sortBy,
                }
            );
        }
    }
}
