using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Core.Interfaces;

namespace GeoLocator.Infrastructure.Caching;

/// <summary>
/// Decorates the LocationRepository to provide caching - means caching can be removed/added quite easily
/// </summary>
public class CachedLocationRepositoryDecorator : ILocationRepository
{
    private readonly ILocationRepository _locationRepository;
    private readonly ICacheService _cacheService;
    private readonly IAppLogger<CachedLocationRepositoryDecorator> _logger;

    public CachedLocationRepositoryDecorator(ILocationRepository repository, 
        ICacheService cacheService, 
        IAppLogger<CachedLocationRepositoryDecorator> logger)
    {
        _locationRepository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Location> AddAsync(Location location)
    {
        await _locationRepository.AddAsync(location);

        AddToCache(location);

        return location;
    }

    private void AddToCache(Location location)
    {
        var ipAddresses = location.IpAddresses.Select(i => i.Ip);

        foreach (var ipAddress in ipAddresses)
        {
            _cacheService.Set(ipAddress, location);
        }
    }

    public async Task<Location?> GetByIpAddress(string ipAddress)
    {
        var isCached = _cacheService.TryGet<Location>(ipAddress, out var locationFromCache);
        if (isCached)
        {
            _logger.LogInformation("{IpAddress} found in cache");
            return locationFromCache;
        }

        var locationFromRepository = await _locationRepository.GetByIpAddress(ipAddress);

        if (locationFromRepository is not null)
        {
            _logger.LogInformation("{IpAddress} found in persistence layer");
            _cacheService.Set(ipAddress, locationFromRepository);
        }

        return null;
    }
}
