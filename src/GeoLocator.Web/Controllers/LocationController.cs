using GeoLocator.Core.Interfaces;
using GeoLocator.Web.ApiModels;
using GeoLocator.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocator.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ILocatorService _locatorService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAppLogger<LocationController> _logger;

    public LocationController(ILocatorService locatorService, 
        IHttpContextAccessor httpContextAccessor,
        IAppLogger<LocationController> logger)
    {
        _locatorService = locatorService;
        _contextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// GET: api/location?ipAddress={ipAddress}
    /// // Determines the location information for the IpAddress provided, or for the caller if no IP provided
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(VaryByQueryKeys = new[] { "ipAddress" }, Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> GetLocationByIpAddress([FromQuery]string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            _logger.LogInformation($"IpAddress null, attempting to find location for caller");
            var clientIpAddress = _contextAccessor?.HttpContext?.Connection.RemoteIpAddress;

            var isLocalConnection = clientIpAddress.IsInternal();

            if (isLocalConnection)
            {
                _logger.LogWarning("Cannot determine location for {IpAddress} as it is a local connection", ipAddress);
                return BadRequest("IpAddress not available when running locally");
            }

            ipAddress = clientIpAddress.ToString();
        }

        var location = await _locatorService.GetLocationByIp(ipAddress);

        var result = new LocationDTO(location.Country, location.CountryCode, location.City);

        return Ok(result);
    }
}
