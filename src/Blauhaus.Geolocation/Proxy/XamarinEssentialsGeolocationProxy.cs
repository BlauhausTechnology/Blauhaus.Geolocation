using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Proxy
{
    public class XamarinEssentialsGeolocationProxy : IGeolocationProxy
    {
        public Task<Location> GetLastKnownLocationAsync()
        {
            return Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
        }

        public Task<Location> GetCurrentLocationAsync(GeolocationRequest request)
        {
            return Xamarin.Essentials.Geolocation.GetLocationAsync(request);
        }

    }
}