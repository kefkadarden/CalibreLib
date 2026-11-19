using CalibreLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers;

public class EditController : Controller
{
    private readonly IBookService _bookService;
    public EditController(IBookService bookService)
    {
        _bookService = bookService;
    }


    [Route("edit/{id?}")]
    public async Task<IActionResult> Index(int? id)
    {
        if (id == null)
            return View();

        var book = await _bookService.GetByIdAsync(id.Value);
        
        if (book == null)
            return NotFound();

        var bookCard = await _bookService.GetBookCardModelAsync(book);

        return View(bookCard);
    }
}