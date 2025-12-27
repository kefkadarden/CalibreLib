using Microsoft.AspNetCore.Identity;

namespace CalibreLib.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? EReaderEmail { get; set; } = null!;

    public virtual List<ArchivedBook> ArchivedBooks { get; set; } = [];
    public virtual List<ReadBook> ReadBooks { get; set; } = [];

    public virtual List<Shelf> Shelves { get; set; } = [];
}