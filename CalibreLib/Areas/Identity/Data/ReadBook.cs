using CalibreLib.Models.Metadata;

namespace CalibreLib.Areas.Identity.Data
{
    public class ReadBook
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;
        public int ReadStatus { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? LastTimeStarted {  get; set; }
        public int TimesReading { get; set; }
    }
}
