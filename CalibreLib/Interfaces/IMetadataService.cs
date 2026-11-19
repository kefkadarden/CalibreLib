using CalibreLib.Models.Metadata;

namespace CalibreLib.Services;

public interface IMetadataService
{
    Task<List<Author>> GetAuthorsAsync(bool includeEmpty = false);
    Task<List<Publisher>> GetPublishersAsync(bool includeEmpty = false);
    Task<List<Tag>> GetTagsAsync(bool includeEmpty = false);
    Task<List<Series>> GetSeriesAsync(bool includeEmpty = false);
    Task<List<Language>> GetLanguagesAsync(bool includeEmpty = false);
    Task<List<Rating>> GetRatingsAsync();
    
    Task<Author?> GetAuthorByIdAsync(int id);
    Task<Publisher?> GetPublisherByIdAsync(int id);
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Series?> GetSeriesByIdAsync(int id);
    Task<Language?> GetLanguageByIdAsync(int id);
    Task<Rating?> GetRatingByIdAsync(int id);
}