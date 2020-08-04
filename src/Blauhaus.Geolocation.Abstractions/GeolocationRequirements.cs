using System;

namespace Blauhaus.Geolocation.Abstractions
{
    public class GeolocationRequirements
    {
        public GeolocationRequirements(
            TimeSpan updateInterval, 
            LocationAccuracy accuracy = LocationAccuracy.Medium)
        {
            UpdateInterval = updateInterval;
            Accuracy = accuracy;
        }

        public TimeSpan UpdateInterval { get; }
        public LocationAccuracy Accuracy { get; }
    }
}