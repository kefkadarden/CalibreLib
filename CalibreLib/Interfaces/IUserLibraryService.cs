using CalibreLib.Areas.Identity.Data;

namespace CalibreLib.Services
{
    public interface IUserLibraryService
    {
        Task<HashSet<int>> GetReadBookIdsAsync(string userId);
        Task<HashSet<int>> GetArchivedBookIdsAsync(string userId);
        Task<List<Shelf>> GetUserShelvesAsync(string userId);
        Task<Shelf?> GetUserShelfByIdAsync(string userId, int shelfId);
        Task<HashSet<int>> GetBookIdsOnShelvesAsync(string userId, IEnumerable<int> shelfIds);
        
        // --- Shelves CRUD ---
        Task<Shelf> CreateShelfAsync(string userId, string name);
        Task<bool> UpdateShelfNameAsync(string userId, int shelfId, string newName);
        Task<bool> DeleteShelfAsync(string userId, int shelfId);
        Task<bool> AddBookToShelfAsync(string userId, int shelfId, int bookId);
        Task<bool> RemoveBookFromShelfAsync(string userId, int shelfId, int bookId);
        Task UpdateShelfBookOrderAsync(string userId, int shelfId, List<int> orderedBookIds);

        // --- ReadBooks CRUD ---
        Task SetReadStatusAsync(string userId, int bookId, int status);
        Task<bool> RemoveReadRecordAsync(string userId, int bookId);

        // --- ArchivedBooks CRUD ---
        Task SetArchiveStatusAsync(string userId, int bookId, bool isArchived);
        Task ToggleArchiveAsync(string userId, int bookId);
        Task<bool> RemoveArchivedRecordAsync(string userId, int bookId);
    }
}