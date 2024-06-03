using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksRatingsLink
{
    public int Id { get; set; }

    public Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public Rating Rating { get; set; } = null!;
    public int RatingId { get; set; }
}
