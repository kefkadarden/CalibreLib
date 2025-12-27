namespace CalibreLib.Models.Metadata;

public partial class ConversionOption
{
    public int Id { get; set; }

    public string Format { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
    public int? BookId { get; set; }

    public byte[] Data { get; set; } = null!;
}
