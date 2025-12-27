namespace CalibreLib.Models.Metadata;

public partial class Datum
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int? BookId { get; set; }

    public string? Format { get; set; }

    public int? UncompressedSize { get; set; }

    public string? Name { get; set; }
}
