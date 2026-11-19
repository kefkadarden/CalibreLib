using CalibreLib.Models;
using CalibreLib.Models.Metadata;

namespace CalibreLib.Services
{
    public interface IBookService
    {
        Task<PagedResult<Book>> GetBooksAsync(BookQueryParameters parameters);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<IEnumerable<Book>> GetByBookListAsync(IEnumerable<int> bookIds);
        Task<IEnumerable<Book>> GetByArchivedAsync(string userId);
        Task<IEnumerable<Book>> SearchByQueryAsync(string query);
        Task<IEnumerable<Book>> SearchByModelAsync(SearchModel model, string? userId = null);
        Task<IEnumerable<Book>> GetByAuthorAsync(int id);
        Task<IEnumerable<Book>> GetBySeriesAsync(int id);
        Task<IEnumerable<Book>> GetByPublisherAsync(int id);
        Task<IEnumerable<Book>> GetByTagAsync(int id);
        Task<IEnumerable<Book>> GetByRatingAsync(int id);
        Task<IEnumerable<Book>> GetByLanguageAsync(int id);
        Task<List<Identifier>> GetIdentifiersAsync();
        Task<List<BookCardModel>> GetBookCardModelsAsync(IEnumerable<Book> books, string? userId = null);
        Task<BookCardModel> GetBookCardModelAsync(Book book, string? userId = null);
        Task AddBookAsync(Book item);
        Task UpdateBookAsync(Book item);
        Task DeleteBookAsync(Book item);
    }
}