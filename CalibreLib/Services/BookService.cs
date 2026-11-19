using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using Microsoft.EntityFrameworkCore;

namespace CalibreLib.Services
{
    public class BookService(
        MetadataDBContext context,
        IUserLibraryService userLibraryService,
        BookFileManager bookFileManager) : IBookService
    {
        public async Task<PagedResult<Book>> GetBooksAsync(BookQueryParameters parameters)
        {
            var query = context.Books.AsNoTracking().AsQueryable();

            // 1. Apply Search or Category Filter
            if (!string.IsNullOrWhiteSpace(parameters.Query) && parameters.FilterType != EFilterType.SearchList)
            {
                query = ApplyTextSearch(query, parameters.Query);
            }
            else if (parameters.SearchModel != null)
            {
                query = await ApplyAdvancedSearchAsync(query, parameters.SearchModel, parameters.UserId);
            }
            else
            {
                query = await ApplyCategoryFilterAsync(query, parameters.FilterType, parameters.FilterId, parameters.UserId);
            }

            // 2. Get total matching record count in MetadataDBContext before pagination
            var totalCount = await query.CountAsync();

            // 3. Apply Sorting in SQL
            query = ApplySorting(query, parameters);

            // 4. Apply Pagination in SQL (coalescing null PageNumber to 0 to match original Skip(0) behavior)
            var skipCount = (parameters.PageNumber ?? 0) * parameters.PageSize;
            var items = await query
                .Skip(skipCount)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Book>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

        public async Task<IEnumerable<Book>> GetAllAsync() =>
            await context.Books.AsNoTracking().ToListAsync();

        public async Task<Book?> GetByIdAsync(int id) =>
            await context.Books.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<Book>> GetByBookListAsync(IEnumerable<int> bookIds) =>
            await context.Books.AsNoTracking().Where(x => bookIds.Contains(x.Id)).ToListAsync();

        public async Task<IEnumerable<Book>> GetByArchivedAsync(string userId)
        {
            var archivedIds = await userLibraryService.GetArchivedBookIdsAsync(userId);
            if (archivedIds.Count == 0)
                return [];

            return await context.Books
                .AsNoTracking()
                .Where(x => archivedIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchByQueryAsync(string query) =>
            await ApplyTextSearch(context.Books.AsNoTracking(), query).ToListAsync();

        public async Task<IEnumerable<Book>> SearchByModelAsync(SearchModel model, string? userId = null)
        {
            var query = await ApplyAdvancedSearchAsync(context.Books.AsNoTracking(), model, userId);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByAuthorAsync(int id) =>
            await context.Books.AsNoTracking().Where(x => x.BookAuthors.Any(y => y.AuthorId == id)).ToListAsync();

        public async Task<IEnumerable<Book>> GetBySeriesAsync(int id) =>
            await context.Books.AsNoTracking().Where(x => x.BookSeries.Any(y => y.SeriesId == id)).ToListAsync();

        public async Task<IEnumerable<Book>> GetByPublisherAsync(int id) =>
            await context.Books.AsNoTracking().Where(x => x.BookPublishers.Any(y => y.PublisherId == id)).ToListAsync();

        public async Task<IEnumerable<Book>> GetByTagAsync(int id) =>
            await context.Books.AsNoTracking().Where(x => x.BookTags.Any(y => y.TagId == id)).ToListAsync();

        public async Task<IEnumerable<Book>> GetByRatingAsync(int id)
        {
            if (id == -1)
                return await context.Books.AsNoTracking().Where(x => !x.BookRatings.Any()).ToListAsync();

            return await context.Books.AsNoTracking().Where(x => x.BookRatings.Any(y => y.RatingId == id)).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByLanguageAsync(int id) =>
            await context.Books.AsNoTracking().Where(x => x.BookLanguages.Any(y => y.LanguageId == id)).ToListAsync();

        public async Task<List<Identifier>> GetIdentifiersAsync() =>
            await context.Identifiers.AsNoTracking().ToListAsync();

        public async Task<List<BookCardModel>> GetBookCardModelsAsync(IEnumerable<Book> books, string? userId = null)
        {
            var readBookIds = userId != null
                ? await userLibraryService.GetReadBookIdsAsync(userId)
                : [];

            var archivedBookIds = userId != null
                ? await userLibraryService.GetArchivedBookIdsAsync(userId)
                : [];

            var models = new List<BookCardModel>();
            foreach (var book in books)
            {
                models.Add(await BuildBookCardModelAsync(book, readBookIds, archivedBookIds));
            }
            return models;
        }

        public async Task<BookCardModel> GetBookCardModelAsync(Book book, string? userId = null)
        {
            var readBookIds = userId != null
                ? await userLibraryService.GetReadBookIdsAsync(userId)
                : [];

            var archivedBookIds = userId != null
                ? await userLibraryService.GetArchivedBookIdsAsync(userId)
                : [];

            return await BuildBookCardModelAsync(book, readBookIds, archivedBookIds);
        }

        public async Task AddBookAsync(Book item)
        {
            await context.Books.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book item)
        {
            context.Books.Update(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(Book item)
        {
            context.Books.Remove(item);
            await context.SaveChangesAsync();
        }

        #region Private Query & Mapping Helpers

        private async Task<BookCardModel> BuildBookCardModelAsync(
            Book book,
            HashSet<int> readBookIds,
            HashSet<int> archivedBookIds)
        {
            var cover = await bookFileManager.GetBookCoverAsync(book);
            return new BookCardModel
            {
                id = book.Id,
                title = book.Title,
                Book = book,
                Authors = book.BookAuthors,
                Series = book.BookSeries,
                Languages = book.BookLanguages,
                Tags = book.BookTags,
                CoverImage = cover,
                RatingValue = book.BookRatings.FirstOrDefault()?.Rating.RatingValue ?? 0,
                Archived = archivedBookIds.Contains(book.Id),
                Read = readBookIds.Contains(book.Id),
                Description = book.Comment?.Text ?? string.Empty
            };
        }

        private static IQueryable<Book> ApplyTextSearch(IQueryable<Book> query, string searchText)
        {
            var term = searchText.ToLower();

            return query.Where(x =>
                x.Title.ToLower().Contains(term) ||
                (x.Isbn != null && x.Isbn.ToLower().Contains(term)) ||
                (x.Lccn != null && x.Lccn.ToLower().Contains(term)) ||
                x.Identifiers.Any(i => i.Val.ToLower().Contains(term)) ||
                x.BookTags.Any(t => t.Tag.Name.ToLower().Contains(term)) ||
                x.BookPublishers.Any(p => p.Publisher.Name.ToLower().Contains(term)) ||
                x.BookAuthors.Any(a => a.Author.Name.ToLower().Contains(term)) ||
                x.BookSeries.Any(s => s.Series.Name.ToLower().Contains(term)));
        }

        private async Task<IQueryable<Book>> ApplyAdvancedSearchAsync(
            IQueryable<Book> query,
            SearchModel model,
            string? userId)
        {
            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.ToLower().Contains(model.Title.ToLower()));

            if (model.Authors.Count != 0)
                query = query.Where(x => x.BookAuthors.Any(y => model.Authors.Contains(y.AuthorId)));

            if (model.Publishers.Count != 0)
                query = query.Where(x => x.BookPublishers.Any(y => model.Publishers.Contains(y.PublisherId)));

            if (model.PublishedDateFrom.HasValue)
                query = query.Where(x => x.Pubdate >= model.PublishedDateFrom.Value);

            if (model.PublishedDateTo.HasValue)
                query = query.Where(x => x.Pubdate <= model.PublishedDateTo.Value);

            if (model.ReadStatus is 1 && userId != null)
            {
                var readBookIds = await userLibraryService.GetReadBookIdsAsync(userId);
                query = query.Where(b => readBookIds.Contains(b.Id));
            }

            if (model.Shelves.Count != 0 && userId != null)
            {
                var shelfBookIds = await userLibraryService.GetBookIdsOnShelvesAsync(userId, model.Shelves);
                query = query.Where(b => shelfBookIds.Contains(b.Id));
            }

            if (model.ExcludeShelves.Count != 0 && userId != null)
            {
                var excludedShelfBookIds = await userLibraryService.GetBookIdsOnShelvesAsync(userId, model.ExcludeShelves);
                query = query.Where(b => !excludedShelfBookIds.Contains(b.Id));
            }

            if (model.Tags.Count != 0)
                query = query.Where(x => x.BookTags.Any(y => model.Tags.Contains(y.TagId)));

            if (model.Series.Count != 0)
                query = query.Where(x => x.BookSeries.Any(y => model.Series.Contains(y.SeriesId)));

            if (model.Languages.Count != 0)
                query = query.Where(x => x.BookLanguages.Any(y => model.Languages.Contains(y.LanguageId)));

            if (model.ExcludeTags.Count != 0)
                query = query.Where(x => !x.BookTags.Any(y => model.ExcludeTags.Contains(y.TagId)));

            if (model.ExcludeSeries.Count != 0)
                query = query.Where(x => !x.BookSeries.Any(y => model.ExcludeSeries.Contains(y.SeriesId)));

            if (model.ExcludeLanguages.Count != 0)
                query = query.Where(x => !x.BookLanguages.Any(y => model.ExcludeLanguages.Contains(y.LanguageId)));

            if (!string.IsNullOrEmpty(model.Description))
            {
                var desc = model.Description.ToLower();
                query = query.Where(x => x.Comment != null && x.Comment.Text.ToLower().Contains(desc));
            }

            if (model.RatingAbove.HasValue)
                query = query.Where(x => x.BookRatings.Any(y => y.Rating.RatingValue >= model.RatingAbove.Value));

            if (model.RatingBelow.HasValue)
                query = query.Where(x => x.BookRatings.Any(y => y.Rating.RatingValue <= model.RatingBelow.Value));

            return query;
        }

        private async Task<IQueryable<Book>> ApplyCategoryFilterAsync(
            IQueryable<Book> query,
            EFilterType type,
            int? filterId,
            string? userId)
        {
            int id = filterId ?? -1;

            switch (type)
            {
                case EFilterType.Authors:
                    return query.Where(b => b.BookAuthors.Any(a => a.AuthorId == id));
                case EFilterType.Categories:
                    return query.Where(b => b.BookTags.Any(t => t.TagId == id));
                case EFilterType.Series:
                    return query.Where(b => b.BookSeries.Any(s => s.SeriesId == id));
                case EFilterType.Publishers:
                    return query.Where(b => b.BookPublishers.Any(p => p.PublisherId == id));
                case EFilterType.Languages:
                    return query.Where(b => b.BookLanguages.Any(l => l.LanguageId == id));
                case EFilterType.Ratings:
                    return id == -1
                        ? query.Where(b => !b.BookRatings.Any())
                        : query.Where(b => b.BookRatings.Any(r => r.RatingId == id));
                case EFilterType.Shelf:
                    if (userId == null || !filterId.HasValue)
                        return query.Where(_ => false);

                    var shelfBookIds = await userLibraryService.GetBookIdsOnShelvesAsync(userId, [filterId.Value]);
                    return query.Where(b => shelfBookIds.Contains(b.Id));
                case EFilterType.Archived:
                    if (userId == null)
                        return query.Where(_ => false);

                    var archivedBookIds = await userLibraryService.GetArchivedBookIdsAsync(userId);
                    return query.Where(b => archivedBookIds.Contains(b.Id));
                default:
                    return query;
            }
        }

        private static IQueryable<Book> ApplySorting(IQueryable<Book> query, BookQueryParameters parameters)
        {
            if (parameters.OrderBy != null)
            {
                return parameters.Ascending
                    ? query.OrderBy(parameters.OrderBy)
                    : query.OrderByDescending(parameters.OrderBy);
            }

            return (parameters.SortField?.ToLower(), parameters.Ascending) switch
            {
                ("title", true)    => query.OrderBy(b => b.Title),
                ("title", false)   => query.OrderByDescending(b => b.Title),
                ("pubdate", true)  => query.OrderBy(b => b.Pubdate),
                ("pubdate", false) => query.OrderByDescending(b => b.Pubdate),
                _                  => query.OrderBy(b => b.Id)
            };
        }

        #endregion
    }
}