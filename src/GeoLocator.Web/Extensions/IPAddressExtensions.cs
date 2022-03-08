using System.Net;

namespace GeoLocator.Web.Extensions
{
    public static class IPAddressExtensions
    {
        public static bool IsLocal(this IPAddress ipAddress) =>
            string.IsNullOrEmpty(ipAddress.ToString())
            || ipAddress.ToString() == "127.0.0.1"
            || ipAddress.ToString() == "::1";
    }
}
