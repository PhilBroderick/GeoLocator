using GeoLocator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoLocator.Infrastructure;
public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        if (UseInMemoryDatabase(configuration))
        {
            services.AddDbContext<GeoLocatorDbContext>(c =>
               c.UseInMemoryDatabase("GeoLocator"));
        }
        else
        {
            // use real database
            // Requires DB to be running locally (Docker preferable)
            services.AddDbContext<GeoLocatorDbContext>(c =>
                c.UseSqlServer(configuration.GetConnectionString("GeoLocatorConnection")));
        }
    }

    public static async Task EnsureDatabaseCreated(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        if (!UseInMemoryDatabase(configuration))
        {
            var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();

            await using var geoLocatorDbContext = scope.ServiceProvider.GetRequiredService<GeoLocatorDbContext>();
            await geoLocatorDbContext.Database.MigrateAsync();
        }
    }

    private static bool UseInMemoryDatabase(IConfiguration configuration) => bool.TryParse(configuration["UseOnlyInMemoryDatabase"], out var useOnlyInMemoryDatabase) && useOnlyInMemoryDatabase;
}
