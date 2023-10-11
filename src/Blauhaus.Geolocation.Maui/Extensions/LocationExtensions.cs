using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Microsoft.Maui.Devices.Sensors;

namespace Blauhaus.Geolocation.Maui.Extensions
{
    public static class LocationExtensions
    {
        public static GpsLocation ToGpsLocation(this Location location)
        {
            return new GpsLocation(
                location.Latitude,
                location.Longitude);
        }
    }
}