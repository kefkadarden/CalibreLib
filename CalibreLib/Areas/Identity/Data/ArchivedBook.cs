using CalibreLib.Models.Metadata;

namespace CalibreLib.Areas.Identity.Data
{
    public class ArchivedBook
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;
        public bool IsArchived { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
