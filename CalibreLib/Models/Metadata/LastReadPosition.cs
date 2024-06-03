using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class LastReadPosition
{
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public string Format { get; set; } = null!;

    public string User { get; set; } = null!;

    public string Device { get; set; } = null!;

    public string Cfi { get; set; } = null!;

    public double Epoch { get; set; }

    public double PosFrac { get; set; }
}
