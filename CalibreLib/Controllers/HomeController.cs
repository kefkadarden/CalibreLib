using CalibreLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Security.Claims;
using CalibreLib.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using CalibreLib.Models.Metadata;
using Microsoft.EntityFrameworkCore;

namespace CalibreLib.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        //private readonly GraphServiceClient _graphServiceClient;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser>   _userManager;
        private readonly MetadataDBContext _metadataDBContext;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, MetadataDBContext metadataDBContext)//, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            //_graphServiceClient = graphServiceClient;
            _userManager = userManager;
            _metadataDBContext = metadataDBContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var books = await _metadataDBContext.Books.Where(x => x.BookAuthors.Count() > 0).ToListAsync();
            List<BookCardModel> model = new List<BookCardModel>();
            foreach(var book in books)
            {
                var bcModel = new BookCardModel()
                {
                    id = book.Id,
                    title = book.Title,
                    Author = String.Join(", ", book.BookAuthors.Select(x => x.Author.Name)),
                    //CoverPath = "cover.jpg", //TODO: Need to have backend file explorer that builds whether a book has a cover based on there being a cover.jpg in the book directory.
                    rating = book.BookRatings.FirstOrDefault()?.Rating.RatingValue ?? 0,
                };
                model.Add(bcModel);
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
