using System.Globalization;
using System.IO.Compression;
using CalibreLib.Areas.Identity.Data;
using CalibreLib.Models;
using CalibreLib.Models.MailService;
using CalibreLib.Models.Metadata;
using CalibreLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class CardGridController(IBookService bookService,
                                    UserManager<ApplicationUser> userManager,
                                    IMailService mailService,
                                    BookFileManager bookFileManager) : Controller
    {
        public IActionResult ViewEPub(string bookPath)
        {
            bookPath = "/books/" + bookPath + "";
            return PartialView("~/Views/Shared/Components/EPubViewer.cshtml", bookPath);
        }

        public async Task<IActionResult> SendToReader(int bookId, string format)
        {
            var book = await bookService.GetByIdAsync(bookId);

            if (book == null)
                return NotFound();

            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var emailAddress = user.EReaderEmail;

            if (emailAddress == null)
                return BadRequest("EReaderEmail not set for user");

            var response = await bookFileManager.DownloadBookAsync(book, format.ToLowerInvariant());
            if (response == null)
                return BadRequest("Response from downloading book empty");

            Stream attachment = new MemoryStream(response);

            await mailService.SendMailAsync(
                new MailData()
                {
                    EmailAddress = emailAddress,
                    Body = "",
                    Subject = book.Title,
                    Attachment = attachment,
                    AttachmentName = book.Title + $".{format.ToLowerInvariant()}",
                    IsBodyHtml = false,
                }
            );

            return Ok($"Success! Book queued for sending to {emailAddress}");
        }

        public async Task<IActionResult> DownloadBook(int bookId, string format)
        {
            var book = await bookService.GetByIdAsync(bookId);

            if (book == null)
                return NotFound();
            //var datum = book.Data.FirstOrDefault(x => x.Format == Format);

            var result = await bookFileManager.DownloadBookAsync(book, format);

            if (result != null)
            {
                return File(result, "application/octet-stream", book.Title + "." + format);
            }
            return NotFound();
        }

        public async Task<IActionResult> DownloadShelf(int shelfId, string format)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            
            if (user == null)
                return Challenge();

            var shelf = user.Shelves.Find(x => x.Id.Equals(shelfId));

            if (shelf == null)
                return NotFound();
            
            var safeShelfName = string.Concat(shelf.Name.Split(Path.GetInvalidFileNameChars()));
            var zipName = $"{safeShelfName}_{format.ToLowerInvariant()}.zip";
            
            var bookIds = shelf.BookShelves.Select(x => x.BookId).ToList();
            var books = await bookService.GetByBookListAsync(bookIds);
            var booksFiltered = books
                .SelectMany(x => x.Data)
                .Where(x => x.Format?.Equals(format, StringComparison.OrdinalIgnoreCase) ?? false)
                .ToList();

            if (!booksFiltered.Any())
            {
                TempData["ErrorMessage"] = $"None of the books on this shelf are available in {format.ToUpperInvariant()} format.";
                return RedirectToAction("Index", "Shelf", new { id = shelfId });
            }

            using var ms = new MemoryStream();
            var filesAdded = 0;
            
            {
                await using var zip = new ZipArchive(ms, ZipArchiveMode.Create, true);
                foreach (var book in booksFiltered)
                {
                    var result = await bookFileManager.DownloadBookAsync(book.Book, format);
                    if (result is null)
                        continue;
                    
                    var safeTitle = string.Concat(book.Book.Title.Split(Path.GetInvalidFileNameChars()));
                    var entry = zip.CreateEntry($"{safeTitle}.{format.ToLowerInvariant()}");
                    await using var entryStream = await entry.OpenAsync();
                    await entryStream.WriteAsync(result);
                    filesAdded++;

                }
            }
            
            // If books were on the shelf, but none had files in the requested format (e.g., no EPUBs found)
            if (filesAdded == 0)
            {
                TempData["ErrorMessage"] = $"None of the books on this shelf are available in {format.ToUpperInvariant()} format.";
                return RedirectToAction("Index", "Shelf", new { id = shelfId });
            }
            
            return File(ms.ToArray(), "application/zip", zipName);
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
            SearchModel? searchModel2 = null;
            if (searchModel != null)
            {
                searchModel2 = JsonConvert.DeserializeObject<SearchModel>(searchModel);
            }

            if (!Enum.TryParse(type, true, out EFilterType eType))
                eType = EFilterType.BookCardGrid;

            var books = await GetBookList(pageNumber, sortBy, eType, id, searchModel2, query);
            var model = await bookService.GetBookCardModelsAsync(books);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return new List<Book>();

            bool ascending = sortBy != null && sortBy.EndsWith("desc");
            sortBy = sortBy?.Replace("desc", "").Replace("asc", "");

            Expression<Func<Book, object?>> orderBy = sortBy switch
            {
                "title" => book => book.Title,
                "date" => book => book.Pubdate,
                "author" => book => book.AuthorSort,
                _ => book => book.Title
            };

            var books = await bookService.GetBooksAsync(new BookQueryParameters()
            {
                PageNumber = pageNumber,
                OrderBy = orderBy,
                Query = query,
                Ascending = ascending,
                FilterType = type,
                FilterId = id,
                SearchModel = searchModel,
                UserId = userId
            });
            return books.Items;
        }

        public async Task<IActionResult> UpdateArchivedStatus(
            int bookId = -1,
            bool isArchived = false
        )
        {
            var book = await bookService.GetByIdAsync(bookId);

            ArgumentNullException.ThrowIfNull(book);

            var user = await userManager.GetUserAsync(HttpContext.User);

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

            await userManager.UpdateAsync(user);

            return Ok();
        }

        public async Task<IActionResult> UpdateReadStatus(int bookId = -1, bool readStatus = false)
        {
            var book = await bookService.GetByIdAsync(bookId);

            ArgumentNullException.ThrowIfNull(book);

            var user = await userManager.GetUserAsync(HttpContext.User);

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

            await userManager.UpdateAsync(user);

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
