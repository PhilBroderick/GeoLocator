using GeoLocator.Core.Entities;

namespace GeoLocator.Core.Interfaces;

public interface ILocatorService
{
    Task<Location> GetLocationByIp(string ipAddress);
}

