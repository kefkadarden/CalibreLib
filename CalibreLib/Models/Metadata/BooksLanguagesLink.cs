using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksLanguagesLink
{
    public int Id { get; set; }

    public Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public Language Language { get; set; } = null!;
    public int LanguageId { get; set; }

    public int ItemOrder { get; set; }
}
