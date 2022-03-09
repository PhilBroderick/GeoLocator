using GeoLocator.Core.Entities;
using GeoLocator.Core.Entities.LocationAggregate;

namespace GeoLocator.Core.Interfaces;

public interface ILocatorService
{
    Task<Location> GetLocationByIp(string ipAddress);
}

