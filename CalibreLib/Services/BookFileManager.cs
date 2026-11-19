using CalibreLib.Models.Metadata;

namespace CalibreLib.Services
{
    public enum BookFormat
    {
        AZW3 = 0,
        CBR,
        CBZ,
        DOC,
        DOCX,
        EPUB,
        LIT,
        LRF,
        MOBI,
        PDB,
        PDF,
        TXT
    }

    public class BookFileManager(
        IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory)
    {
        private string GetBaseBookUrl()
        {
            var request = httpContextAccessor.HttpContext?.Request
                ?? throw new InvalidOperationException("HttpContext is not available to construct the book URL.");

            return $"{request.Scheme}://{request.Host}/books";
        }

        public async Task<string> GetBookCoverAsync(Book book)
        {
            byte[] file;
            string type;

            if (!string.IsNullOrWhiteSpace(book.Path))
            {
                try
                {
                    var client = httpClientFactory.CreateClient();
                    var coverUrl = $"{GetBaseBookUrl()}/{book.Path.Replace("\\", "/")}/cover.jpg";

                    file = await client.GetByteArrayAsync(coverUrl);
                    type = "image/jpeg";

                    return $"data:{type};base64,{Convert.ToBase64String(file)}";
                }
                catch
                {
                    // Fall through to default nocover.gif if remote cover is missing or unreachable
                }
            }

            var fallbackPath = Path.Combine(env.WebRootPath, "images", "nocover.gif");
            file = await File.ReadAllBytesAsync(fallbackPath);
            type = "image/gif";

            return $"data:{type};base64,{Convert.ToBase64String(file)}";
        }

        public async Task<byte[]?> DownloadBookAsync(Book book, string format)
        {
            if (string.IsNullOrWhiteSpace(book.Path))
                return null;

            var bookData = book.Data.FirstOrDefault(e =>
                string.Equals(e.Format, format, StringComparison.OrdinalIgnoreCase));

            if (bookData == null || string.IsNullOrWhiteSpace(bookData.Name))
                return null;

            var fileUrl = new Uri(
                $"{GetBaseBookUrl()}/{book.Path.Replace("\\", "/")}/{bookData.Name}.{format.ToLowerInvariant()}");

            var client = httpClientFactory.CreateClient();
            using var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsByteArrayAsync()
                : null;
        }
    }
}
