using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksSeriesLink
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public virtual Series Series { get; set; } = null!;
    public int SeriesId { get; set; }
}
