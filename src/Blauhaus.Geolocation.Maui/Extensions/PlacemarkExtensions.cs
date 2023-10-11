using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Microsoft.Maui.Devices.Sensors;

namespace Blauhaus.Geolocation.Maui.Extensions
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