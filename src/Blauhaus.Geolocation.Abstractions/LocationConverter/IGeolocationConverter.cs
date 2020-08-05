using System;
using System.Threading.Tasks;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using CSharpFunctionalExtensions;

namespace Blauhaus.Geolocation.Abstractions.LocationConverter
{
    public interface IGeolocationConverter
    {
        Task<Result<Address>> ToAddressAsync(GpsLocation gpsLocation);
        Task<Result<GpsLocation>> FromAddressAsync(string address);
    }
}