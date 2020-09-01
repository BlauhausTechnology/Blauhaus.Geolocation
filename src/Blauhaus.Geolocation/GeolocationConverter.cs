using System;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.DeviceServices.Abstractions.Thread;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.LocationConverter;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Extensions;
using Blauhaus.Geolocation.Proxy;
using CSharpFunctionalExtensions;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation
{
    public class GeolocationConverter : IGeolocationConverter
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IGeolocationProxy _proxy;

        public GeolocationConverter(
            IAnalyticsService analyticsService,
            IGeolocationProxy proxy)
        {
            _analyticsService = analyticsService;
            _proxy = proxy;
        }
        public async Task<Result<Address>> ToAddressAsync(IGpsLocation gpsLocation)
        {
            try
            {
                var placemarks = await _proxy.GetPlacemarksFromLocationAsync(new Location(gpsLocation.Latitude, gpsLocation.Longitude));
                var bestMatch = placemarks.FirstOrDefault();

                if (bestMatch == null)
                {
                    _analyticsService.TraceWarning(this, "No address found for Gps coordinates", gpsLocation.ToObjectDictionary());
                    return GeolocationErrors.AddressNotFound.ToResult<Address>();
                }

                var address = bestMatch.ToAddress();

                _analyticsService.TraceVerbose(this, "Address found from GPS coordinates", gpsLocation.ToObjectDictionary()
                    .WithValue("Address", address.ToString()));

                return address;
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResult<Address>(this, e, GeolocationErrors.AddressLookupFailed);
            }
        }

        public async Task<Result<IGpsLocation>> FromAddressAsync(string address)
        {
            try
            {
                var matchingLocations = await _proxy.GetLocationsFromAddressAsync(address);
                var bestMatch = matchingLocations.FirstOrDefault();

                if (bestMatch == null)
                {
                    _analyticsService.TraceWarning(this, "No GPS coordinates found for given address", address.ToObjectDictionary("Address"));
                    return GeolocationErrors.GpsCoordinatesNotFound.ToResult<IGpsLocation>();
                }
                
                var gpsLocation = bestMatch.ToGpsLocation();
                
                _analyticsService.TraceVerbose(this, "GPS cooridnates found from address", gpsLocation.ToObjectDictionary()
                    .WithValue("Address", address));

                return gpsLocation;

            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResult<IGpsLocation>(this, e, GeolocationErrors.GpsLookupFailed);
            }
        }
    }
}