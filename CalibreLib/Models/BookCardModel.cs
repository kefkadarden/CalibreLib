﻿using System.Drawing;

namespace CalibreLib.Models
{
    public class BookCardModel
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string CoverImage { get; set; }
        public string? Author { get; set; }

        public int? Rating { get; set; }
       
    }
}
