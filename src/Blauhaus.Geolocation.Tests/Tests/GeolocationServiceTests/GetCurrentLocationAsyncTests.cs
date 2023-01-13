using Blauhaus.DeviceServices.Abstractions.Permissions;
using System.Threading.Tasks;
using System;
using Blauhaus.Geolocation.Tests.Tests.Base;
using NUnit.Framework;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Responses;
using Moq;
using Xamarin.Essentials;
using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Errors;
using Blauhaus.Errors.Extensions;
using Blauhaus.Responses.Extensions;

namespace Blauhaus.Geolocation.Tests.Tests.GeolocationServiceTests
{
    [TestFixture]
    public class GetCurrentLocationAsyncTests : BaseGeolocationTest<GeolocationService>
    {
        public override void Setup()
        {
            base.Setup();
            
            MockProxy.Where_GetLastKnownLocationAsync_returns(null);
            MockProxy.Where_GetCurrentLocationAsync_returns(new Location(12, 22, DateTimeOffset.Now));
            MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(2, 3, DateTimeOffset.Now));
        }

        public class Permissions : GetCurrentLocationAsyncTests
        {
            [Test]
            public async Task SHOULD_request_LocationWhenInUse_permission()
            {
                //Act
                await Sut.GetCurrentLocationAsync();

                //Assert
                MockDevicePermissionsService.Mock.Verify(x=> x.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse));
            }

            [Test]
            public async Task IF_getting_permission_fails_SHOULD_error()
            {
                //Arrange 
                MockDevicePermissionsService.Mock.Setup(x => x.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse))
                    .ReturnsAsync(Response.Failure(Error.Cancelled));

                //Act
                var result = await Sut.GetCurrentLocationAsync();

                //Assert
                Assert.That(result.Error, Is.EqualTo(Error.Cancelled));

            }
        }


        public class LastKnownLocation : GetCurrentLocationAsyncTests
        {
            public override void Setup()
            {
                base.Setup();
                MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(2, 3, DateTimeOffset.Now));
            }
            
            [Test]
            public async Task IF_lastknownlocation_is_available_SHOULD_return_it()
            { 
                //Act
                var result = await Sut.GetCurrentLocationAsync();

                //Assert 
                Assert.That(result.Value.Latitude, Is.EqualTo(2));
                Assert.That(result.Value.Longitude, Is.EqualTo(3));

            }

            [TestCase(LocationAccuracy.High, GeolocationAccuracy.Best)]
            [TestCase(LocationAccuracy.Medium, GeolocationAccuracy.Medium)]
            [TestCase(LocationAccuracy.Low, GeolocationAccuracy.Low)]
            public async Task SHOULD_use_given_accuracy(LocationAccuracy given, GeolocationAccuracy required)
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(22, 66, DateTimeOffset.Now));

                //Act
                await Sut.GetCurrentLocationAsync(given);

                //Assert
                MockProxy.Mock.Verify(x => x.GetLastKnownLocationAsync());
            }

            
            [Test]
            public async Task IF_LastKnownLocation_is_invalid_SHOULD_error()
            {
                //Arrange
                MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(-212, 22, DateTimeOffset.Now));

                //Act
                var result = await Sut.GetCurrentLocationAsync();

                //Assert
                Assert.That(result.IsError(GeolocationError.InvalidLatitude));
            }
            
            [Test]
            public async Task IF_LastKnownLocation_throws_SHOULD_error()
            {
                //Arrange
                MockProxy.Where_GetLastKnownLocationAsync_throws(new Exception("oops"));

                //Act
                var result = await Sut.GetCurrentLocationAsync();

                //Assert
                Assert.That(result.IsError(GeolocationError.Unexpected));
            }
        }


        public class CurrentLocation: GetCurrentLocationAsyncTests
        {
            public override void Setup()
            {
                base.Setup();
                MockProxy.Where_GetLastKnownLocationAsync_returns(null);
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(12, 22, DateTimeOffset.Now));
            }
            
            [Test]
            public async Task IF_LastKnownLocation_is_unavailable_SHOULD_return_current_location_and_trace()
            { 
                //Arrange
                MockProxy.Where_GetLastKnownLocationAsync_returns(null);

                //Act
                var result = await Sut.GetCurrentLocationAsync();

                //Assert
                Assert.That(result.Value.Latitude, Is.EqualTo(12));
                Assert.That(result.Value.Longitude, Is.EqualTo(22));
            }

            [TestCase(LocationAccuracy.High, GeolocationAccuracy.Best)]
            [TestCase(LocationAccuracy.Medium, GeolocationAccuracy.Medium)]
            [TestCase(LocationAccuracy.Low, GeolocationAccuracy.Low)]
            public async Task SHOULD_use_given_accuracy(LocationAccuracy given, GeolocationAccuracy required)
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(22, 66, DateTimeOffset.Now));

                //Act
                await Sut.GetCurrentLocationAsync(given);

                //Assert
                MockProxy.Mock.Verify(x => x.GetCurrentLocationAsync(It.Is<GeolocationRequest>(y => y.DesiredAccuracy == required)));
            }

            [Test]
            public async Task IF_current_location_throws_SHOULD_error()
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_throws(new Exception("oops"));

                //Act
                var result = await Sut.GetCurrentLocationAsync();

                //Assert
                Assert.That(result.IsError(GeolocationError.Unexpected));
            }

            [Test]
            public async Task IF_current_location_is_invalid_SHOULD_error()
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(-212, 22, DateTimeOffset.Now));

                //Act
                var result = await Sut.GetCurrentLocationAsync();

                //Assert
                Assert.That(result.IsError(GeolocationError.InvalidLatitude)); 
            }

        }

       
    }
}