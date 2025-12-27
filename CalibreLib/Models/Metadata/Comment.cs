namespace CalibreLib.Models.Metadata;

public partial class Comment
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int? BookId { get; set; }

    public string? Text { get; set; }
}
