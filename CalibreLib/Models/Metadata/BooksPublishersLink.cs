using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksPublishersLink
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public virtual Publisher Publisher { get; set; } = null!;
    public int PublisherId { get; set; }
}
