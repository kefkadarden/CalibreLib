using CalibreLib.Models.Metadata;
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

        public string BookString
        {
            get
            {
                return @"<a class=""link-dark"" href=""book/" + id + @""">" + title + @"</a>";
            }
        }
        public string AuthorsString 
        { 
            get
            {
                return String.Join(" & ",this.Authors.Select(x => @"<a class=""link-primary"" href=""author/" + x.Author.Id + @""">" + x.Author.Name + @"</a>"));
            } 
        }

        public string SeriesString
        {
            get
            {
                return String.Join(" & ", this.Series.Select(x => @"<a class=""link-primary"" href=""series/" + x.Series.Id + @""">" + x.Series.Name +"</a>" + @" (" + x.Book.SeriesIndex + ")"));
            }
        }
        
        public int? Rating { get; set; }
       
    }
}
