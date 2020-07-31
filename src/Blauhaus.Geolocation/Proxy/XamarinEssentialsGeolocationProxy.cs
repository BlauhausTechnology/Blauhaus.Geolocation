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
    }
}