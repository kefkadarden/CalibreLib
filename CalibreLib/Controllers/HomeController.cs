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
using CalibreLib.Data;
using static System.Reflection.Metadata.BlobBuilder;

namespace CalibreLib.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser>   _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            return View();
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
