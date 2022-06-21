using System;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.LocationConverter;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Extensions;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Responses;
using Blauhaus.Responses.Extensions;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation
{
    public class GeolocationConverter : IGeolocationConverter
    {
        private readonly IAnalyticsLogger<GeolocationConverter> _logger;
        private readonly IGeolocationProxy _proxy;

        public GeolocationConverter(
            IAnalyticsLogger<GeolocationConverter> logger,
            IGeolocationProxy proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }
        public async Task<Response<Address>> ToAddressAsync(IGpsLocation gpsLocation)
        {
            if (gpsLocation == null)
            {
                return _logger.LogErrorResponse<Address>(GeolocationError.EmptyLocation);
            }
            try
            {
                var placemarks = await _proxy.GetPlacemarksFromLocationAsync(new Location(gpsLocation.Latitude, gpsLocation.Longitude));
                var bestMatch = placemarks.FirstOrDefault();

                if (bestMatch == null)
                {
                    _logger.LogWarning("No address found for Gps coordinates {@GeoLocation}", gpsLocation);
                    return GeolocationError.AddressNotFound.ToResponse<Address>();
                }

                var address = bestMatch.ToAddress();

                _logger.LogDebug("Address found from GPS coordinates {@Geolocation}", gpsLocation);

                return Response.Success(address);
            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse<Address>(GeolocationError.AddressLookupFailed, e);
            }
        }

        public async Task<Response<IGpsLocation>> FromAddressAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return _logger.LogErrorResponse<IGpsLocation>(GeolocationError.EmptyAddress);
            }
            try
            {
                var matchingLocations = await _proxy.GetLocationsFromAddressAsync(address);
                var bestMatch = matchingLocations.FirstOrDefault();

                if (bestMatch == null)
                {
                    return _logger.LogErrorResponse<IGpsLocation>(GeolocationError.GpsCoordinatesNotFound);
                }
                
                var gpsLocation = bestMatch.ToGpsLocation();
                
                _logger.LogTrace("GPS cooridnates found from address {@GeoLocation}", gpsLocation);

                return Response.Success<IGpsLocation>(gpsLocation);

            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse<IGpsLocation>(GeolocationError.GpsLookupFailed, e);
            }
        }
    }
}