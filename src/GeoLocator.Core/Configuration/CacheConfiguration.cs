﻿namespace GeoLocator.Core.Configuration;
public class CacheConfiguration
{
    public int AbsoluteExpirationInHours { get; set; }
    public int SlidingExpirationInMinutes { get; set; }
}
