using CalibreLib.Models.Metadata;
using System.Net;
using System.Security.Policy;

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
        private IWebHostEnvironment _env;
        private string? _pathBase = string.Empty;

        public BookFileManager(IWebHostEnvironment env, string? pathBase) 
        {
            _env = env;
            _pathBase = pathBase;
        }
        public async Task<string> GetBookCoverAsync(Book book)
        {
            HttpClient client = new HttpClient();
            var file = await File.ReadAllBytesAsync(_env.WebRootPath + "/images/nocover.gif");
            string type = "image/gif";

            if (book.Path != null)
            {
                try
                {
                    file = await client.GetByteArrayAsync(_pathBase + "/books/" + book.Path.Replace("\\", "/") + "/cover.jpg");
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
}
