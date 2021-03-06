﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.LocationConverter;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Extensions;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Responses;
using Blauhaus.Responses.Extensions;
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
        public async Task<Response<Address>> ToAddressAsync(IGpsLocation gpsLocation)
        {
            if (gpsLocation == null)
            {
                return _analyticsService.TraceErrorResponse<Address>(this, GeolocationErrors.EmptyLocation);
            }
            try
            {
                var placemarks = await _proxy.GetPlacemarksFromLocationAsync(new Location(gpsLocation.Latitude, gpsLocation.Longitude));
                var bestMatch = placemarks.FirstOrDefault();

                if (bestMatch == null)
                {
                    _analyticsService.TraceWarning(this, "No address found for Gps coordinates", gpsLocation.ToObjectDictionary());
                    return GeolocationErrors.AddressNotFound.ToResponse<Address>();
                }

                var address = bestMatch.ToAddress();

                _analyticsService.TraceVerbose(this, "Address found from GPS coordinates", gpsLocation.ToObjectDictionary()
                    .WithValue("Address", address.ToString()));

                return Response.Success(address);
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse<Address>(this, e, GeolocationErrors.AddressLookupFailed, gpsLocation.ToObjectDictionary("GpsLocation"));
            }
        }

        public async Task<Response<IGpsLocation>> FromAddressAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return _analyticsService.TraceErrorResponse<IGpsLocation>(this, GeolocationErrors.EmptyAddress);
            }
            try
            {
                var matchingLocations = await _proxy.GetLocationsFromAddressAsync(address);
                var bestMatch = matchingLocations.FirstOrDefault();

                if (bestMatch == null)
                {
                    _analyticsService.TraceWarning(this, "No GPS coordinates found for given address", address.ToObjectDictionary("Address"));
                    return GeolocationErrors.GpsCoordinatesNotFound.ToResponse<IGpsLocation>();
                }
                
                var gpsLocation = bestMatch.ToGpsLocation();
                
                _analyticsService.TraceVerbose(this, "GPS cooridnates found from address", gpsLocation.ToObjectDictionary()
                    .WithValue("Address", address));

                return Response.Success<IGpsLocation>(gpsLocation);

            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse<IGpsLocation>(this, e, GeolocationErrors.GpsLookupFailed, address.ToObjectDictionary("AddressString"));
            }
        }
    }
}