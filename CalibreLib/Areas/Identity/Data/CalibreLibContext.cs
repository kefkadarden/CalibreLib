using CalibreLib.Areas.Identity.Data;
using CalibreLib.Models.MailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalibreLib.Data;

public class CalibreLibContext : IdentityDbContext<ApplicationUser>
{
    public virtual DbSet<ArchivedBook> ArchivedBooks { get; set; }
    public virtual DbSet<MailSettings> MailSettings { get; set; }

    public CalibreLibContext(DbContextOptions<CalibreLibContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations
        // calling base.OnModelCreating(builder);

        builder.Entity<ArchivedBook>(entity =>
        {
            entity.ToTable("ArchivedBooks",(string)null);

            entity.HasIndex(e => e.BookId, "IX_ArchivedBooks_BookID");
            entity.HasIndex(e => new { e.BookId, e.UserId }, "IX_ArchivedBooks_bookid_userid").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnName("book_id");
            entity.Property(e => e.UserId)
                .HasColumnName("user_id");
            entity.Property(e => e.IsArchived)
                .HasColumnType("BOOLEAN")
                .HasColumnName("is_archived");
            entity.Property(e => e.LastModified)            
                .HasColumnType("DATETIME")
                .HasColumnName("last_modified");
            entity.HasOne(x => x.User).WithMany(x => x.ArchivedBooks).HasForeignKey(x => x.UserId).IsRequired();
        });

        builder.Entity<ReadBook>(entity =>
        {
            entity.ToTable("ReadBooks", (string)null);

            entity.HasIndex(e => e.BookId, "IX_ReadBooks_BookID");
            entity.HasIndex(e => new { e.BookId, e.UserId }, "IX_ReadBooks_bookid_userid").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnName("book_id");
            entity.Property(e => e.UserId)
                .HasColumnName("user_id");
            entity.Property(e => e.ReadStatus)
                .HasColumnName("read_status");
            entity.Property(e => e.LastModified)
                .HasColumnType("DATETIME")
                .HasColumnName("last_modified");
            entity.Property(e => e.LastTimeStarted)
                .HasColumnType("DATETIME")
                .HasColumnName("last_time_started_reading");
            entity.Property(e => e.TimesReading)
                .HasColumnName("times_started_reading");
            entity.HasOne(x => x.User).WithMany(x => x.ReadBooks).HasForeignKey(x => x.UserId).IsRequired();
        });

        builder.Entity<Shelf>(entity =>
        {
            entity.ToTable("Shelf", (string)null);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.UUId)
                .ValueGeneratedOnAdd()
                .HasColumnName("uuid");
            entity.Property(e => e.UserId)
                .HasColumnName("user_id");
            entity.Property(e => e.Name)
                .HasColumnName("name");
            entity.Property(e => e.Created)
                .ValueGeneratedOnAdd()
                .HasColumnName("created");
            entity.Property(e => e.LastModified)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("last_modified");

            entity.HasOne(x =>x.User).WithMany(x => x.Shelves).HasForeignKey(x => x.UserId).IsRequired();
        });

        builder.Entity<BooksShelvesLink>(entity =>
        {
            entity.ToTable("BooksShelvesLink", (string)null);

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");
            entity.Property(e => e.BookId)
            .HasColumnName("book_id");
            entity.Property(e => e.Order)
            .HasColumnName("order");
            entity.Property(e => e.ShelfId)
            .HasColumnName("shelf");
            entity.Property(e => e.DateAdded)
            .ValueGeneratedOnAdd()
            .HasColumnName("date_added");

            entity.HasOne(e => e.Shelf).WithMany(e => e.BookShelves).HasForeignKey(e => e.ShelfId);
        });

        builder.Entity<MailSettings>(entity =>
        {
            entity.ToTable("Settings", (string)null);

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");
            entity.Property(e => e.SMTP_HostName)
            .HasColumnName("smtp_hostname");
            entity.Property(e => e.SMTP_Port)
            .HasColumnName("smtp_port");
            entity.Property(e => e.SMTP_Encryption)
            .HasColumnName("smtp_encryption");
            entity.Property(e => e.SMTP_UserName)
            .HasColumnName("smtp_username");
            entity.Property(e => e.SMTP_Password)
            .HasColumnName("smtp_password");
            entity.Property(e => e.FromEmail)
            .HasColumnName("from_email");
            entity.Property(e => e.SMTP_AttachmentSize)
            .HasColumnName("smtp_attachmentsize");
        });
    }
}
