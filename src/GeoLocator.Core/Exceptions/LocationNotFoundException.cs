namespace GeoLocator.Core.Exceptions;

public class LocationNotFoundException : KeyNotFoundException
{
    public LocationNotFoundException(string ipAddress) 
        : base(ipAddress)
    {
    }
}
