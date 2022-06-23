using Blauhaus.Common.Abstractions;
using Blauhaus.Geolocation.Abstractions.ValueObjects;

namespace Blauhaus.Geolocation.Abstractions.Service
{
    public interface IGeolocationPublisher : IAsyncPublisher<IGpsLocation>, IAsyncInitializable<GeolocationRequirements>
    {
    }
}