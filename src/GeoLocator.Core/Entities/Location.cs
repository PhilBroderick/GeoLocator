﻿namespace GeoLocator.Core.Entities;

public class Location
{
    public string Country { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public string City { get; set; } = "";
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}