using Microsoft.EntityFrameworkCore;
using CalibreLib.Models.Metadata;

namespace CalibreLib.Services
{
    public class MetadataService(MetadataDBContext context) : IMetadataService
    {
        // --- Collection Lookups ---

        public async Task<List<Author>> GetAuthorsAsync(bool includeEmpty = false)
        {
            var query = context.Authors.AsNoTracking();
            if (!includeEmpty)
                query = query.Where(a => a.BookAuthors.Any());

            return await query
                .OrderBy(a => a.Sort ?? a.Name)
                .ToListAsync();
        }

        public async Task<List<Publisher>> GetPublishersAsync(bool includeEmpty = false)
        {
            var query = context.Publishers.AsNoTracking();
            if (!includeEmpty)
                query = query.Where(p => p.BookPublishers.Any());

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Tag>> GetTagsAsync(bool includeEmpty = false)
        {
            var query = context.Tags.AsNoTracking();
            if (!includeEmpty)
                query = query.Where(t => t.BookTags.Any());

            return await query
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<Series>> GetSeriesAsync(bool includeEmpty = false)
        {
            var query = context.Series.AsNoTracking();
            if (!includeEmpty)
                query = query.Where(s => s.BookSeries.Any());

            return await query
                .OrderBy(s => s.Sort ?? s.Name)
                .ToListAsync();
        }

        public async Task<List<Language>> GetLanguagesAsync(bool includeEmpty = false)
        {
            var query = context.Languages.AsNoTracking();
            if (!includeEmpty)
                query = query.Where(l => l.BookLanguages.Any());

            return await query
                .OrderBy(l => l.LangCode)
                .ToListAsync();
        }

        public async Task<List<Rating>> GetRatingsAsync()
        {
            return await context.Ratings
                .AsNoTracking()
                .OrderByDescending(r => r.RatingValue)
                .ToListAsync();
        }

        // --- Single-Item Lookups by ID ---

        public async Task<Author?> GetAuthorByIdAsync(int id) =>
            await context.Authors
                .AsNoTracking()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

        public async Task<Publisher?> GetPublisherByIdAsync(int id) =>
            await context.Publishers
                .AsNoTracking()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

        public async Task<Tag?> GetTagByIdAsync(int id) =>
            await context.Tags
                .AsNoTracking()
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

        public async Task<Series?> GetSeriesByIdAsync(int id) =>
            await context.Series
                .AsNoTracking()
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

        public async Task<Language?> GetLanguageByIdAsync(int id) =>
            await context.Languages
                .AsNoTracking()
                .Where(l => l.Id == id)
                .FirstOrDefaultAsync();

        public async Task<Rating?> GetRatingByIdAsync(int id) =>
            await context.Ratings
                .AsNoTracking()
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();
    }
}