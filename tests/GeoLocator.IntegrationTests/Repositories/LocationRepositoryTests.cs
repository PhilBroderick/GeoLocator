using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Infrastructure.Data;
using GeoLocator.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace GeoLocator.IntegrationTests.Repositories;
public class LocationRepositoryTests
{
    private readonly GeoLocatorDbContext _dbContext;
    private readonly LocationRepository _locationRepository;

    public LocationRepositoryTests()
    {
        var dbOptions = new DbContextOptionsBuilder<GeoLocatorDbContext>()
            .UseInMemoryDatabase("GeoLocatorIntegrationTests")
            .Options;

        _dbContext = new GeoLocatorDbContext(dbOptions);
        _locationRepository = new LocationRepository(_dbContext);
    }

    [Fact]
    public async Task AddAsync_ValidLocation_AddsLocationAndSetsId()
    {
        // Arrange
        var location = new Location();

        // Act
        var result = await _locationRepository.AddAsync(location);

        // Assert
        Assert.True(result.Id >= 1);
    }

    [Fact]
    public async Task GetByIpAddress_LocationExistsForIp_ReturnsLocationWithIp()
    {
        // Arrange
        var validIpAddress = new IpAddress { Ip = "valid ip" };
        var existingLocation = new Location();
        existingLocation.AddIpAddress(validIpAddress);
        await _locationRepository.AddAsync(existingLocation);

        // Act
        var location = await _locationRepository.GetByIpAddress(validIpAddress.Ip);

        // Assert
        Assert.Contains(validIpAddress, location.IpAddresses);
    }

    [Fact]
    public async Task GetByIpAddress_NoLocationExistsForIp_ReturnsNull()
    {
        // Arrange
        var validIpAddress = new IpAddress { Ip = "another valid ip" };

        // Act
        var location = await _locationRepository.GetByIpAddress(validIpAddress.Ip);

        // Assert
        Assert.Null(location);

    }
}
