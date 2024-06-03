using CalibreLib.Areas.Identity.Data;
using CalibreLib.Data;
using CalibreLib.Models.Metadata;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
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
