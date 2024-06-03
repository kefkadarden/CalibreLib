using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksAuthorsLink
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public virtual Author Author { get; set; } = null!;
    public int AuthorId { get; set; }
}
