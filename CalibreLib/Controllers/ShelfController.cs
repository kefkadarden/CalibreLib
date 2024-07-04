using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class ShelfController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookRepository _bookRepository;
        private readonly CalibreLibContext _calibreLibContext;

        public ShelfController(UserManager<ApplicationUser> userManager, BookRepository bookRepository, CalibreLibContext calibreLibContext)
        {
            _userManager = userManager;
            _bookRepository = bookRepository;
            _calibreLibContext = calibreLibContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var shelf = user.Shelves.FirstOrDefault(x => x.Id == id);
            if (shelf != null)
            {                
                foreach (var item in shelf.BookShelves)
                {
                    var book = await _bookRepository.GetByIDAsync(item.BookId);
                    if (book != null)
                    {
                        item.Book = book;
                    }
                }
                var books = shelf.BookShelves.Select(x => x.Book);
                shelf.BookCards = await _bookRepository.GetBookCardModels(books);
                return View(shelf);
            }
            else
            {
                return NotFound();
            }

            
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

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return Unauthorized();

            shelf.Created = DateTime.Now;
            shelf.LastModified = DateTime.Now;
            shelf.User = user;
            shelf.UserId = user.Id;

            user.Shelves.Add(shelf);

            await _userManager.UpdateAsync(user);
            ViewBag.SuccessMessage = "Shelf created successfully.";
            ModelState.Clear();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return Unauthorized();

            Shelf? shelf;
            if ((shelf = user.Shelves.FirstOrDefault(shelf => shelf.Id == id)) == null)
                return NotFound();

            return View(shelf);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Shelf? shelf = null)
        {
            if (shelf == null)
                return BadRequest();

            if (shelf?.Id != id)
                return BadRequest();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return Unauthorized();

            var foundShelf = user.Shelves.Find(shelf => shelf.Id == id);

            if (foundShelf == null)
                return NotFound();

            foundShelf.LastModified = DateTime.Now;
            foundShelf.Name = shelf?.Name;

            await _userManager.UpdateAsync(user);
            ViewBag.SuccessMessage = "Shelf edited successfully.";

            return View(foundShelf);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var shelf = user.Shelves.FirstOrDefault(x => x.Id == id);

            if (shelf  == null) return NotFound();

            if (!user.Shelves.Remove(shelf))
                return StatusCode(StatusCodes.Status500InternalServerError, new { Code = 500, Message = "Cannot Delete Shelf", ErrorMessage = "Cannot Delete Shelf" });

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ShelfList()
        {
            return View();
        }
    }
}
