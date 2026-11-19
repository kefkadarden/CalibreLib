using CalibreLib.Data;
using Microsoft.EntityFrameworkCore;
using CalibreLib.Areas.Identity.Data;

namespace CalibreLib.Services
{
    public class UserLibraryService(CalibreLibContext context) : IUserLibraryService
    {
        public async Task<HashSet<int>> GetReadBookIdsAsync(string userId)
        {
            var ids = await context.ReadBooks
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.ReadStatus == 1)
                .Select(x => x.BookId)
                .ToListAsync();

            return [.. ids];
        }

        public async Task<HashSet<int>> GetArchivedBookIdsAsync(string userId)
        {
            var ids = await context.ArchivedBooks
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.IsArchived)
                .Select(x => x.BookId)
                .ToListAsync();

            return [.. ids];
        }

        public async Task<List<Shelf>> GetUserShelvesAsync(string userId)
        {
            return await context.Shelves
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<Shelf?> GetUserShelfByIdAsync(string userId, int shelfId)
        {
            return await context.Shelves
                .AsNoTracking()
                .Where(s => s.UserId == userId && s.BookShelves.Any(bs => bs.ShelfId == shelfId))
                .FirstOrDefaultAsync();
        }

        public async Task<HashSet<int>> GetBookIdsOnShelvesAsync(string userId, IEnumerable<int> shelfIds)
        {
            var ids = await context.Shelves
                .AsNoTracking()
                .Where(s => s.UserId == userId && shelfIds.Contains(s.Id))
                .SelectMany(s => s.BookShelves.Select(bs => bs.BookId))
                .ToListAsync();

            return [.. ids];
        }

        // ==========================================
        // Shelves CRUD
        // ==========================================

        public async Task<Shelf> CreateShelfAsync(string userId, string name)
        {
            var now = DateTime.UtcNow;
            var shelf = new Shelf
            {
                UserId = userId,
                Name = name.Trim(),
                UUId = Guid.NewGuid().ToString(),
                Created = now,
                LastModified = now
            };

            await context.Shelves.AddAsync(shelf);
            await context.SaveChangesAsync();
            return shelf;
        }

        public async Task<bool> UpdateShelfNameAsync(string userId, int shelfId, string newName)
        {
            var shelf = await context.Shelves
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == shelfId);

            if (shelf is null)
                return false;

            shelf.Name = newName.Trim();
            shelf.LastModified = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShelfAsync(string userId, int shelfId)
        {
            var shelf = await context.Shelves
                .Include(s => s.BookShelves)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == shelfId);

            if (shelf is null)
                return false;

            context.BooksShelvesLinks.RemoveRange(shelf.BookShelves);
            context.Shelves.Remove(shelf);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddBookToShelfAsync(string userId, int shelfId, int bookId)
        {
            var shelf = await context.Shelves
                .Include(s => s.BookShelves)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == shelfId);

            if (shelf is null)
                return false;

            // Avoid duplicate entries on the same shelf
            if (shelf.BookShelves.Any(bs => bs.BookId == bookId))
                return true;

            var nextOrder = shelf.BookShelves.Count != 0
                ? shelf.BookShelves.Max(bs => bs.Order) + 1
                : 0;

            var link = new BooksShelvesLink
            {
                ShelfId = shelfId,
                BookId = bookId,
                Order = nextOrder,
                DateAdded = DateTime.UtcNow
            };

            await context.BooksShelvesLinks.AddAsync(link);
            shelf.LastModified = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveBookFromShelfAsync(string userId, int shelfId, int bookId)
        {
            var link = await context.BooksShelvesLinks
                .FirstOrDefaultAsync(bs =>
                    bs.ShelfId == shelfId &&
                    bs.BookId == bookId &&
                    bs.Shelf.UserId == userId);

            if (link is null)
                return false;

            context.BooksShelvesLinks.Remove(link);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateShelfBookOrderAsync(string userId, int shelfId, List<int> orderedBookIds)
        {
            var shelf = await context.Shelves
                .Include(s => s.BookShelves)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == shelfId);

            if (shelf is null)
                return;

            for (int i = 0; i < orderedBookIds.Count; i++)
            {
                var link = shelf.BookShelves.FirstOrDefault(bs => bs.BookId == orderedBookIds[i]);
                if (link != null)
                    link.Order = i;
            }

            shelf.LastModified = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        // ==========================================
        // ReadBooks CRUD
        // ==========================================

        public async Task SetReadStatusAsync(string userId, int bookId, int status)
        {
            var now = DateTime.UtcNow;
            var record = await context.ReadBooks
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);

            if (record is null)
            {
                record = new ReadBook
                {
                    UserId = userId,
                    BookId = bookId,
                    ReadStatus = status,
                    LastModified = now,
                    LastTimeStarted = status == 2 ? now : null, // e.g., 1 = Read, 2 = In Progress
                    TimesReading = status is 1 or 2 ? 1 : 0
                };
                await context.ReadBooks.AddAsync(record);
            }
            else
            {
                if (status == 2 && record.ReadStatus != 2)
                {
                    record.LastTimeStarted = now;
                    record.TimesReading++;
                }

                record.ReadStatus = status;
                record.LastModified = now;
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> RemoveReadRecordAsync(string userId, int bookId)
        {
            var record = await context.ReadBooks
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);

            if (record is null)
                return false;

            context.ReadBooks.Remove(record);
            await context.SaveChangesAsync();
            return true;
        }

        // ==========================================
        // ArchivedBooks CRUD
        // ==========================================

        public async Task SetArchiveStatusAsync(string userId, int bookId, bool isArchived)
        {
            var now = DateTime.UtcNow;
            var record = await context.ArchivedBooks
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);

            if (record is null)
            {
                await context.ArchivedBooks.AddAsync(new ArchivedBook
                {
                    UserId = userId,
                    BookId = bookId,
                    IsArchived = isArchived,
                    LastModified = now
                });
            }
            else
            {
                record.IsArchived = isArchived;
                record.LastModified = now;
            }

            await context.SaveChangesAsync();
        }

        public async Task ToggleArchiveAsync(string userId, int bookId)
        {
            var now = DateTime.UtcNow;
            var record = await context.ArchivedBooks
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);

            if (record is null)
            {
                await context.ArchivedBooks.AddAsync(new ArchivedBook
                {
                    UserId = userId,
                    BookId = bookId,
                    IsArchived = true,
                    LastModified = now
                });
            }
            else
            {
                record.IsArchived = !record.IsArchived;
                record.LastModified = now;
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> RemoveArchivedRecordAsync(string userId, int bookId)
        {
            var record = await context.ArchivedBooks
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);

            if (record is null)
                return false;

            context.ArchivedBooks.Remove(record);
            await context.SaveChangesAsync();
            return true;
        }
    }
}