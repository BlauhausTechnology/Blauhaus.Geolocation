using Blauhaus.DeviceServices.Abstractions.Thread;
using Microsoft.Maui.Devices.Sensors;

namespace Blauhaus.Geolocation.Maui.Proxy
{
    public class MauiEssentialsGeolocationProxy : IGeolocationProxy
    {
        private readonly IThreadService _threadService;

        public MauiEssentialsGeolocationProxy(IThreadService threadService)
        {
            _threadService = threadService;
        }


        public Task<Location> GetLastKnownLocationAsync()
        {
            return _threadService.InvokeOnMainThreadAsync(async () => 
                await Microsoft.Maui.Devices.Sensors.Geolocation.GetLastKnownLocationAsync());
        }

        public Task<Location> GetCurrentLocationAsync(GeolocationRequest request)
        {
            return _threadService.InvokeOnMainThreadAsync(async () => 
                await  Microsoft.Maui.Devices.Sensors.Geolocation.GetLocationAsync(request));
        }

        public Task<IEnumerable<Location>> GetLocationsFromAddressAsync(string address)
        {
            return _threadService.InvokeOnMainThreadAsync(async () => 
                await Geocoding.GetLocationsAsync(address));
        }

        
        public Task<IEnumerable<Placemark>> GetPlacemarksFromLocationAsync(Location location)
        {
            return _threadService.InvokeOnMainThreadAsync(async () => 
                await Geocoding.GetPlacemarksAsync(location));
        }
    }
}