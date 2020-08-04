using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.DeviceServices.Abstractions.Thread;
using Blauhaus.Geolocation.Abstractions.LocationConverter;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Proxy;

namespace Blauhaus.Geolocation
{
    public class GpsLocationConverter : IGpsLocationConverter
    {
        public GpsLocationConverter(
            IAnalyticsService analyticsService,
            IDevicePermissionsService devicePermissionsService,
            IThreadService threadService,
            IGeolocationProxy proxy)
        {
            
        }
        public Task<Address> ToAddressAsync(IGpsLocation gpsLocation)
        {
            throw new System.NotImplementedException();
        }

        public Task<IGpsLocation> FromAddressAsync(Address address)
        {
            throw new System.NotImplementedException();
        }
    }
}