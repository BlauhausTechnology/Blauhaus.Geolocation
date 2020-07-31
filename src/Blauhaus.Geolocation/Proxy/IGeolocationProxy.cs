using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Proxy
{
    public interface IGeolocationProxy
    {
        Task<Location> GetLastKnownLocationAsync();
    }
}