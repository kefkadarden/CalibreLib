using CalibreLib.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalibreLib.Services
{
    public class GlobalExceptionHandler(IHostEnvironment env, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
    {
        private const string UnhandledExceptionMsg = "An unhandled exception has occurred while executing the request.";

        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, exception.Message);

            var statusCode = exception switch
            {
                KeyNotFoundException or FileNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                ArgumentException or InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            if (IsApiOrAjaxRequest(context))
            {
                var problemDetails = CreateProblemDetails(context, exception);
                //var json = ToJson(problemDetails);

                var errorViewModel = new ErrorViewModel
                {
                    RequestId = context.TraceIdentifier,
                    ProblemDetails = problemDetails
                };

                context.Response.StatusCode = problemDetails.Status ?? 500;

                const string contentType = "application/problem+json";
                context.Response.ContentType = contentType;
                await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

                return true;
            }

            return false;
        }

        private bool IsApiOrAjaxRequest(HttpContext context)
        {
            var headers = context.Request.Headers;
            return context.Request.Path.StartsWithSegments("/api") ||
              headers["Accept"].ToString().Contains("application/json") ||
              headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private ProblemDetails CreateProblemDetails(in HttpContext context, in Exception exception)
        {
            var statusCode = context.Response.StatusCode;
            var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
            if (string.IsNullOrEmpty(reasonPhrase))
            {
                reasonPhrase = UnhandledExceptionMsg;
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = reasonPhrase
            };

            if (!env.IsDevelopment())
            {
                return problemDetails;
            }

            problemDetails.Detail = exception.ToString();
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["data"] = exception.Data;

            return problemDetails;
        }

        // private string ToJson(in ProblemDetails problemDetails)
        // {
        //     try
        //     {
        //         return JsonSerializer.Serialize(problemDetails, SerializerOptions);
        //     }
        //     catch (Exception ex)
        //     {
        //         const string msg = "An exception has occurred while serializing error to JSON.";
        //         logger.LogError(ex, msg);
        //     }
        //
        //     return string.Empty;
        // }
    }
}
