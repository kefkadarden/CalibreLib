using CalibreLib.Models.Metadata;
using Microsoft.EntityFrameworkCore;

namespace CalibreLib.Data
{
    public class BookRepository : IRepositoryBase<Book>
    {
        private bool disposedValue = false;
        private MetadataDBContext context;
        public int PageSize { get; set; } = 30;

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

        public async Task<int> GetPageCountAsync()
        {
            decimal dPageSize = Convert.ToDecimal(PageSize);
            var count = await context.Books.CountAsync();
            return Convert.ToInt32(Math.Ceiling( Convert.ToDecimal(count / dPageSize) ));
        }

        public IEnumerable<Book> GetBooks(int? pageNumber, Func<Book, object> orderBy, bool ascending = true)
        {
            var numRecordsToSkip = pageNumber * PageSize;
            if (ascending)
                return context.Books.OrderBy(orderBy).Skip(Convert.ToInt32(numRecordsToSkip)).Take(PageSize).ToList();
            else
                return context.Books.OrderByDescending(orderBy).Skip(Convert.ToInt32(numRecordsToSkip)).Take(PageSize).ToList();
        }

        public async Task<Book> GetByIDAsync(int id)
        {
            return await context.Books.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Identifier>> GetIdentifiersAsync()
        {
            return await context.Identifiers.ToListAsync();
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
