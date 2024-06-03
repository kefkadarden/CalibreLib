using CalibreLib.Models.Metadata;
using Microsoft.EntityFrameworkCore;

namespace CalibreLib.Data
{
    public class BookRepository : IRepositoryBase<Book>
    {
        private bool disposedValue = false;
        private MetadataDBContext context;

        public BookRepository(MetadataDBContext context)
        {
            this.context = context;
        }

        public void Delete(Book item)
        {
            context.Books.Remove(item);
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await context.Books.ToListAsync();
        }

        public IEnumerable<Book> GetBooks(int? pageNumber, Func<Book, object> orderBy, bool ascending = true)
        {
            const int pageSize = 30;
            var numRecordsToSkip = pageNumber * pageSize;
            if (ascending)
                return context.Books.OrderBy(orderBy).Skip(Convert.ToInt32(numRecordsToSkip)).Take(pageSize).ToList();
            else
                return context.Books.OrderByDescending(orderBy).Skip(Convert.ToInt32(numRecordsToSkip)).Take(pageSize).ToList();
        }

        public async Task<Book> GetByIDAsync(int id)
        {
            return await context.Books.FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Insert(Book item)
        {
            context.Books.Add(item);
        }

        public async void SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public void Update(Book item)
        {
            context.Books.Update(item);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }

                disposedValue = true;
            }
        }

        ~BookRepository()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
