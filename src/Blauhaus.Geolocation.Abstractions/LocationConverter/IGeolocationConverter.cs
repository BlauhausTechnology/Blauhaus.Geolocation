using System;
using System.Threading.Tasks;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using CSharpFunctionalExtensions;

namespace Blauhaus.Geolocation.Abstractions.LocationConverter
{
    public interface IGeolocationConverter
    {
        Task<Result<Address>> ToAddressAsync(IGpsLocation gpsLocation);
        Task<Result<IGpsLocation>> FromAddressAsync(string address);
    }
}