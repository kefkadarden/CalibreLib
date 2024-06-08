using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalibreLib.Models.Metadata;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Sort { get; set; }

    public DateTime? Timestamp { get; set; }

    public DateTime? Pubdate { get; set; }

    public double SeriesIndex { get; set; }

    public string? AuthorSort { get; set; }

    public string? Isbn { get; set; }

    public string? Lccn { get; set; }

    public string Path { get; set; } = null!;

    public int Flags { get; set; }

    public string? Uuid { get; set; }

    public bool? HasCover { get; set; }

    public DateTime LastModified { get; set; }

    public virtual List<BooksAuthorsLink> BookAuthors { get; set; } = [];
    public virtual List<BooksLanguagesLink> BookLanguages { get; set; } = [];
    public virtual List<BooksPublishersLink> BookPublishers { get; set; } = [];
    public virtual List<BooksRatingsLink> BookRatings { get; set; } = [];
    public virtual List<BooksSeriesLink> BookSeries { get; set; } = [];
    public virtual List<BooksTagsLink> BookTags { get; set; } = [];

    public virtual List<Identifier> Identifiers { get; set; } = [];
}
