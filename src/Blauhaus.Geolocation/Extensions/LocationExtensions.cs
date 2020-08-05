using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Extensions
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