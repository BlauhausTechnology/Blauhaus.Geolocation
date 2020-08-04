namespace Blauhaus.Geolocation.Abstractions.ValueObjects
{
    public interface IGpsLocation
    {
        double Longitude { get; }
        double Latitude { get; }
        double GetMetresFrom(GpsLocation otherLocation);
    }
}