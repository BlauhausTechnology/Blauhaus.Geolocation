using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Extensions
{
    public static class PlacemarkExtensions
    {
        public static Address ToAddress(this Placemark placemark)
        {
            return new Address(
                placemark.CountryCode,
                placemark.CountryName,
                placemark.PostalCode,
                placemark.AdminArea,
                placemark.SubAdminArea,
                placemark.Locality,
                placemark.SubLocality,
                placemark.Thoroughfare,
                placemark.SubThoroughfare,
                placemark.FeatureName);
        }
    }
}