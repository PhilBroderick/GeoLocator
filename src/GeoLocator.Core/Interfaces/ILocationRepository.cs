using GeoLocator.Core.Entities.LocationAggregate;

namespace GeoLocator.Core.Interfaces;
public interface ILocationRepository : IRepository<Location>
{
    Task<Location> GetByIpAddress(string ipAddress);
}
