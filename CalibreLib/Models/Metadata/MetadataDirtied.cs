namespace CalibreLib.Models.Metadata;

public partial class MetadataDirtied
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int BookId { get; set; }
}
