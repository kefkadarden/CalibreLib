using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CalibreLib.Models.Metadata;

public partial class MetadataDBContext : DbContext
{
    public MetadataDBContext()
    {
    }

    public MetadataDBContext(DbContextOptions<MetadataDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    //public virtual DbSet<BooksAuthorsLink> BooksAuthorsLinks { get; set; }

    //public virtual DbSet<BooksLanguagesLink> BooksLanguagesLinks { get; set; }

    public virtual DbSet<BooksPluginDatum> BooksPluginData { get; set; }

    //public virtual DbSet<BooksPublishersLink> BooksPublishersLinks { get; set; }

    //public virtual DbSet<BooksRatingsLink> BooksRatingsLinks { get; set; }

    //public virtual DbSet<BooksSeriesLink> BooksSeriesLinks { get; set; }

    //public virtual DbSet<BooksTagsLink> BooksTagsLinks { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<ConversionOption> ConversionOptions { get; set; }

    public virtual DbSet<CustomColumn> CustomColumns { get; set; }

    public virtual DbSet<Datum> Data { get; set; }

    public virtual DbSet<Feed> Feeds { get; set; }

    public virtual DbSet<Identifier> Identifiers { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<LastReadPosition> LastReadPositions { get; set; }

    public virtual DbSet<LibraryId> LibraryIds { get; set; }

    public virtual DbSet<MetadataDirtied> MetadataDirtieds { get; set; }

    public virtual DbSet<Preference> Preferences { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Name=CalibreMetadataContextConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");

            entity.HasIndex(e => e.Name, "IX_authors_name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasDefaultValueSql("\"\"")
                .HasColumnName("link");
            entity.Property(e => e.Name)
                .UseCollation("NOCASE")
                .HasColumnName("name");
            entity.Property(e => e.Sort)
                .UseCollation("NOCASE")
                .HasColumnName("sort");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books");

            entity.HasIndex(e => e.AuthorSort, "authors_idx");

            entity.HasIndex(e => e.Sort, "books_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorSort)
                .UseCollation("NOCASE")
                .HasColumnName("author_sort");
            entity.Property(e => e.Flags)
                .HasDefaultValue(1)
                .HasColumnName("flags");
            entity.Property(e => e.HasCover)
                .HasDefaultValue(false)
                .HasColumnType("BOOL")
                .HasColumnName("has_cover");
            entity.Property(e => e.Isbn)
                .HasDefaultValueSql("\"\"")
                .UseCollation("NOCASE")
                .HasColumnName("isbn");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("\"2000-01-01 00:00:00+00:00\"")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.Lccn)
                .HasDefaultValueSql("\"\"")
                .UseCollation("NOCASE")
                .HasColumnName("lccn");
            entity.Property(e => e.Path)
                .HasDefaultValueSql("\"\"")
                .HasColumnName("path");
            entity.Property(e => e.Pubdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("pubdate");
            entity.Property(e => e.SeriesIndex)
                .HasDefaultValue(1.0)
                .HasColumnName("series_index");
            entity.Property(e => e.Sort)
                .UseCollation("NOCASE")
                .HasColumnName("sort");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("timestamp");
            entity.Property(e => e.Title)
                .HasDefaultValue("Unknown")
                .UseCollation("NOCASE")
                .HasColumnName("title");
            entity.Property(e => e.Uuid).HasColumnName("uuid");
        });

        modelBuilder.Entity<BooksAuthorsLink>(entity =>
        {
            entity.ToTable("books_authors_link");

            entity.HasIndex(e => new { e.BookId, e.AuthorId }, "IX_books_authors_link_book_author").IsUnique();

            entity.HasIndex(e => e.AuthorId, "books_authors_link_aidx");

            entity.HasIndex(e => e.BookId, "books_authors_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("author");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.HasKey(e => new { e.BookId, e.AuthorId });
            entity.HasOne(e => e.Book).WithMany(e => e.BookAuthors).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Author).WithMany(e => e.BookAuthors).HasForeignKey(e => e.AuthorId);
        });

        modelBuilder.Entity<BooksLanguagesLink>(entity =>
        {
            entity.ToTable("books_languages_link");

            entity.HasIndex(e => new { e.BookId, e.LanguageId }, "IX_books_languages_link_book_lang_code").IsUnique();

            entity.HasIndex(e => e.LanguageId, "books_languages_link_aidx");

            entity.HasIndex(e => e.BookId, "books_languages_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.Property(e => e.ItemOrder).HasColumnName("item_order");
            entity.Property(e => e.LanguageId).HasColumnName("lang_code");
            entity.HasKey(e => new { e.BookId, e.LanguageId });
            entity.HasOne(e => e.Book).WithMany(e => e.BookLanguages).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Language).WithMany(e => e.BookLanguages).HasForeignKey(e => e.LanguageId);
        });

        modelBuilder.Entity<BooksPluginDatum>(entity =>
        {
            entity.ToTable("books_plugin_data");

            entity.HasIndex(e => new { e.BookId, e.Name }, "IX_books_plugin_data_book_name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnType("INTEGER NON")
                .HasColumnName("book");
            entity.Property(e => e.Name)
                .HasColumnType("TEXT NON")
                .HasColumnName("name");
            entity.Property(e => e.Val)
                .HasColumnType("TEXT NON")
                .HasColumnName("val");
            entity.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<BooksPublishersLink>(entity =>
        {
            entity.ToTable("books_publishers_link");

            entity.HasIndex(e => e.BookId, "IX_books_publishers_link_book").IsUnique();

            entity.HasIndex(e => e.PublisherId, "books_publishers_link_aidx");

            entity.HasIndex(e => e.BookId, "books_publishers_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.Property(e => e.PublisherId).HasColumnName("publisher");
            entity.HasKey(e => new { e.BookId, e.PublisherId });
            entity.HasOne(e => e.Book).WithMany(e => e.BookPublishers).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Publisher).WithMany(e => e.BookPublishers).HasForeignKey(e => e.PublisherId);
        });

        modelBuilder.Entity<BooksRatingsLink>(entity =>
        {
            entity.ToTable("books_ratings_link");

            entity.HasIndex(e => new { e.BookId, e.RatingId }, "IX_books_ratings_link_book_rating").IsUnique();

            entity.HasIndex(e => e.RatingId, "books_ratings_link_aidx");

            entity.HasIndex(e => e.BookId, "books_ratings_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.Property(e => e.RatingId).HasColumnName("rating");
            entity.HasKey(e => new { e.BookId, e.RatingId });
            entity.HasOne(e => e.Book).WithMany(e => e.BookRatings).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Rating).WithMany(e => e.BookRatings).HasForeignKey(e => e.RatingId);
        });

        modelBuilder.Entity<BooksSeriesLink>(entity =>
        {
            entity.ToTable("books_series_link");

            entity.HasIndex(e => e.BookId, "IX_books_series_link_book").IsUnique();

            entity.HasIndex(e => e.SeriesId, "books_series_link_aidx");

            entity.HasIndex(e => e.BookId, "books_series_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.Property(e => e.SeriesId).HasColumnName("series");
            entity.HasKey(e => new { e.BookId, e.SeriesId });
            entity.HasOne(e => e.Book).WithMany(e => e.BookSeries).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Series).WithMany(e => e.BookSeries).HasForeignKey(e => e.SeriesId);
        });

        modelBuilder.Entity<BooksTagsLink>(entity =>
        {
            entity.ToTable("books_tags_link");

            entity.HasIndex(e => new { e.BookId, e.TagId }, "IX_books_tags_link_book_tag").IsUnique();

            entity.HasIndex(e => e.TagId, "books_tags_link_aidx");

            entity.HasIndex(e => e.BookId, "books_tags_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.Property(e => e.TagId).HasColumnName("tag");
            entity.HasKey(e => new { e.BookId, e.TagId });
            entity.HasOne(e => e.Book).WithMany(e => e.BookTags).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Tag).WithMany(e => e.BookTags).HasForeignKey(e => e.TagId);

        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");

            entity.HasIndex(e => e.BookId, "IX_comments_book").IsUnique();

            entity.HasIndex(e => e.BookId, "comments_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnType("INTEGER NON")
                .HasColumnName("book");
            entity.Property(e => e.Text)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT NON")
                .HasColumnName("text");
            entity.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<ConversionOption>(entity =>
        {
            entity.ToTable("conversion_options");

            entity.HasIndex(e => new { e.Format, e.BookId }, "IX_conversion_options_format_book").IsUnique();

            entity.HasIndex(e => e.Format, "conversion_options_idx_a");

            entity.HasIndex(e => e.BookId, "conversion_options_idx_b");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Format)
                .UseCollation("NOCASE")
                .HasColumnName("format");
            entity.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<CustomColumn>(entity =>
        {
            entity.ToTable("custom_columns");

            entity.HasIndex(e => e.Label, "IX_custom_columns_label").IsUnique();

            entity.HasIndex(e => e.Label, "custom_columns_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datatype).HasColumnName("datatype");
            entity.Property(e => e.Display)
                .HasDefaultValueSql("\"{}\"")
                .HasColumnName("display");
            entity.Property(e => e.Editable)
                .HasDefaultValue(true)
                .HasColumnType("BOOL")
                .HasColumnName("editable");
            entity.Property(e => e.IsMultiple)
                .HasColumnType("BOOL")
                .HasColumnName("is_multiple");
            entity.Property(e => e.Label).HasColumnName("label");
            entity.Property(e => e.MarkForDelete)
                .HasColumnType("BOOL")
                .HasColumnName("mark_for_delete");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Normalized)
                .HasColumnType("BOOL")
                .HasColumnName("normalized");
        });

        modelBuilder.Entity<Datum>(entity =>
        {
            entity.ToTable("data");

            entity.HasIndex(e => new { e.BookId, e.Format }, "IX_data_book_format").IsUnique();

            entity.HasIndex(e => e.BookId, "data_idx");

            entity.HasIndex(e => e.Format, "formats_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnType("INTEGER NON")
                .HasColumnName("book");
            entity.Property(e => e.Format)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT NON")
                .HasColumnName("format");
            entity.Property(e => e.Name)
                .HasColumnType("TEXT NON")
                .HasColumnName("name");
            entity.Property(e => e.UncompressedSize)
                .HasColumnType("INTEGER NON")
                .HasColumnName("uncompressed_size");
            entity.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<Feed>(entity =>
        {
            entity.ToTable("feeds");

            entity.HasIndex(e => e.Title, "IX_feeds_title").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Script).HasColumnName("script");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Identifier>(entity =>
        {
            entity.ToTable("identifiers");

            entity.HasIndex(e => new { e.BookId, e.Type }, "IX_identifiers_book_type").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnType("INTEGER NON")
                .HasColumnName("book");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("\"isbn\"")
                .UseCollation("NOCASE")
                .HasColumnType("TEXT NON")
                .HasColumnName("type");
            entity.Property(e => e.Val)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT NON")
                .HasColumnName("val");
            entity.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("languages");

            entity.HasIndex(e => e.LangCode, "IX_languages_lang_code").IsUnique();

            entity.HasIndex(e => e.LangCode, "languages_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.LangCode)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT NON")
                .HasColumnName("lang_code");
        });

        modelBuilder.Entity<LastReadPosition>(entity =>
        {
            entity.ToTable("last_read_positions");

            entity.HasIndex(e => new { e.User, e.Device, e.BookId, e.Format }, "IX_last_read_positions_user_device_book_format").IsUnique();

            entity.HasIndex(e => e.BookId, "lrp_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.Property(e => e.Cfi).HasColumnName("cfi");
            entity.Property(e => e.Device).HasColumnName("device");
            entity.Property(e => e.Epoch).HasColumnName("epoch");
            entity.Property(e => e.Format)
                .UseCollation("NOCASE")
                .HasColumnName("format");
            entity.Property(e => e.PosFrac).HasColumnName("pos_frac");
            entity.Property(e => e.User).HasColumnName("user");
            entity.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<LibraryId>(entity =>
        {
            entity.ToTable("library_id");

            entity.HasIndex(e => e.Uuid, "IX_library_id_uuid").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Uuid).HasColumnName("uuid");
        });

        modelBuilder.Entity<MetadataDirtied>(entity =>
        {
            entity.ToTable("metadata_dirtied");

            entity.HasIndex(e => e.BookId, "IX_metadata_dirtied_book").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book");
            entity.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity.ToTable("preferences");

            entity.HasIndex(e => e.Key, "IX_preferences_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Key)
                .HasColumnType("TEXT NON")
                .HasColumnName("key");
            entity.Property(e => e.Val)
                .HasColumnType("TEXT NON")
                .HasColumnName("val");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("publishers");

            entity.HasIndex(e => e.Name, "IX_publishers_name").IsUnique();

            entity.HasIndex(e => e.Name, "publishers_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .UseCollation("NOCASE")
                .HasColumnName("name");
            entity.Property(e => e.Sort)
                .UseCollation("NOCASE")
                .HasColumnName("sort");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.ToTable("ratings");

            entity.HasIndex(e => e.Rating1, "IX_ratings_rating").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Rating1).HasColumnName("rating");
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.ToTable("series");

            entity.HasIndex(e => e.Name, "IX_series_name").IsUnique();

            entity.HasIndex(e => e.Name, "series_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .UseCollation("NOCASE")
                .HasColumnName("name");
            entity.Property(e => e.Sort)
                .UseCollation("NOCASE")
                .HasColumnName("sort");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");

            entity.HasIndex(e => e.Name, "IX_tags_name").IsUnique();

            entity.HasIndex(e => e.Name, "tags_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .UseCollation("NOCASE")
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
