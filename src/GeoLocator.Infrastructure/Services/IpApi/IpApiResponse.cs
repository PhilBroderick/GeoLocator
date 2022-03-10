
namespace GeoLocator.Infrastructure.Services.IpApi;
public class IpApiResponse
{
    public string Status { get; set; } = "";
    public string Country { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public string Region { get; set; } = "";
    public string RegionName { get; set; } = "";
    public string City { get; set; } = "";
    public double Lat { get; set; }
    public double Lon { get; set; }
}
