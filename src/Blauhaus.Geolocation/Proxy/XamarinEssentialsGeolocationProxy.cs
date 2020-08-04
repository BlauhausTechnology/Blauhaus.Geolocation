using System.Threading.Tasks;
using Blauhaus.DeviceServices.Abstractions.Thread;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Proxy
{
    public class XamarinEssentialsGeolocationProxy : IGeolocationProxy
    {
        private readonly IThreadService _threadService;

        public XamarinEssentialsGeolocationProxy(IThreadService threadService)
        {
            _threadService = threadService;
        }


        public Task<Location> GetLastKnownLocationAsync()
        {
            return _threadService.InvokeOnMainThreadAsync(async () => 
                await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync());
        }

        public Task<Location> GetCurrentLocationAsync(GeolocationRequest request)
        {
            return _threadService.InvokeOnMainThreadAsync(async () => 
                await Xamarin.Essentials.Geolocation.GetLocationAsync(request));
        }
         

    }
}