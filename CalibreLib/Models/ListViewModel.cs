namespace CalibreLib.Models
{
    public class ListViewModel
    {
        public int Count { get; set; } = 0;
        public int? Id { get; set; } = null;
        public string? Url { get; set; } = null;
        public string Title { get; set; } = string.Empty;

        public EFilterType? FilterType { get; set; }
    }
}
