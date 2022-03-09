using GeoLocator.Core.Entities.LocationAggregate;
using GeoLocator.Core.Interfaces;
using GeoLocator.Shared.HttpClients;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GeoLocator.Infrastructure.Services.IpStack;

/// <summary>
/// Wraps the ipstack.com API - https://ipstack.com/
/// </summary>
public class IpStackService : IIpLocationLookupService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _accessKey;
    private readonly IAppLogger<IpStackService> _logger;

    public IpStackService(IHttpClientFactory httpClientFactory, IOptions<IpStackOptions> options,
        IAppLogger<IpStackService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _accessKey = options.Value.AccessKey;
        _logger = logger;
    }

    public async Task<Location> GetLocationFromIp(string ip)
    {
        _logger.LogInformation("Retrieving location information for {IpAddress}", ip);

        var client = _httpClientFactory.CreateClient(NamedHttpClients.IpStackHttpClient);

        var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}?access_key={_accessKey}");

        try
        {
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                };

                var ipStackResponse = JsonConvert.DeserializeObject<IpStackResponse>(json, jsonSettings);

                if (IsInvalidResponse(ipStackResponse))
                {
                    throw new Exception("Invalid response {ipStackResponse} received from ipstack.com"); ;
                }

                return new Location
                {
                    Country = ipStackResponse.CountryName,
                    CountryCode = ipStackResponse.CountryCode,
                    City = ipStackResponse.City,
                    Longitude = ipStackResponse.Longitude,
                    Latitude = ipStackResponse.Latitude,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve location information for {IpAddress}", ip);
        }
        return null;
    }

    private static bool IsInvalidResponse(IpStackResponse ipStackResponse) => 
        string.IsNullOrWhiteSpace(ipStackResponse.Ip) ||
        string.IsNullOrWhiteSpace(ipStackResponse.City);
}
