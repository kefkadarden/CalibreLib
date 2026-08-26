using CalibreLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace CalibreLib.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
      private readonly IHostEnvironment _env;

      public ErrorController(IHostEnvironment env)
      {
        _env = env;
      }

      [Route("Error/{statusCode?}")]
        public IActionResult Index(int? statusCode = null)
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var statusCodeFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            var code = statusCode ?? (exceptionFeature != null ? StatusCodes.Status500InternalServerError : HttpContext.Response.StatusCode);
            if (code == StatusCodes.Status200OK) code = StatusCodes.Status500InternalServerError;

            var originalPath = exceptionFeature?.Path ?? (statusCodeFeature != null ? $"{statusCodeFeature.OriginalPath}{statusCodeFeature.OriginalQueryString}" : HttpContext.Request.Path.Value);
            var detail = _env.IsDevelopment() && exceptionFeature?.Error != null
                    ? exceptionFeature.Error.Message
                    : (code == StatusCodes.Status404NotFound ? "The requested resource was not found." : "An error occurred while processing your request.");
            
                if (IsApiOrJsonRequest(originalPath))
                {
                    var problemDetails = new ProblemDetails
                    {
                      Status = code,
                      Title = ReasonPhrases.GetReasonPhrase(code),
                      Instance = originalPath,
                      Detail = detail
                    };
                    problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;

                    return new ObjectResult(problemDetails)
                    {
                      StatusCode = code,
                      ContentTypes = { "application/problem+json" }
                    };
                }
            var errorModel = new ErrorViewModel
            {
              RequestId = HttpContext.TraceIdentifier,
              ProblemDetails = new ProblemDetails
              {
                Status = code,
                Title = ReasonPhrases.GetReasonPhrase(code),
                Instance = originalPath,
                Detail = detail 
              }
            };

            return View("~/Views/Shared/Error.cshtml", errorModel);
        }

          private bool IsApiOrJsonRequest(string originalPath)
            {
                  var headers = HttpContext.Request.Headers;
                var accept = headers["Accept"].ToString();
                var requestedWith = headers["X-Requested-With"].ToString();

                return originalPath.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
                    || accept.Contains("application/json")
                    || requestedWith.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            }
    }
}
