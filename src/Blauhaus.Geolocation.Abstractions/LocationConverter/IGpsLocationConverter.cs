using System;
using System.Threading.Tasks;
using Blauhaus.Geolocation.Abstractions.ValueObjects;

namespace Blauhaus.Geolocation.Abstractions.LocationConverter
{
    public interface IGpsLocationConverter
    {
        Task<Address> ToAddressAsync(IGpsLocation gpsLocation);
        Task<IGpsLocation> FromAddressAsync(Address address);
    }
}