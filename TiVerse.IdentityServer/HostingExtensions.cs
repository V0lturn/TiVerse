using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.IndentityDbContext;
namespace TiVerseIdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var migrationAssembly = typeof(Program).Assembly.GetName().Name;

        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

        builder.Services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
            options.EmitStaticAudienceClaim = true;
        })
        .AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseSqlServer(
           connectionString, opt => opt.MigrationsAssembly(migrationAssembly))).
        AddOperationalStore(options => options.ConfigureDbContext = b => b.UseSqlServer(
           connectionString, opt => opt.MigrationsAssembly(migrationAssembly)))
        .AddAspNetIdentity<ApplicationUser>();

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseAuthorization();
        app.MapRazorPages();
        return app;
    }
}