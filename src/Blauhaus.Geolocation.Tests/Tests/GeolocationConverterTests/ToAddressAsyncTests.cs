using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Tests.Tests._Base;
using Moq;
using NUnit.Framework;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Tests.Tests.GeolocationConverterTests
{
    public class ToAddressAsyncTests : BaseGeolocationTest<GeolocationConverter>
    {
        private GpsLocation _gpsLocation;
        private Placemark _placemark;

        public override void Setup()
        {
            base.Setup();

            _gpsLocation = new GpsLocation(-10, 20);
            _placemark = new Placemark
            {
                CountryCode = "country code",
                CountryName = "country name",
                PostalCode = "postal code",
                AdminArea = "admin area",
                SubAdminArea = "sub admin area",
                Locality = "locality",
                SubLocality = "sub locality",
                Thoroughfare = "thoroughfare",
                SubThoroughfare = "sub thoroughfare",
                FeatureName = "feature name",
            };

            MockProxy.Where_GetPlacemarksFromLocationAsync_returns(_placemark);
        }

        [Test]
        public async Task SHOULD_request_placemarks()
        {
            //Act
            await Sut.ToAddressAsync(_gpsLocation);

            //Assert
            MockProxy.Mock.Verify(x => x.GetPlacemarksFromLocationAsync(It.Is<Location>(y => 
                y.Latitude == -10 &&
                y.Longitude == 20)));
        }

        [Test]
        public async Task SHOULD_convert_placemarks_to_addresses_and_return_first()
        {
            //Act
            var result = await Sut.ToAddressAsync(_gpsLocation);

            //Assert
            Assert.That(result.Value.PlaceName, Is.EqualTo(_placemark.FeatureName));
            Assert.That(result.Value.StreetNumber, Is.EqualTo(_placemark.SubThoroughfare));
            Assert.That(result.Value.StreetName, Is.EqualTo(_placemark.Thoroughfare));
            Assert.That(result.Value.Suburb, Is.EqualTo(_placemark.SubLocality));
            Assert.That(result.Value.City, Is.EqualTo(_placemark.Locality));
            Assert.That(result.Value.County, Is.EqualTo(_placemark.SubAdminArea));
            Assert.That(result.Value.State, Is.EqualTo(_placemark.AdminArea));
            Assert.That(result.Value.PostalCode, Is.EqualTo(_placemark.PostalCode));
            Assert.That(result.Value.Country, Is.EqualTo(_placemark.CountryName));
            Assert.That(result.Value.CountryCode, Is.EqualTo(_placemark.CountryCode));
        }
        
        [Test]
        public async Task SHOULD_trace_success()
        {
            //Act
            await Sut.ToAddressAsync(_gpsLocation);

            //Assert
            MockAnalyticsService.VerifyTrace("Address found from GPS coordinates");
            MockAnalyticsService.VerifyTraceProperty("Address", "feature name, sub thoroughfare thoroughfare, sub locality, sub admin area, locality, admin area, postal code, country name");
            MockAnalyticsService.VerifyTraceProperty(nameof(GpsLocation), _gpsLocation);
        }
        [Test]
        public async Task IF_no_placemarks_found_SHOULD_Error_and_trace()
        {
            //Arrange
            MockProxy.Where_GetPlacemarksFromLocationAsync_returns(new List<Placemark>());

            //Act
            var result = await Sut.ToAddressAsync(_gpsLocation);

            //Assert
            MockAnalyticsService.VerifyTrace("No address found for Gps coordinates", LogSeverity.Warning);
            MockAnalyticsService.VerifyTraceProperty(nameof(GpsLocation), _gpsLocation);
            Assert.That(result.IsError(GeolocationErrors.AddressNotFound));
        }

        
        [Test]
        public async Task IF_API_call_throws_SHOULD_return_Error()
        {
            //Arrange
            MockProxy.Where_GetPlacemarksFromLocationAsync_throws(new Exception("oops"));

            //Act
            var result = await Sut.ToAddressAsync(_gpsLocation);

            //Assert
            Assert.That(result.IsError(GeolocationErrors.AddressLookupFailed));
            MockAnalyticsService.VerifyLogException<Exception>("oops");
        }
    }
}