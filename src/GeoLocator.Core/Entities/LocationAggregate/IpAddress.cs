using GeoLocator.Shared;

namespace GeoLocator.Core.Entities.LocationAggregate;
public class IpAddress : BaseEntity<int>
{
    public string Ip { get; set; } = "";
}