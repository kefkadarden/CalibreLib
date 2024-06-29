using CalibreLib.Models.Metadata;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalibreLib.Areas.Identity.Data
{
    public class BooksShelvesLink
    {
        public int Id { get; set; }
        [NotMapped]
        public Book Book { get; set; } = null!;
        public int BookId { get; set; }

        public virtual Shelf Shelf { get; set; } = null!;
        public int ShelfId { get; set; }

        public int Order { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
