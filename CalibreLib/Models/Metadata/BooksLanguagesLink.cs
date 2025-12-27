namespace CalibreLib.Models.Metadata;

public partial class BooksLanguagesLink
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public virtual Language Language { get; set; } = null!;
    public int LanguageId { get; set; }

    public int ItemOrder { get; set; }
}
