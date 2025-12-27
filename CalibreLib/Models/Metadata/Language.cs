namespace CalibreLib.Models.Metadata;

public partial class Language
{
    public int Id { get; set; }

    public string? LangCode { get; set; }

    public virtual List<BooksLanguagesLink> BookLanguages { get; set; } = [];
}
