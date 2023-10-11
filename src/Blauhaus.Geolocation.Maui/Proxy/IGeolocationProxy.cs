using Microsoft.Maui.Devices.Sensors;

namespace Blauhaus.Geolocation.Maui.Proxy
{
    public interface IGeolocationProxy
    {
        Task<Location> GetLastKnownLocationAsync();
        Task<Location> GetCurrentLocationAsync(GeolocationRequest request);

        Task<IEnumerable<Location>> GetLocationsFromAddressAsync(string address);
        Task<IEnumerable<Placemark>> GetPlacemarksFromLocationAsync(Location location);
    }
}