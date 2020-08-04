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
    }
}