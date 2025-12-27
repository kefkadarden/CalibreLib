namespace CalibreLib.Models.Metadata;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual List<BooksTagsLink> BookTags { get; set; } = [];
}
