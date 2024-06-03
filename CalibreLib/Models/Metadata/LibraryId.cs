using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class LibraryId
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;
}
