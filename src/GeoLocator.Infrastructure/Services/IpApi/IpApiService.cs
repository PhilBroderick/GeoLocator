using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Core.Interfaces;
using GeoLocator.Shared.HttpClients;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GeoLocator.Infrastructure.Services.IpApi;

/// <summary>
/// Wraps the ip-api.com API - https://ip-api.com/
/// </summary>
public class IpApiService : IIpLocationLookupService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAppLogger<IpApiService> _logger;

    public IpApiService(IHttpClientFactory httpClientFactory, IAppLogger<IpApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Location> GetLocationFromIp(string ip)
    {
        _logger.LogInformation("Retrieving location information for {IpAddress}", ip);

        var client = _httpClientFactory.CreateClient(NamedHttpClients.IpApiHttpClient);

        var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}");

        try
        {
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var ipApiResponse = JsonConvert.DeserializeObject<IpApiResponse>(json);

                if (IsInvalidResponse(ipApiResponse))
                {
                    throw new Exception("Invalid response {IpApiResponse} received from ip-api.com"); ;
                }

                return new Location
                {
                    Country = ipApiResponse.Country,
                    CountryCode = ipApiResponse.CountryCode,
                    City = ipApiResponse.City,
                    Longitude = ipApiResponse.Lon,
                    Latitude = ipApiResponse.Lat,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve location information for {IpAddress}", ip);
        }
        return null;
    }

    private static bool IsInvalidResponse(IpApiResponse ipApiResponse) => 
        string.IsNullOrWhiteSpace(ipApiResponse.Status) || !string.Equals("success", ipApiResponse.Status, StringComparison.OrdinalIgnoreCase);
}
