namespace GeoLocator.Infrastructure.Services.IpStack;
public class IpStackOptions
{
    public const string IpStack = "IpStack";

    public string BaseUrl { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public bool Enabled { get; set; }
}
