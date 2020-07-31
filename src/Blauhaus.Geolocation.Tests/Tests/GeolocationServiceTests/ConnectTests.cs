using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Errors;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Geolocation.Tests.MockBuilders;
using Blauhaus.Geolocation.Tests.Tests._Base;
using NUnit.Framework;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Tests.Tests.GeolocationServiceTests
{
    public class ConnectTests : BaseGeolocationTest<GeolocationService>
    {

        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task SHOULD_publish_last_known_location()
        {
            //Arrange
            MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(12, 22, DateTimeOffset.Now));

            //Act
            Sut.Connect(TimeSpan.Zero).Subscribe(next =>
            {
                LocationTaskCompletionSource.SetResult(new List<IGpsLocation> { next });
            });
            var locations = await LocationTaskCompletionSource.Task;

            //Assert
            Assert.That(locations[0].Latitude, Is.EqualTo(12));
            Assert.That(locations[0].Longitude, Is.EqualTo(22));
        }

        [Test]
        public async Task IF_last_known_location_is_invalid_SHOULD_error()
        {
            //Arrange
            MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(-212, 22, DateTimeOffset.Now));

            //Act
            Sut.Connect(TimeSpan.Zero).Subscribe(next => { }, ex =>
            {
                ExceptionTaskCompletionSource.SetResult(ex);
            });
            var exception = await ExceptionTaskCompletionSource.Task;

            //Assert
            Assert.That(exception.ToError(), Is.EqualTo(GeolocationErrors.InvalidLatitude));
            MockAnalyticsService.VerifyTrace(GeolocationErrors.InvalidLatitude.ToString(), LogSeverity.Error);
        }

        [Test]
        public async Task IF_last_known_location_throws_SHOULD_error()
        {
            //Arrange
            MockProxy.Where_GetLastKnownLocationAsync_throws(new Exception("oops"));

            //Act
            Sut.Connect(TimeSpan.Zero).Subscribe(next => { }, ex =>
            {
                ExceptionTaskCompletionSource.SetResult(ex);
            });
            var exception = await ExceptionTaskCompletionSource.Task;

            //Assert
            Assert.That(exception.Message, Is.EqualTo("oops"));
            MockAnalyticsService.VerifyLogException<Exception>("oops");
        }
    }
}