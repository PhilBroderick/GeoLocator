using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Core.Exceptions;
using GeoLocator.Core.Interfaces;
using GeoLocator.Core.Services;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace GeoLocator.UnitTests;
public class LocatorServiceTests
{
    private readonly Mock<IIpLocationLookupService> _ipLocationLookupServiceMock;
    private readonly Mock<ILocationRepository> _locationRepositoryMock;
    private readonly Mock<IIpValidator> _ipValidatorMock;
    private readonly LocatorService _locatorService;

    public LocatorServiceTests()
    {
        _ipLocationLookupServiceMock = new Mock<IIpLocationLookupService>();
        _locationRepositoryMock = new Mock<ILocationRepository>();
        _ipValidatorMock = new Mock<IIpValidator>();
        var appLogger = Mock.Of<IAppLogger<LocatorService>>();

        _locatorService = new LocatorService(
            _locationRepositoryMock.Object, 
            _ipLocationLookupServiceMock.Object,
            _ipValidatorMock.Object,
            appLogger);
    }

    [Fact]
    public async Task GetLocationByIp_InvalidIp_ThrowsLocationNotFoundException()
    {
        // Arrange
        var ipAddress = "invalid ip";

        // Act/Assert
        await Assert.ThrowsAsync<LocationNotFoundException>(() => _locatorService.GetLocationByIp(ipAddress));
    }

    [Fact]
    public async Task GetLocationByIp_ValidIp_ChecksRepositoryForIpAddress()
    {
        // Arrange
        var ipAddress = "valid ip";
        _ipValidatorMock.Setup(x => x.ValidateIp(ipAddress)).Returns(true);
        _ipLocationLookupServiceMock.Setup(x => x.GetLocationFromIp(ipAddress)).ReturnsAsync(new Location());

        // Act
        await _locatorService.GetLocationByIp(ipAddress);

        // Assert
        _locationRepositoryMock.Verify(x => x.GetByIpAddress(ipAddress), Times.Once);
    }

    [Fact]
    public async Task GetLocationByIp_IpAddressExistsInRepository_DoesNotCallIpLocationLookupService()
    {
        // Arrange
        _locationRepositoryMock.Setup(x => x.GetByIpAddress(It.IsAny<string>())).ReturnsAsync(new Location());

        var ipAddress = "valid ip";
        _ipValidatorMock.Setup(x => x.ValidateIp(ipAddress)).Returns(true);

        // Act
        await _locatorService.GetLocationByIp(ipAddress);

        // Assert
        _ipLocationLookupServiceMock.Verify(x => x.GetLocationFromIp(ipAddress), Times.Never);
    }

    [Fact]
    public async Task GetLocationByIp_IpAddressNotInRepository_CallsIpLocationLookupService()
    {
        // Arrange
        var ipAddress = "valid ip";
        _ipValidatorMock.Setup(x => x.ValidateIp(ipAddress)).Returns(true);

        _ipLocationLookupServiceMock.Setup(x => x.GetLocationFromIp(ipAddress)).ReturnsAsync(new Location());

        // Act
        await _locatorService.GetLocationByIp(ipAddress);


        // Assert
        _ipLocationLookupServiceMock.Verify(x => x.GetLocationFromIp(ipAddress), Times.Once);
    }

    [Fact]
    public async Task GetLocationByIp_RetrievesLocationFromIpLocationLookupService_AddsLocationToRepository()
    {
        // Arrange
        var ipAddress = "valid ip";
        _ipValidatorMock.Setup(x => x.ValidateIp(ipAddress)).Returns(true);

        _ipLocationLookupServiceMock.Setup(x => x.GetLocationFromIp(ipAddress)).ReturnsAsync(new Location());

        // Act
        await _locatorService.GetLocationByIp(ipAddress);


        // Assert
        _locationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Location>()), Times.Once);
    }

    [Fact]
    public async Task GetLocationByIp_CannotDetermineLocation_ThrowsLocationNotFoundException()
    {
        // Arrange
        var ipAddress = "location from ip not determined";
        _ipValidatorMock.Setup(x => x.ValidateIp(ipAddress)).Returns(true);

        // Act/Assert
        await Assert.ThrowsAsync<LocationNotFoundException>(() => _locatorService.GetLocationByIp(ipAddress));
    }
}
