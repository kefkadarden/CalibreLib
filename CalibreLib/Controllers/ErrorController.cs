using CalibreLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CalibreLib.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel()
            {
                RequestId = HttpContext.TraceIdentifier,
                ProblemDetails = new ProblemDetails()
                {
                    Status = HttpContext.Response.StatusCode,
                    Title = ReasonPhrases.GetReasonPhrase(HttpContext.Response.StatusCode)
                }
            });
        }
    }
}
