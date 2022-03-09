using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Core.Interfaces;
using GeoLocator.Infrastructure.Caching;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace GeoLocator.UnitTests;
public class CachedLocationRepositoryDecoratorTests
{
    private readonly Mock<ILocationRepository> _locationRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CachedLocationRepositoryDecorator _cachedLocationRepositoryDecorator;

    public CachedLocationRepositoryDecoratorTests()
    {
        _locationRepositoryMock = new Mock<ILocationRepository>();
        _cacheServiceMock = new Mock<ICacheService>();

        var logger = Mock.Of<IAppLogger<CachedLocationRepositoryDecorator>>();

        _cachedLocationRepositoryDecorator = new CachedLocationRepositoryDecorator(
            _locationRepositoryMock.Object,
            _cacheServiceMock.Object,
            logger);
    }

    [Fact]
    public async Task AddAsync_WhenCalledWithLocation_AddsToRepository()
    {
        // Arrange
        var location = new Location();
    
        // Act
        await _cachedLocationRepositoryDecorator.AddAsync(location);

        // Assert
        _locationRepositoryMock.Verify(x => x.AddAsync(location), Times.Once);
    }

    [Fact]
    public async Task AddAsync_LocationWithIp_AddsToCache()
    {
        // Arrange
        var location = new Location();
        var ipAddress = new IpAddress { Ip = "127.0.0.1" };
        location.AddIpAddress(ipAddress);

        // Act
        await _cachedLocationRepositoryDecorator.AddAsync(location);

        // Assert
        _cacheServiceMock.Verify(x => x.Set(ipAddress.Ip.ToString(), location), Times.Once);
    }

    [Fact]
    public async Task GetByIpAddress_IpExistsInCache_RetrievesFromCache()
    {
        // Arrange
        var expectedLocation = new Location();
        _cacheServiceMock.Setup(x => x.TryGet<Location>(It.IsAny<string>(), out expectedLocation)).Returns(true).Verifiable();

        var ipAddress = new IpAddress { Ip = "127.0.0.1" };

        // Act
        var result = await _cachedLocationRepositoryDecorator.GetByIpAddress(ipAddress.Ip);

        // Assert
        Assert.Equal(expectedLocation, result);
        _cacheServiceMock.Verify();
    }

    [Fact]
    public async Task GetByIpAddress_IpNotInCache_RetrievesFromRepository()
    {
        // Arrange
        var ipAddress = new IpAddress { Ip = "127.0.0.1" };

        // Act
        var result = await _cachedLocationRepositoryDecorator.GetByIpAddress(ipAddress.Ip);

        // Assert
        _locationRepositoryMock.Verify(x => x.GetByIpAddress(ipAddress.Ip), Times.Once);
    }

    [Fact]
    public async Task GetByIpAddress_IpInRepository_AddsToCache()
    {
        // Arrange
        var existingLocation = new Location();
        var ipAddress = new IpAddress { Ip = "127.0.0.1" };

        _locationRepositoryMock.Setup(x => x.GetByIpAddress(ipAddress.Ip)).ReturnsAsync(existingLocation);

        // Act
        var result = await _cachedLocationRepositoryDecorator.GetByIpAddress(ipAddress.Ip);

        // Assert
        _cacheServiceMock.Verify(x => x.Set(ipAddress.Ip, existingLocation), Times.Once);
    }

    [Fact]
    public async Task GetByIpAddress_IpInRepository_ReturnsLocation()
    {
        // Arrange
        var existingLocation = new Location();
        var ipAddress = new IpAddress { Ip = "127.0.0.1" };

        _locationRepositoryMock.Setup(x => x.GetByIpAddress(ipAddress.Ip)).ReturnsAsync(existingLocation);

        // Act
        var result = await _cachedLocationRepositoryDecorator.GetByIpAddress(ipAddress.Ip);

        // Assert
        Assert.Equal(existingLocation, result);
    }

    [Fact]
    public async Task GetByIpAddress_IpNotInCacheOrRepository_ReturnsNull()
    {
        // Arrange
        var ipAddress = new IpAddress { Ip = "127.0.0.1" };

        // Act
        var result = await _cachedLocationRepositoryDecorator.GetByIpAddress(ipAddress.Ip);

        // Assert
        Assert.Null(result);
    }
}
