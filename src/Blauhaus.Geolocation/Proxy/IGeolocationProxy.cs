using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Proxy
{
    public interface IGeolocationProxy
    {
        Task<Location> GetLastKnownLocationAsync();
        Task<Location> GetCurrentLocationAsync(GeolocationRequest request);

        Task<IEnumerable<Location>> GetLocationsFromAddressAsync(string address);
        Task<IEnumerable<Placemark>> GetPlacemarksFromLocationAsync(Location location);
    }
}