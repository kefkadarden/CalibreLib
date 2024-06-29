using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalibreLib.Areas.Identity.Data
{
    public class Shelf
    {
        public int Id { get; set; }
        public string? UUId { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }

        [NotMapped]
        public List<BookCardModel> BookCards { get; set; } = [];

        public virtual List<BooksShelvesLink> BookShelves { get; set; } = [];
    }
}
