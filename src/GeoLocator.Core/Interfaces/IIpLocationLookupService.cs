using GeoLocator.Core.Entities;

namespace GeoLocator.Core.Interfaces;
public interface IIpLocationLookupService
{
    Task<Location> GetLocationFromIp(string ip);
}
