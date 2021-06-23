using System;
using System.Threading.Tasks;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Responses;

namespace Blauhaus.Geolocation.Abstractions.Service
{
    public interface IGeolocationService
    {
        IObservable<IGpsLocation> Connect(GeolocationRequirements requirements);
        Task<Response<IGpsLocation>> GetCurrentLocationAsync(LocationAccuracy accuracy = LocationAccuracy.Medium);
    }
}