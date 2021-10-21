using Blauhaus.Common.ValueObjects.Base;

namespace Blauhaus.Geolocation.Abstractions.ValueObjects
{
    public interface IGpsLocation : IValueObject<IGpsLocation> 
    {
        double Longitude { get; }
        double Latitude { get; }
        double GetMetresFrom(IGpsLocation otherLocation);
    }
}