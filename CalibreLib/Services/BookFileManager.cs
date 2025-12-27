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
    public class BookFileManager
    {
        private readonly IWebHostEnvironment _env;
        private readonly HttpRequest _httpRequest;
        private string? _pathBase = string.Empty;

        public BookFileManager(IWebHostEnvironment env, HttpRequest httpRequest)
        {
            _env = env;
            _httpRequest = httpRequest;
        }
        public async Task<string> GetBookCoverAsync(Book book)
        {
            using (var client = new HttpClient())
            {
                var file = await File.ReadAllBytesAsync(_env.WebRootPath + "/images/nocover.gif");
                string type = "image/gif";

                if (book.Path != null)
                {
                    try
                    {
                        file = await client.GetByteArrayAsync(_httpRequest.Scheme + "://" + _httpRequest.Host + "/books/" + book.Path.Replace("\\", "/") + "/cover.jpg");
                        type = "image/jpeg";
                    }
                    catch
                    {

                    }
                }
                var base64 = Convert.ToBase64String(file);
                var imgSrc = $"data:{type};base64,{base64}";
                return imgSrc;
            }
        }

        public async Task<byte[]?> DownloadBookAsync(Book book, string Format)
        {
            // URL of the file to be downloaded
            var fileUrl = new System.Uri(_httpRequest.Scheme + "://" + _httpRequest.Host + "/books/" + book.Path.Replace("\\", "/") + "/" + book.Data.FirstOrDefault(e => e.Format?.ToUpper() == Format.ToUpper()).Name + "." + Format.ToLower());

            // var localFilePath = Path.Combine(_env.WebRootPath, "/book/", book.Path.Replace("\\", "/") + "/" + book.Data.FirstOrDefault(e => e.Format?.ToUpper() == Format.ToUpper()).Name + "." + Format.ToLower());
            // Console.WriteLine("LOCALFILEPATH: " + _env.WebRootPath);
            // if (!File.Exists(localFilePath))
            //   return null;
            // Console.WriteLine("made it past the spot");
            //
            using (var httpClient = new HttpClient())
            {
                // Send a GET request to the specified Uri
                using (var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    Console.WriteLine("Content Length: " + response.Content.Headers.ContentLength);
                    return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                }
            }
        }

    }
}
