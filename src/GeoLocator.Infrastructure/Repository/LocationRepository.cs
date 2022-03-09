using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Core.Interfaces;
using GeoLocator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeoLocator.Infrastructure.Repository;
public class LocationRepository : RepositoryBase<Location>, ILocationRepository
{
    private readonly GeoLocatorDbContext _dbContext;
    public LocationRepository(GeoLocatorDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Location> GetByIpAddress(string ipAddress)
    {
        var location = await _dbContext.Set<Location>().FirstOrDefaultAsync(l => 
            l.IpAddresses.Any(ipAddr => ipAddr.Ip == ipAddress));

        return location;
    }
}
