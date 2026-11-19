using System.Linq.Expressions;
using CalibreLib.Models.Metadata;

namespace CalibreLib.Models
{
    public class BookQueryParameters
    {
        // Pagination (matches original int? signature)
        public int? PageNumber { get; set; }
        public int PageSize { get; set; } = 30;

        // Sorting
        public Expression<Func<Book, object?>>? OrderBy { get; set; }
        public string? SortField { get; set; }
        public bool Ascending { get; set; } = true;

        // Filtering
        public EFilterType FilterType { get; set; } = EFilterType.BookCardGrid;
        public int? FilterId { get; set; }
        public string? Query { get; set; }
        public SearchModel? SearchModel { get; set; }

        // User Context
        public string? UserId { get; set; }
    }
}