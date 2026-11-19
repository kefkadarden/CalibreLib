namespace CalibreLib.Models
{
    public class PagedResult<T>
    {
        public required List<T> Items { get; init; }
        public int TotalCount { get; init; }
        public int? PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    }
}