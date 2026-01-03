namespace CalibreLib.Models.Metadata;

public partial class Publisher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Sort { get; set; }

    public virtual List<BooksPublishersLink> BookPublishers { get; set; } = [];
}
