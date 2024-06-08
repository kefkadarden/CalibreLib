using CalibreLib.Models.Metadata;

namespace CalibreLib.Areas.Identity.Data
{
    public class ArchivedBook
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public int BookId { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
