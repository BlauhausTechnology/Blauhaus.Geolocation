using System;
using System.Collections.Generic;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Tests.MockBuilders
{
    public class GeolocationProxyMockBuilder : BaseMockBuilder<GeolocationProxyMockBuilder, IGeolocationProxy>
    {

        public GeolocationProxyMockBuilder()
        {
            Where_GetPlacemarksFromLocationAsync_returns(new List<Placemark>());
            Where_GetLocationsFromAddressAsync_returns(new List<Location>());
        }

        public GeolocationProxyMockBuilder Where_GetLastKnownLocationAsync_returns(Location location)
        {
            Mock.Setup(x => x.GetLastKnownLocationAsync()).ReturnsAsync(location);
            return this;
        }
        
        public GeolocationProxyMockBuilder Where_GetLastKnownLocationAsync_throws(Exception e)
        {
            Mock.Setup(x => x.GetLastKnownLocationAsync()).ThrowsAsync(e);
            return this;
        }

        public GeolocationProxyMockBuilder Where_GetCurrentLocationAsync_returns(Location location)
        {
            Mock.Setup(x => x.GetCurrentLocationAsync(It.IsAny<GeolocationRequest>())).ReturnsAsync(location);
            return this;
        }
        public GeolocationProxyMockBuilder Where_GetCurrentLocationAsync_returns_sequence(IEnumerable<Location> locations)
        {
            var queue = new Queue<Location>(locations);
            Mock.Setup(x => x.GetCurrentLocationAsync(It.IsAny<GeolocationRequest>())).ReturnsAsync(queue.Dequeue);
            return this;
        }
        public GeolocationProxyMockBuilder Where_GetCurrentLocationAsync_throws(Exception e)
        {
            Mock.Setup(x => x.GetCurrentLocationAsync(It.IsAny<GeolocationRequest>())).ThrowsAsync(e);
            return this;
        }

        
        public GeolocationProxyMockBuilder Where_GetPlacemarksFromLocationAsync_returns(Placemark placemark)
        {
            Mock.Setup(x => x.GetPlacemarksFromLocationAsync(It.IsAny<Location>())).ReturnsAsync(new List<Placemark>{placemark});
            return this;
        }
        public GeolocationProxyMockBuilder Where_GetPlacemarksFromLocationAsync_returns(IEnumerable<Placemark> placemarks)
        {
            Mock.Setup(x => x.GetPlacemarksFromLocationAsync(It.IsAny<Location>())).ReturnsAsync(placemarks);
            return this;
        }
        public GeolocationProxyMockBuilder Where_GetPlacemarksFromLocationAsync_throws(Exception e)
        {
            Mock.Setup(x => x.GetPlacemarksFromLocationAsync(It.IsAny<Location>())).ThrowsAsync(e);
            return this;
        }

        
        public GeolocationProxyMockBuilder Where_GetLocationsFromAddressAsync_returns(Location location)
        {
            Mock.Setup(x => x.GetLocationsFromAddressAsync(It.IsAny<string>())).ReturnsAsync(new List<Location>{location});
            return this;
        }
        public GeolocationProxyMockBuilder Where_GetLocationsFromAddressAsync_returns(IEnumerable<Location> locations)
        {
            Mock.Setup(x => x.GetLocationsFromAddressAsync(It.IsAny<string>())).ReturnsAsync(locations);
            return this;
        }
        public GeolocationProxyMockBuilder Where_GetLocationsFromAddressAsync_throws(Exception e)
        {
            Mock.Setup(x => x.GetLocationsFromAddressAsync(It.IsAny<string>())).ThrowsAsync(e);
            return this;
        }
    }
}