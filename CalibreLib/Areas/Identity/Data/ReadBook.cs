namespace CalibreLib.Areas.Identity.Data
{
    public class ReadBook
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public int BookId { get; set; }
        public int ReadStatus { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? LastTimeStarted { get; set; }
        public int TimesReading { get; set; }
    }
}
