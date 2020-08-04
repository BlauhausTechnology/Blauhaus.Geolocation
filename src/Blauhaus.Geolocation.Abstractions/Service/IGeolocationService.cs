using System;
using Blauhaus.Geolocation.Abstractions.ValueObjects;

namespace Blauhaus.Geolocation.Abstractions.Service
{
    public interface IGeolocationService
    {
        IObservable<IGpsLocation> Connect(GeolocationRequirements requirements);
    }
}