using GeoLocator.Core.Entities.LocationAggregate;
using Microsoft.EntityFrameworkCore;

namespace GeoLocator.Infrastructure.Data;
public class GeoLocatorDbContext : DbContext
{
    public GeoLocatorDbContext(DbContextOptions<GeoLocatorDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Location> Locations { get; set; }
}
