using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksPluginDatum
{
    public int Id { get; set; }

    public Book Book { get; set; } = null!;
    public int? BookId { get; set; }

    public string? Name { get; set; }

    public string? Val { get; set; }
}
