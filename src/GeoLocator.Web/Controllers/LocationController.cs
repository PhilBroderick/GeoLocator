using GeoLocator.Core.Interfaces;
using GeoLocator.Web.ApiModels;
using GeoLocator.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GeoLocator.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ILocatorService _locatorService;
    private readonly IHttpContextAccessor _contextAccessor;

    public LocationController(ILocatorService locatorService, IHttpContextAccessor httpContextAccessor)
    {
        _locatorService = locatorService;
        _contextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// GET: api/location/{ip}
    /// // Determines the location information for the IpAddress provided
    /// </summary>
    [HttpGet("{ip}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLocationByIpAddress(string ipAddress)
    {
        var location = await _locatorService.GetLocationByIp(ipAddress);

        var result = new LocationDTO(location.Country, location.CountryCode, location.City);

        return Ok(result);
    }

    /// <summary>
    /// GET: api/location
    /// Used to determine location of caller of API without providing IP - only available when not running locally
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Core.Entities.Location))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLocationForClient()
    {
        var clientIpAddress = _contextAccessor?.HttpContext?.Connection.RemoteIpAddress;

        var isLocalConnection = clientIpAddress.IsLocal();

        if (isLocalConnection)
        {
            // TODO: log
            return BadRequest("IpAddress not available when running locally");
        }

        var location = await _locatorService.GetLocationByIp(clientIpAddress.ToString());

        var result = new LocationDTO(location.Country, location.CountryCode, location.City);

        return Ok(result);
    }
}
