using Blauhaus.Errors;

namespace Blauhaus.Geolocation.Abstractions
{
    public static class GeolocationErrors
    {
        public static readonly Error InvalidLatitude = Error.Create("The value provided for Latitude is invalid. Latitude must be between -90 and +90 degrees");
        public static readonly Error InvalidLongitude = Error.Create("The value provided for Longitude is invalid. Latitude must be between -180 and +180 degrees");

    }
}