using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Core.Exceptions;
using GeoLocator.Core.Interfaces;

namespace GeoLocator.Core.Services;
public class LocatorService : ILocatorService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IIpLocationLookupService _ipLocationLookupService;
    private readonly IIpValidator _ipValidator;
    private readonly IAppLogger<LocatorService> _logger;

    public LocatorService(ILocationRepository locationRepository,
        IIpLocationLookupService ipLocationLookupService,
        IIpValidator ipValidator,
        IAppLogger<LocatorService> logger)
    {
        _locationRepository = locationRepository;
        _ipLocationLookupService = ipLocationLookupService;
        _ipValidator = ipValidator;
        _logger = logger;
    }

    public async Task<Location> GetLocationByIp(string ipAddress)
    {
        try
        {
            if (!_ipValidator.ValidateIp(ipAddress))
            {
                _logger.LogWarning("Unable to validate {IpAddress}", ipAddress);

                throw new ArgumentException("Invalid IpAddress");
            }
            var locationFromRepo = await _locationRepository.GetByIpAddress(ipAddress);

            if (locationFromRepo is not null)
            {
                _logger.LogInformation("Location for {IpAddress} read from repository", ipAddress);

                return locationFromRepo;
            }

            var locationFromService = await _ipLocationLookupService.GetLocationFromIp(ipAddress);

            locationFromService.AddIpAddress(new() { Ip = ipAddress });

            await _locationRepository.AddAsync(locationFromService);

            return locationFromService;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving location from {IpAddress}", ipAddress);
            throw new LocationNotFoundException(ipAddress);
        }
    }
}
