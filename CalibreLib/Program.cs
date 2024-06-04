using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using CalibreLib.Models.Metadata;
using CalibreLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.UI;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CalibreLibContextConnection") ?? throw new InvalidOperationException("Connection string 'CalibreLibContextConnection' not found.");
var metaDataConnectionString = builder.Configuration.GetConnectionString("CalibreMetadataContextConnection") ?? throw new InvalidOperationException("Connection string 'CalibreMetadataContextConnection' not found.");

builder.Services.AddDbContext<CalibreLibContext>(options => options.UseSqlite(connectionString));
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
