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
                return @"<a class=""link-dark"" href=""/book/" + id + @""">" + title + @"</a>";
            }
        }
        public string AuthorsHTML
        { 
            get
            {
                return String.Join(" & ",this.Authors.Select(x => @"<a class=""link-primary"" href=""author/" + x.Author.Id + @""">" + x.Author.Name + @"</a>"));
            } 
        }

        public string SeriesHTML
        {
            get
            {
                return String.Join(" & ", this.Series.Select(x => @"<a class=""link-primary"" href=""series/" + x.Series.Id + @""">" + x.Series.Name +"</a>" + @" (" + x.Book.SeriesIndex + ")"));
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

                return String.Join(" & ", this.Book.BookPublishers.Select(x => @"Publisher: " + @"<a class=""link-primary"" href=""publisher/" + x.Publisher.Id + @""">" + x.Publisher.Name + "</a>"));
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
                    tags = String.Join("", this.Tags.Select(x => @"<p class=""label label-blue mb-1 me-1""><a class=""link-light link-primary"" href=""categories/" + x.TagId.ToString() + @""">" + x.Tag.Name + "</a></p>"));
                }
                 
                return tags;
            }
        }

        public string IdentifiersHTML
        {
            get
            {
                return String.Join("", this.Book.Identifiers.Select(x => @"<p class=""label label-green mb-1 me-1""><a class=""link-light link-primary"" href=""identifier/" + x.Val + @""">" + x.Type + "</a></p>"));
            }
        }

        public bool Read { get; set; } = false;
        public bool Archived { get; set; } = false;

        public string Description { get; set; }

        public string AddShelfButtonHTML
        {
            get
            {
                return "<button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">\r\n    Add to Shelf\r\n  </button>\r\n  " +
                    "<ul class=\"dropdown-menu\">\r\n    " +
                    "<li><a class=\"dropdown-item\" href=\"#\">Action</a></li>\r\n    " +
                    "<li><a class=\"dropdown-item\" href=\"#\">Another action</a></li>\r\n    " +
                    "<li><a class=\"dropdown-item\" href=\"#\">Something else here</a></li>\r\n    " +
                    "<li><hr class=\"dropdown-divider\"></li>\r\n    " +
                    "<li><a class=\"dropdown-item\" href=\"#\">Separated link</a></li>\r\n  " +
                    "</ul>\r\n";
            }
        }

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
    }
}
