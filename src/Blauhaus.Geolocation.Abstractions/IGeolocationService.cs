using System;

namespace Blauhaus.Geolocation.Abstractions
{
    public interface IGeolocationService
    {
        IObservable<IGpsLocation> Connect(TimeSpan interval);
    }
}