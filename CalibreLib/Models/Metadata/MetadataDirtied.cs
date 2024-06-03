using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class MetadataDirtied
{
    public int Id { get; set; }

    public Book Book { get; set; } = null!;
    public int BookId { get; set; }
}
