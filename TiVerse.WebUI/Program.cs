using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TiVerse.Application.Data;
using TiVerse.Application.Interfaces.IAccountServiceInterface;
using TiVerse.Application.Interfaces.IRepositoryInterface;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.Application.Services;
using TiVerse.Application.UseCase;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;
using TiVerse.Infrastructure.IndentityDbContext;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using TiVerse.WebUI.CityLocalizer;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<TiVerseDbContext>(options =>
              options.UseSqlServer(connectionString));

builder.Services.AddScoped<ITiVerseDbContext, TiVerseDbContext>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITiVerseIRepository<Account>, TiVerseRepository<Account>>();
builder.Services.AddScoped<ITiVerseIRepository<Trip>, TiVerseRepository<Trip>>();
builder.Services.AddScoped<ITiVerseIRepository<UserRouteHistory>, TiVerseRepository<UserRouteHistory>>();
builder.Services.AddScoped<ICityLocalization, CityLocalization>();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuth", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";

        options.ClientId = "web";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("api1");
        options.Scope.Add("offline_access");
        options.GetClaimsFromUserInfoEndpoint = true;
    });

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";

});

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
                    new CultureInfo("ru"),
                    new CultureInfo("en")
                };

    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=MainPage}/{action=Index}/{id?}");
});

app.Run();


