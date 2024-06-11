using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using CalibreLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web.UI;


var builder = WebApplication.CreateBuilder(args);
var bookdirectory = builder.Configuration.GetValue<string>("BookDirectory");
var dbdirectory = builder.Configuration.GetValue<string>("DBDirectory");
var connectionString = builder.Configuration.GetConnectionString("CalibreLibContextConnection") ?? throw new InvalidOperationException("Connection string 'CalibreLibContextConnection' not found.");
var metaDataConnectionString = builder.Configuration.GetConnectionString("CalibreMetadataContextConnection") ?? throw new InvalidOperationException("Connection string 'CalibreMetadataContextConnection' not found.");


builder.Services.AddDbContext<CalibreLibContext>(options => options.UseLazyLoadingProxies().UseSqlite(connectionString));
builder.Services.AddDbContext<MetadataDBContext>(options => options.UseLazyLoadingProxies().UseSqlite(metaDataConnectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<CalibreLibContext>();

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

// Add services to the container.
builder.Services.AddAuthentication()
    .AddMicrosoftAccount(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Scope.Add("User.Read");
    });

builder.Logging.ClearProviders();
builder.Logging.AddEventLog(x => { x.SourceName = "CalibreLib"; x.LogName = "Application"; });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddControllersWithViews();
//options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddScoped<BookRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseExceptionHandler(_ => { });
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(bookdirectory),
    RequestPath = "/books"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(bookdirectory),
    RequestPath = "/db"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "book",
    pattern: "book/{id?}",
    defaults: new { controller="Home", action = "Book" });

app.MapControllerRoute(
    name: "author",
    pattern: "author/{id?}",
    defaults: new { controller = "Author", action = "Index" });

app.MapControllerRoute(
    name: "publisher",
    pattern: "publisher/{id?}",
    defaults: new { controller = "Publisher", action = "Index" });

app.MapControllerRoute(
    name: "language",
    pattern: "language/{id?}",
    defaults: new { controller = "Language", action = "Index" });

app.MapControllerRoute(
    name: "rating",
    pattern: "rating/{id?}",
    defaults: new { controller = "Rating", action = "Index" });

app.MapControllerRoute(
    name: "category",
    pattern: "category/{id?}",
    defaults: new { controller = "Category", action = "Index" });

app.MapControllerRoute(
    name: "series",
    pattern: "series/{id?}",
    defaults: new { controller = "Series", action = "Index" });

app.MapRazorPages();

app.Run();
