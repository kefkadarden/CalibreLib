using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Sort { get; set; }

    public string Link { get; set; } = null!;

    public virtual List<BooksAuthorsLink> BookAuthors { get; set; } = [];
}
