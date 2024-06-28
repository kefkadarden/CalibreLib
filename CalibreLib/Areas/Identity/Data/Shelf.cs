namespace CalibreLib.Areas.Identity.Data
{
    public class Shelf
    {
        public int Id { get; set; }
        public string? UUId { get; set; }
        public string? Name { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
