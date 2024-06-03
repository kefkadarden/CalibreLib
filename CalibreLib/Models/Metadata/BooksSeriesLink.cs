﻿using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class BooksSeriesLink
{
    public int Id { get; set; }

    public Book Book { get; set; } = null!;
    public int BookId { get; set; }

    public Series Series { get; set; } = null!;
    public int SeriesId { get; set; }
}
