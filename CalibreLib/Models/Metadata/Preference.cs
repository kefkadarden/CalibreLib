using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class Preference
{
    public int Id { get; set; }

    public string? Key { get; set; }

    public string? Val { get; set; }
}
