using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksRatingsLink
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public virtual Rating Rating { get; set; } = null!;
    public int RatingId { get; set; }
}
