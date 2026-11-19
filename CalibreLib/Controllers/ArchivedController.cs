using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Mvc;
using CalibreLib.Services;
using System.Linq.Expressions;
using System.Security.Claims;
using CalibreLib.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace CalibreLib.Controllers
{
    public class ArchivedController : Controller
    {
        private readonly IBookService _bookService;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public ArchivedController(IBookService bookService,  UserManager<ApplicationUser> userManager)
        {
            _bookService = bookService;
            _userManager = userManager;
            
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            
            Expression<Func<Book, object?>> orderBy = book => book.Title;
            
            var books = await _bookService.GetBooksAsync(new BookQueryParameters()
            {
                PageNumber = 0,
                OrderBy = orderBy,
                Ascending = true,
                FilterType = Models.EFilterType.Archived,
                UserId = userId
            });
            var bookCards = await _bookService.GetBookCardModelsAsync(books.Items);
            return View(bookCards);

        }
    }
}
