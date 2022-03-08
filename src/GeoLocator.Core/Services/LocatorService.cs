using GeoLocator.Core.Entities;
using GeoLocator.Core.Interfaces;

namespace GeoLocator.Core.Services;
public class LocatorService : ILocatorService
{
    private readonly IIpLocationLookupService _ipLocationLookupService;

    public LocatorService(IIpLocationLookupService ipLocationLookupService)
    {
        _ipLocationLookupService = ipLocationLookupService;
    }

    public async Task<Location> GetLocationByIp(string ipAddress)
    {
        var result = await _ipLocationLookupService.GetLocationFromIp(ipAddress);

        return result;
    }
}
