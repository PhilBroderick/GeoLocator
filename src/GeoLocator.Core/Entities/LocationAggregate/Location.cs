using GeoLocator.Shared;
using GeoLocator.Shared.Interfaces;

namespace GeoLocator.Core.Entities.LocationAggregate;

public class Location : IAggregateRoot
{
    private List<IpAddress> _ipAddresses = new();

    public IEnumerable<IpAddress> IpAddresses => _ipAddresses.AsReadOnly();

    public string Country { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public string City { get; set; } = "";
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public int Id {get; protected set;}

    public void AddIpAddress(IpAddress ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress.Ip))
        {
            throw new ArgumentNullException(nameof(ipAddress));
        }

        _ipAddresses.Add(ipAddress);
    }
}