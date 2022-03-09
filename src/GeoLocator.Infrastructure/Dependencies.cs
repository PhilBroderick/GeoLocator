using GeoLocator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoLocator.Infrastructure;
public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        var useOnlyInMemoryDatabase = false;
        if (configuration["UseOnlyInMemoryDatabase"] != null)
        {
            useOnlyInMemoryDatabase = bool.Parse(configuration["UseOnlyInMemoryDatabase"]);
        }

        if (useOnlyInMemoryDatabase)
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
}
