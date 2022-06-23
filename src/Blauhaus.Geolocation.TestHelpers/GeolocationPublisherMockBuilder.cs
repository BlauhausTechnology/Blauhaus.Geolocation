using Blauhaus.Common.TestHelpers.MockBuilders;
using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Abstractions.ValueObjects;

namespace Blauhaus.Geolocation.TestHelpers;

public class GeolocationPublisherMockBuilder : BaseAsyncPublisherMockBuilder<GeolocationPublisherMockBuilder,IGeolocationPublisher,IGpsLocation>
{
    
}