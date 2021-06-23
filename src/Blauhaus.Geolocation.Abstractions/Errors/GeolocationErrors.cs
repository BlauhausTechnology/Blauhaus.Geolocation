using Blauhaus.Errors;

namespace Blauhaus.Geolocation.Abstractions.Errors
{
    public static class GeolocationErrors
    {
        public static readonly Error Unexpected = Error.Create("An unexpected error occured while attempting to access geolocation services");
       
        public static readonly Error InvalidLatitude = Error.Create("The value provided for Latitude is invalid. Latitude must be between -90 and +90 degrees");
        public static readonly Error InvalidLongitude = Error.Create("The value provided for Longitude is invalid. Latitude must be between -180 and +180 degrees");
        public static readonly Error EmptyAddress = Error.Create("The given address was empty");
        public static readonly Error EmptyLocation = Error.Create("No location was provided for the address lookup");

        public static Error InvalidSerialization() => Error.Create("The text provided is not a valid GPS location");
        public static Error InvalidSerialization(string exceptionMessage) => Error.Create("The text provided is not a valid GPS location. Exception message: " + exceptionMessage);
        
        public static readonly Error AddressNotFound = Error.Create("No address was found for these coordinates");
        public static readonly Error AddressLookupFailed = Error.Create("An unexpected error occured while looking for an address");
        public static readonly Error GpsCoordinatesNotFound = Error.Create("No GPS location was found for this address");
        public static readonly Error GpsLookupFailed = Error.Create("An unexpected error occured while looking for GPS coordinates");

    }
}