﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CalibreLib.Models.Metadata;
using Microsoft.AspNetCore.Identity;

namespace CalibreLib.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public List<ArchivedBook> ArchivedBooks { get; set; } = [];
    public List<ReadBook> ReadBooks { get; set; } = [];
}