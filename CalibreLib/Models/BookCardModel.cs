using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using System.Drawing;

namespace CalibreLib.Models
{
    public class BookCardModel
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string CoverImage { get; set; }
        public List<BooksAuthorsLink> Authors { get; set; } = null!;

        public List<BooksSeriesLink> Series { get; set; } = null!;

        public List<BooksLanguagesLink> Languages { get; set; } = null!;

        public List<BooksTagsLink> Tags { get; set; } = null!;

        public Book Book { get; set; } = null!;

        public string BookHTML
        {
            get
            {
                return @"<a class=""link-underline link-underline-opacity-0 link-underline-opacity-100-hover"" href=""/book/" + id + @""">" + title + @"</a>";
            }
        }
        public string AuthorsHTML
        { 
            get
            {
                return String.Join(" & ",this.Authors.Select(x => @"<a class=""link-underline link-underline-opacity-0 link-underline-opacity-100-hover"" href=""/author/" + x.Author.Id + @""">" + x.Author.Name + @"</a>"));
            } 
        }

        public string SeriesHTML
        {
            get
            {
                return String.Join(" & ", this.Series.Select(x => @"<a class=""link-underline link-underline-opacity-0 link-underline-opacity-100-hover"" href=""/series/" + x.Series.Id + @""">" + x.Series.Name +"</a>" + @" (" + x.Book.SeriesIndex + ")"));
            }
        }
        
        public int? Rating { get; set; }

        public string PublishDateHTML 
        { 
            get 
            {
                
                return (this.Book.Pubdate != DateTime.Parse("1/1/0101") && !string.IsNullOrEmpty(this.Book.Pubdate?.ToString())) ? "Published: " + ((DateTime)this.Book.Pubdate).ToShortDateString() : "";
            } 
        }

        public string PublisherHTML 
        { 
            get 
            {

                return String.Join(" & ", this.Book.BookPublishers.Select(x => @"Publisher: " + @"<a class=""link-primary"" href=""/publisher/" + x.Publisher.Id + @""">" + x.Publisher.Name + "</a>"));
            } 
        }

        public string LanguageHTML
        {
            get
            {
                return (this.Languages != null && this.Languages.Count() > 0) ? @"<p class=""label"">Language: " + this.Languages.FirstOrDefault()?.Language?.LangCode + "</p>" : "";
            }
        }

        public string TagsHTML 
        {
            get
            {
                string tags = "";
                if (this.Tags != null && this.Tags.Count() > 0)
                {
                    tags = String.Join("", this.Tags.Select(x => @"<p class=""label label-blue mb-1 me-1""><a class=""link-light link-primary"" href=""/category/" + x.TagId.ToString() + @""">" + x.Tag.Name + "</a></p>"));
                }
                 
                return tags;
            }
        }

        public string IdentifiersHTML
        {
            get
            {
                return String.Join("", this.Book.Identifiers.Select(x => {
                    string? url = "/identifier/";
                    switch(x.Type?.ToLower())
                    {
                        case "google":
                            url = "https://books.google.com/books?id=" + x.Val;
                            break;
                        case "amazon":
                            url = "https://www.amazon.com/dp/" + x.Val;
                            break;
                        case "isbn":
                            url = "https://search.worldcat.org/search?q=bn:" + x.Val;
                            break;
                        case "uri":
                            url = x.Val;
                            break;
                        case "mobi-asin":
                            url = "/search?query=" + x.Val;
                            break;
                    }
                    return @"<p class=""label label-green mb-1 me-1""><a class=""link-light link-primary"" target=""_blank"" href=""" + url + @""">" + x.Type + "</a></p>";
                    }));
            }
        }

        public bool Read { get; set; } = false;
        public bool Archived { get; set; } = false;

        public string Description { get; set; }

        public string AddDownloadButtonHTML
        {
            get
            {
                var html = "<button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">\r\n    Download\r\n  </button>\r\n  " +
                    "<ul class=\"dropdown-menu\">\r\n    ";

                foreach(var datum in this.Book.Data)
                {
                    html += $"<li><a class=\"dropdown-item\" href=\"/CardGrid/DownloadBook?BookID={this.Book.Id}&Format={datum.Format}\">"+datum.Format + " (" + Math.Round((datum.UncompressedSize ?? 0)/1000.0,1).ToString() + " KB)" +"</a></li>\r\n    ";
                }
                html += "</ul>\r\n";

                return html;
            }
        }

        public string SendToEReaderHTML
        {
            get
            {
                string html = "";
                if (this.Book.Data.Count(x => x.Format.ToUpper() == "PDF" || x.Format.ToUpper() == "EPUB") == 2)
                {
                    html = "<button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">\r\n    Send to EReader\r\n  </button>\r\n  " +
                    "<ul class=\"dropdown-menu\">\r\n    ";
                    foreach (var datum in this.Book.Data.Where(x => x.Format.ToUpper() == "PDF" || x.Format.ToUpper() == "EPUB"))
                    {
                        html += $"<li><a class=\"dropdown-item\" onclick=\"sendToReader({this.Book.Id}, '{datum.Format.ToLower()}');\">" + datum.Format + "</a></li>\r\n    ";
                    }
                    html += "</ul>\r\n";
                }
                else if (this.Book.Data.Any(x => x.Format.ToUpper() == "PDF"))
                {
                    html = $"<button class=\"btn btn-primary ms-1\" onclick=\"sendToReader({this.Book.Id},'pdf');\">Send PDF to eReader</button>";
                }
                else if (this.Book.Data.Any(x => x.Format.ToUpper() == "EPUB"))
                {
                    html = $"<button class=\"btn btn-primary ms-1\" onclick=\"sendToReader({this.Book.Id},'epub');\">Send Epub to eReader</button>";
                }

                return html;
            }
        }
    }
}
