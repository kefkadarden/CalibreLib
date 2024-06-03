using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class Series
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Sort { get; set; }

    public virtual List<BooksSeriesLink> BookSeries { get; set; } = [];
}
