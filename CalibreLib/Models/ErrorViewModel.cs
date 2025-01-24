using Microsoft.AspNetCore.Mvc;

namespace CalibreLib.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ProblemDetails ProblemDetails { get; set; } = new ProblemDetails();
    }
}
