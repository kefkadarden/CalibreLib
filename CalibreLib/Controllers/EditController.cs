using CalibreLib.Data;
using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers;

public class EditController : Controller
{
    private readonly BookRepository _bookRepository;
    public EditController(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    [Route("edit/{id?}")]
    public async Task<IActionResult> Index(int? id)
    {
        if (id == null)
            return View();

        var book = await _bookRepository.GetByIDAsync(id.Value);
        
        if (book == null)
            return NotFound();

        var bookCard = await _bookRepository.GetBookCardModel(book);

        return View(bookCard);
    }
}