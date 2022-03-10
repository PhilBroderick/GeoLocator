namespace GeoLocator.Infrastructure.Services.IpApi;
public class IpApiOptions
{
    public const string IpApi = "IpApi";

    public string BaseUrl { get; set; } = "";
    public bool Enabled { get; set; }
}
