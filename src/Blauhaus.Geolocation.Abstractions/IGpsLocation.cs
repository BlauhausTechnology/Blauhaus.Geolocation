namespace Blauhaus.Geolocation.Abstractions
{
    public interface IGpsLocation
    {
        double Longitude { get; }
        double Latitude { get; }
        double GetMetresFrom(GpsLocation otherLocation);
    }
}