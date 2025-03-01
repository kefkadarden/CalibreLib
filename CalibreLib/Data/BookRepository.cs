﻿using Azure.Core;
using CalibreLib.Areas.Identity.Data;
using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using CalibreLib.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CalibreLib.Data
{
    public class BookRepository : IRepositoryBase<Book>
    {
        private bool disposedValue = false;
        private MetadataDBContext context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public int PageSize { get; set; } = 30;

        public BookRepository(MetadataDBContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _env = env;
            _userManager = userManager;
            _contextAccessor = httpContextAccessor;
        }

        public void Delete(Book item)
        {
            context.Books.Remove(item);
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await context.Books.ToListAsync();
        }

        public int GetPageCount(IEnumerable<Book> books = null!)
        {
            if (books == null)
                books = context.Books;

            decimal dPageSize = Convert.ToDecimal(PageSize);
            var count = books.Count();
            return Convert.ToInt32(Math.Ceiling( Convert.ToDecimal(count / dPageSize) ));
        }

        public async Task<IEnumerable<Book>> GetBooks(int? pageNumber, Func<Book, object> orderBy, string? query, bool ascending = true, EFilterType type = EFilterType.BookCardGrid, int? filterid = null, SearchModel? searchModel = null)
        {
            var numRecordsToSkip = pageNumber * PageSize;
            IQueryable<Book> books = Enumerable.Empty<Book>().AsQueryable();
            if (query != null && type != EFilterType.SearchList)
            {
                var queryResult = await GetByQueryAsync(query);
                books = queryResult.AsQueryable();
            }
            else if (searchModel != null)
            {
                var queryResult = await GetByQueryAsync(searchModel);
                books = queryResult.AsQueryable();
            }
            else
                books = context.Books;

            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            var bookids = user?.Shelves.FirstOrDefault(x => x.Id == filterid)?.BookShelves.Select(x => x.BookId);

            int filterID = -1;
            if (filterid != null)
                filterID = (int)filterid;

            IEnumerable<Book> booksList = new List<Book>();
            switch(type)
            {
                case EFilterType.Authors:
                    booksList = await this.GetByAuthorAsync(filterID);
                    break;
                case EFilterType.Categories:
                    booksList = await this.GetByTagAsync(filterID);
                    break;
                case EFilterType.Shelf:
                    booksList = await this.GetByBookListAsync(bookids);
                    break;
                case EFilterType.Series:
                    booksList = await this.GetBySeriesAsync(filterID);
                    break;
                case EFilterType.Ratings:
                    booksList = await this.GetByRatingAsync(filterID);
                    break;
                case EFilterType.Publishers:
                    booksList = await this.GetByPublisherAsync(filterID);
                    break;
                case EFilterType.Languages:
                    booksList = await this.GetByLanguageAsync(filterID);
                    break;
                case EFilterType.Archived:
                    booksList = await this.GetByArchivedAsync();
                    break;
                default:
                    booksList = books.ToList();
                    break;
            }

            if (ascending)
                return booksList.OrderBy(orderBy).Skip(Convert.ToInt32(numRecordsToSkip)).Take(PageSize).ToList();
            else
                return booksList.OrderByDescending(orderBy).Skip(Convert.ToInt32(numRecordsToSkip)).Take(PageSize).ToList();
        }

        public async Task<Book> GetByIDAsync(int id)
        {
            return await context.Books.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Book>> GetByBookListAsync(IEnumerable<int> bookids)
        {
            return await context.Books.Where(x => bookids.Contains(x.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByArchivedAsync()
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            var bookIds = user?.ArchivedBooks.Where(x => x.IsArchived).Select(x => x.BookId);
            return await context.Books.Where(x => bookIds.Contains(x.Id)).ToListAsync();
        }
        
        public async Task<IEnumerable<Book>> GetByQueryAsync(string query)
        {
            //Contains() Ignore Case causes Sqlite EF exception currently. Still need to investigate why. Using ToLower() currently for case insenstive comparsion.
            //Potential lead: https://github.com/dotnet/efcore/issues/8033
            query = query.ToLower();

            
                return await context.Books.Where(x => x.Title.ToLower().Contains(query) ||
                                                      x.Isbn.ToLower().Contains(query) ||
                                                      x.Lccn.ToLower().Contains(query) ||
                                                      x.Identifiers.Any(x => x.Val.ToLower().Contains(query)) || 
                                                      x.BookTags.Any(x => x.Tag.Name.ToLower().Contains(query)) ||
                                                      x.BookPublishers.Any(x => x.Publisher.Name.ToLower().Contains(query)) ||
                                                      x.BookAuthors.Any(x => x.Author.Name.ToLower().Contains(query)) ||
                                                      x.BookSeries.Any(x => x.Series.Name.ToLower().Contains(query))).ToListAsync();
            
        }

        public async Task<IEnumerable<Book>> GetByQueryAsync(SearchModel model)
        {
            var query = context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(model.Title))
            {
                query = query.Where(x => x.Title.Contains(model.Title));
            }

            if (model.Authors.Any())
            {
                query = query.Where(x => x.BookAuthors.Any(y => model.Authors.Contains(y.AuthorId)));
            }

            if (model.Publishers.Any())
            {
                query = query.Where(x => x.BookPublishers.Any(y => model.Publishers.Contains(y.PublisherId)));
            }

            if (model.PublishedDateFrom.HasValue)
            {
                query = query.Where(x => x.Pubdate >= model.PublishedDateFrom.Value);
            }

            if (model.PublishedDateTo.HasValue)
            {
                query = query.Where(x => x.Pubdate <= model.PublishedDateTo.Value);
            }

            if (model.ReadStatus.HasValue)
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var bookIds = user?.ReadBooks.Where(x => x.ReadStatus == model.ReadStatus.Value).Select(x => x.BookId);
                if (bookIds != null && bookIds.Any())
                    query = query.Where(x => bookIds.Contains(x.Id));
            }

            if (model.Tags.Any())
            {
                query = query.Where(x => x.BookTags.Any(y => model.Tags.Contains(y.TagId)));
            }

            if (model.Series.Any())
            {
                query = query.Where(x => x.BookSeries.Any(y => model.Series.Contains(y.SeriesId)));
            }

            if (model.Shelves.Any())
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var bookIds = user?.Shelves.Where(s => model.Shelves.Contains(s.Id))
                                           .SelectMany(s => s.BookShelves.Select(bs => bs.BookId));
                query = query.Where(x => bookIds.Contains(x.Id));
            }

            if (model.Languages.Any())
            {
                query = query.Where(x => x.BookLanguages.Any(y => model.Languages.Contains(y.LanguageId)));
            }

            if (model.ExcludeTags.Any())
            {
                query = query.Where(x => x.BookTags.Any(y => !model.ExcludeTags.Contains(y.TagId)));
            }

            if (model.ExcludeSeries.Any())
            {
                query = query.Where(x => x.BookSeries.Any(y => !model.ExcludeSeries.Contains(y.SeriesId)));
            }

            if (model.ExcludeShelves.Any())
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                var bookIds = user?.Shelves.Where(s => !model.ExcludeShelves.Contains(s.Id))
                                           .SelectMany(s => s.BookShelves.Select(bs => bs.BookId));
                query = query.Where(x => bookIds.Contains(x.Id));
            }

            if (model.ExcludeLanguages.Any())
            {
                query = query.Where(x => x.BookLanguages.Any(y => !model.ExcludeLanguages.Contains(y.LanguageId)));
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                query = query.Where(x => x.Comment != null && x.Comment.Text.Contains(model.Description, StringComparison.OrdinalIgnoreCase));
            }

            var books = await query.ToListAsync();


            if (model.RatingAbove.HasValue)
            {
                books = books.Where(x => x.BookRatings.Any(y => y.Rating.RatingValue >= model.RatingAbove.Value && y.Rating.RatingValue <= model.RatingBelow.Value)).ToList();
            }
            if (model.RatingBelow.HasValue)
            {
                books = books.Where(x => x.BookRatings.Any(y => y.Rating.RatingValue <= model.RatingBelow.Value)).ToList();
            }

            return books;
        }

        public async Task<IEnumerable<Book>> GetByAuthorAsync(int id)
        {
            return await context.Books.Where(x => x.BookAuthors.Where(y => y.AuthorId == id).Any()).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBySeriesAsync(int id)
        {
            return await context.Books.Where(x => x.BookSeries.Where(y => y.SeriesId == id).Any()).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByPublisherAsync(int id)
        {
            return await context.Books.Where(x => x.BookPublishers.Where(y => y.PublisherId == id).Any()).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByTagAsync(int id)
        {
            return await context.Books.Where(x => x.BookTags.Where(y => y.TagId == id).Any()).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByRatingAsync(int id)
        {
            if (id == -1)
                return await context.Books.Where(x => x.BookRatings.Count() == 0).ToListAsync();

            return await context.Books.Where(x => x.BookRatings.Where(y => y.RatingId == id).Any()).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByLanguageAsync(int id)
        {
            return await context.Books.Where(x => x.BookLanguages.Where(y => y.LanguageId == id).Any()).ToListAsync();
        }

        public async Task<List<Identifier>> GetIdentifiersAsync()
        {
            return await context.Identifiers.ToListAsync();
        }

        public async Task<List<BookCardModel>> GetBookCardModels(IEnumerable<Book> books)
        {
            List<BookCardModel> model = new List<BookCardModel>();
            foreach(Book book in books)
            {
                var bcModel = await GetBookCardModel(book);
                model.Add(bcModel);
            }

            return model;
        }

        public async Task<BookCardModel> GetBookCardModel(Book book)
        {
            BookFileManager manager = new BookFileManager(_env, _contextAccessor.HttpContext.Request); 
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            List<Identifier> identifiers = await this.GetIdentifiersAsync();
            var cover = await manager.GetBookCoverAsync(book);
            var bcModel = new BookCardModel()
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
                Archived = (user != null) ? user.ArchivedBooks.Where(x => x.BookId == book.Id && x.IsArchived).Any() : false,
                Read = (user != null) ? user.ReadBooks.Where(x => x.BookId == book.Id && x.ReadStatus == 1).Any() : false,
                Description = book.Comment?.Text ?? "",
            };

            return bcModel;
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
