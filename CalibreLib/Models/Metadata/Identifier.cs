namespace CalibreLib.Models.Metadata;

public partial class Identifier
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int? BookId { get; set; }

    public string? Type { get; set; }

    public string? Val { get; set; }
}
