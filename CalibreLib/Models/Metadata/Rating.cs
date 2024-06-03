using System;
using System.Collections.Generic;

namespace CalibreLib.Models.Metadata;

public partial class Rating
{
    public int Id { get; set; }

    public int? Rating1 { get; set; }

    public int? RatingValue
    { 
        get 
        {
            int rating = 0;
            if (this.Rating1 > 0 && this.Rating1 < 4)
                rating = 1;
            else if (this.Rating1 >= 4 && this.Rating1 <= 5)
                rating = 2;
            else if (this.Rating1 >= 6 && this.Rating1 <= 7)
                rating = 3;
            else if (this.Rating1 >= 8 && this.Rating1 <= 9)
                rating = 4;
            else if (this.Rating1 >= 10)
                rating = 5;

            return rating;
        } 
    }

    public virtual List<BooksRatingsLink> BookRatings { get; set; } = [];
}
