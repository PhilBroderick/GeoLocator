using GeoLocator.Core.Entities;
using GeoLocator.Core.Entities.LocationAggregate;

namespace GeoLocator.Core.Interfaces;
public interface IIpLocationLookupService
{
    Task<Location> GetLocationFromIp(string ip);
}
