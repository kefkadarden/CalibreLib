using System.Security.Claims;
using CalibreLib.Areas.Identity.Data;
using CalibreLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class ShelfController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IUserLibraryService _libraryService;

        public ShelfController(
            IBookService bookService,
            IUserLibraryService userLibraryService
        )
        {
            _bookService = bookService;
            _libraryService = userLibraryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shelf = await _libraryService.GetUserShelfByIdAsync(userId, id);
            
            if (shelf != null)
            {
                foreach (var item in shelf.BookShelves)
                {
                    var book = await _bookService.GetByIdAsync(item.BookId);
                    if (book != null)
                    {
                        item.Book = book;
                    }
                }
                
                var books = shelf.BookShelves.Select(x => x.Book);
                shelf.BookCards = await _bookService.GetBookCardModelsAsync(books);
                return View(shelf);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult RefreshShelfSelection(int bookId)
        {
            return ViewComponent("ShelfSelectionComponent", new { bookId = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> Add(int shelfId, int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();
            
            var shelf = await _libraryService.GetUserShelfByIdAsync(userId, shelfId);

            if (shelf != null)
            {
                shelf.BookShelves.Add(
                    new BooksShelvesLink()
                    {
                        BookId = bookId,
                        DateAdded = DateTime.Now,
                        ShelfId = shelfId,
                        Order = shelf.BookShelves.Max(x => (int?)x.Order) ?? 0 + 1,
                    }
                );
            }
            
            var added = await _libraryService.AddBookToShelfAsync(userId, shelfId, bookId);

            if (!added)
            {
                return BadRequest("Book not added to shelf");
            }
            
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int shelfId, int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var shelf = await _libraryService.GetUserShelfByIdAsync(userId, shelfId);

            if (shelf == null)
                return NotFound();

            var removed = await _libraryService.RemoveBookFromShelfAsync(userId, shelfId, bookId);

            if (!removed)
                return BadRequest("Book not removed from shelf");
            
            return Ok();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Shelf? shelf = null)
        {
            if (shelf == null)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            if (String.IsNullOrWhiteSpace(shelf.Name))
                return BadRequest("Shelf Name cannot be empty");

            var newShelf = await _libraryService.CreateShelfAsync(userId, shelf.Name);

            if (newShelf == null)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Code = 500,
                        Message = "Could not create Shelf",
                        ErrorMessage = "Could not create Shelf",
                    }
                );

        ViewBag.SuccessMessage = "Shelf created successfully.";
            ModelState.Clear();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            Shelf? shelf = await _libraryService.GetUserShelfByIdAsync(userId, id.Value);
            if (shelf == null)
                return NotFound();

            return PartialView("Edit", shelf);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Shelf? shelf = null)
        {
            if (shelf == null)
                return BadRequest();

            if (shelf?.Id != id)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            if (String.IsNullOrWhiteSpace(shelf.Name))
                return BadRequest("Shelf Name cannot be empty");
            
            var updated = await _libraryService.UpdateShelfNameAsync(userId, id, shelf.Name);

            if (!updated)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Code = 500,
                        Message = "Cannot Update Shelf",
                        ErrorMessage = "Cannot Update Shelf",
                    }
                );

            return Ok(
                new { message = "Shelf updated successfully.", updatedName = shelf.Name }
            );
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();
            
            if (!id.HasValue)
                return BadRequest("ID  cannot be empty");
            
            var deleted = await _libraryService.DeleteShelfAsync(userId, id.Value);
            
            if  (!deleted)
                return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Code = 500,
                    Message = "Cannot Delete Shelf",
                    ErrorMessage = "Cannot Delete Shelf",
                }
            );

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ShelfList()
        {
            return View();
        }
    }
}
