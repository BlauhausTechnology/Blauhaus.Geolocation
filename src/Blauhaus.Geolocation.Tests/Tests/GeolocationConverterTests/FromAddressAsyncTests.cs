using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Tests.Tests._Base;
using NUnit.Framework;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Tests.Tests.GeolocationConverterTests
{
    public class FromAddressAsyncTests : BaseGeolocationTest<GeolocationConverter>
    {
        private string _addressString;
        private Location _location;

        public override void Setup()
        {
            base.Setup();

            _addressString = "6 Terrace Road";
            _location = new Location
            {
                Latitude = 11,
                Longitude = 22 
            };

            MockProxy.Where_GetLocationsFromAddressAsync_returns(_location);
        }

        [Test]
        public async Task SHOULD_request_matching_locations()
        {
            //Act
            await Sut.FromAddressAsync(_addressString);

            //Assert
            MockProxy.Mock.Verify(x => x.GetLocationsFromAddressAsync(_addressString));
        }

        [Test]
        public async Task IF_address_string_is_empty_SHOULD_fail()
        {
            //Act
            var result = await Sut.FromAddressAsync("");

            //Assert
            result.VerifyResultError(GeolocationErrors.EmptyAddress, MockAnalyticsService); 
        }

        [Test]
        public async Task SHOULD_return_first_location()
        {
            //Act
            var result = await Sut.FromAddressAsync(_addressString);

            //Assert
            Assert.That(result.Value.Latitude, Is.EqualTo(_location.Latitude));
            Assert.That(result.Value.Longitude, Is.EqualTo(_location.Longitude));
        }

        [Test]
        public async Task SHOULD_trace_success()
        {
            //Act
            await Sut.FromAddressAsync(_addressString);

            //Assert
            MockAnalyticsService.VerifyTrace("GPS cooridnates found from address");
            MockAnalyticsService.VerifyTraceProperty("Address", _addressString);
        }

        [Test]
        public async Task IF_no_locations_found_SHOULD_Error_and_trace()
        {
            //Arrange
            MockProxy.Where_GetLocationsFromAddressAsync_returns(new List<Location>());

            //Act
            var result = await Sut.FromAddressAsync(_addressString);

            //Assert
            MockAnalyticsService.VerifyTrace("No GPS coordinates found for given address", LogSeverity.Warning);
            MockAnalyticsService.VerifyTraceProperty("Address", _addressString);
            Assert.That(result.IsError(GeolocationErrors.GpsCoordinatesNotFound));
        }

        
        [Test]
        public async Task IF_API_call_throws_SHOULD_return_Error()
        {
            //Arrange
            MockProxy.Where_GetLocationsFromAddressAsync_throws(new Exception("oops"));

            //Act
            var result = await Sut.FromAddressAsync(_addressString);

            //Assert
            Assert.That(result.IsError(GeolocationErrors.GpsLookupFailed));
            MockAnalyticsService.VerifyLogException<Exception>("oops");
        }
    }
}