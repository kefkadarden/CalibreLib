using CalibreLib.Areas.Identity.Data;

namespace CalibreLib.Models
{
    public class ShelfSelectionModel
    {
        public List<Shelf> UserShelves { get; set; } = [];
        public int BookId { get; set; }
    }
}
