using GeoLocator.Core.Interfaces;
using System.Net;

namespace GeoLocator.Infrastructure.Services;
public class IpValidator : IIpValidator
{
    public bool ValidateIp(string ip) => IPAddress.TryParse(ip, out _);
}
