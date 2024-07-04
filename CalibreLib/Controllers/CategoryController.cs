﻿using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly BookRepository _bookRepository;
        private readonly MetadataDBContext _metadataDBContext;

        public CategoryController(BookRepository bookRepository, MetadataDBContext metadataDBContext)
        {
            _bookRepository = bookRepository;
            _metadataDBContext = metadataDBContext;
        }


        [Route("category/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return View();

            var _books = await _bookRepository.GetByTagAsync((int)id);
            var _bc = await _bookRepository.GetBookCardModels(_books);
            var tag = _metadataDBContext.Tags.FirstOrDefault(x => x.Id == id);
            return View(tag);
        }
    }
}
