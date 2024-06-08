using CalibreLib.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalibreLib.Data;

public class CalibreLibContext : IdentityDbContext<ApplicationUser>
{
    public virtual DbSet<ArchivedBook> ArchivedBooks { get; set; }

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

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasMany(x => x.ArchivedBooks).WithOne().HasForeignKey(x => x.UserId);
            entity.HasMany(x => x.ReadBooks).WithOne().HasForeignKey(x => x.UserId);
        });

        builder.Entity<ArchivedBook>(entity =>
        {
            entity.ToTable("ArchivedBooks");

            entity.HasIndex(e => e.BookId, "IX_ArchivedBooks_BookID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnName("book_id");
            entity.Property(e => e.UserId)
                .UseCollation("NOCASE")
                .HasColumnName("user_id");
            entity.Property(e => e.IsArchived)
                .HasColumnType("BOOLEAN")
                .HasColumnName("is_archived");
            entity.Property(e => e.LastModified)
                .HasColumnType("DATETIME")
                .HasColumnName("last_modified");
        });

        builder.Entity<ReadBook>(entity =>
        {
            entity.ToTable("ReadBooks");

            entity.HasIndex(e => e.BookId, "IX_ReadBooks_BookID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookId)
                .HasColumnName("book_id");
            entity.Property(e => e.UserId)
                .UseCollation("NOCASE")
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
        });
    }
}
