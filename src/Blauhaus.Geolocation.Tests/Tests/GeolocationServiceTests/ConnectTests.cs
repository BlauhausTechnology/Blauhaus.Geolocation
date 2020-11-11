using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.Errors;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Tests.Tests._Base;
using Blauhaus.Responses;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Tests.Tests.GeolocationServiceTests
{
    public class ConnectTests : BaseGeolocationTest<GeolocationService>
    {
        private TestScheduler _testScheduler;
        private GeolocationRequirements _requirements;

        public override void Setup()
        {
            base.Setup();
            
            _testScheduler = MockReactiveSchedulers.With_Test_ThreadPoolScheduler();
            _requirements = new GeolocationRequirements(TimeSpan.FromTicks(1000), LocationAccuracy.High);
            MockDevicePermissionsService.Mock.Setup(x => x.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse))
                .ReturnsAsync(Response.Success);
        }

        public class Permissions : ConnectTests
        {
            [Test]
            public async Task SHOULD_request_LocationWhenInUse_permission()
            {
                //Act
                Sut.Connect(_requirements).Subscribe();

                //Assert
                MockDevicePermissionsService.Mock.Verify(x=> x.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse));
            }

            [Test]
            public async Task IF_getting_permission_fails_SHOULD_error()
            {
                //Arrange 
                MockDevicePermissionsService.Mock.Setup(x => x.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse))
                    .ReturnsAsync(Response.Failure(DevicePermissionErrors.PermissionUnknown("perm")));
                var tcs = new TaskCompletionSource<Exception>();

                //Act
                Sut.Connect(_requirements).Subscribe(n => { }, e => {tcs.SetResult(e);});
                var exception = await tcs.Task;

                //Assert
                var errorException = exception as ErrorException;
                Assert.That(errorException, Is.Not.Null);
                Assert.That(errorException.Error, Is.EqualTo(DevicePermissionErrors.PermissionUnknown("perm")));
                MockAnalyticsService.VerifyTrace("Failed to access GpsLocation due to permissions failure", LogSeverity.Warning);

            }
        }

        public class LastKnownLocation : ConnectTests
        {
            
            public override void Setup()
            {
                base.Setup();

                MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(12, 22, DateTimeOffset.Now));
            }

            [Test]
            public async Task SHOULD_Trace_connection()
            { 
                //Arrange
                _requirements = new GeolocationRequirements(TimeSpan.FromMinutes(1), LocationAccuracy.Low);

                //Act
                Sut.Connect(_requirements).Subscribe(next => {  });

                //Assert
                MockAnalyticsService.VerifyTrace("New GpsLocation connection requested with accuracy Low and interval 00:01:00");
            }

            [Test]
            public async Task SHOULD_publish_last_known_location_and_trace()
            { 
                //Act
                Sut.Connect(_requirements)
                    .Subscribe(next => { LocationTaskCompletionSource.SetResult(new List<IGpsLocation> {next}); });
                var locations = await LocationTaskCompletionSource.Task;

                //Assert
                Assert.That(locations[0].Latitude, Is.EqualTo(12));
                Assert.That(locations[0].Longitude, Is.EqualTo(22));
                MockAnalyticsService.VerifyTrace("Last known location published");
            }

            [Test]
            public async Task IF_last_known_location_is_null_SHOULD_trace()
            {
                //Arrange
                MockProxy.Where_GetLastKnownLocationAsync_returns(null);

                //Act
                Sut.Connect(_requirements).Subscribe(next => { }, ex => { ExceptionTaskCompletionSource.SetResult(ex); });

                //Assert
                MockAnalyticsService.VerifyTrace("Last known location is null");
            }

            [Test]
            public async Task IF_last_known_location_is_invalid_SHOULD_error()
            {
                //Arrange
                MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(-212, 22, DateTimeOffset.Now));

                //Act
                Sut.Connect(_requirements).Subscribe(next => { }, ex => { ExceptionTaskCompletionSource.SetResult(ex); });
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
                Sut.Connect(_requirements).Subscribe(next => { }, ex => { ExceptionTaskCompletionSource.SetResult(ex); });
                var exception = await ExceptionTaskCompletionSource.Task;

                //Assert
                Assert.That(exception.ToError(), Is.EqualTo(GeolocationErrors.Unexpected));
                Assert.That(exception.InnerException.Message, Is.EqualTo("oops"));
                MockAnalyticsService.VerifyLogException<Exception>("oops");
            }
        }

        public class CurrentLocation : ConnectTests
        {
            
            public override void Setup()
            {
                base.Setup();

                MockProxy.Where_GetLastKnownLocationAsync_returns(null);
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(12, 22, DateTimeOffset.Now));
            }

            [Test]
            public async Task SHOULD_publish_current_location_and_trace()
            { 
                //Act
                Sut.Connect(_requirements)
                    .Subscribe(next => { LocationTaskCompletionSource.SetResult(new List<IGpsLocation> {next}); });
                
                var locations = await LocationTaskCompletionSource.Task;

                //Assert
                Assert.That(locations[0].Latitude, Is.EqualTo(12));
                Assert.That(locations[0].Longitude, Is.EqualTo(22));
                MockAnalyticsService.VerifyTrace("Current location published");
            }
            
            [Test]
            public async Task IF_current_location_is_null_SHOULD_warn()
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_returns(null);

                //Act
                Sut.Connect(_requirements).Subscribe(next => { }, ex => { ExceptionTaskCompletionSource.SetResult(ex); });

                //Assert
                MockAnalyticsService.VerifyTrace("Current location is null", LogSeverity.Warning);
            }

            [Test]
            public async Task IF_current_location_is_invalid_SHOULD_error()
            {
                //Arrange
                MockProxy.Where_GetLastKnownLocationAsync_returns(new Location(-212, 22, DateTimeOffset.Now));

                //Act
                Sut.Connect(_requirements).Subscribe(next => { }, ex => { ExceptionTaskCompletionSource.SetResult(ex); });
                var exception = await ExceptionTaskCompletionSource.Task;

                //Assert
                Assert.That(exception.ToError(), Is.EqualTo(GeolocationErrors.InvalidLatitude));
                MockAnalyticsService.VerifyTrace(GeolocationErrors.InvalidLatitude.ToString(), LogSeverity.Error);
            }

            [Test]
            public async Task IF_current_location_throws_SHOULD_error()
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_throws(new Exception("oops"));

                //Act
                Sut.Connect(_requirements).Subscribe(next => { }, ex => { ExceptionTaskCompletionSource.SetResult(ex); });
                var exception = await ExceptionTaskCompletionSource.Task;

                //Assert
                Assert.That(exception.ToError(), Is.EqualTo(GeolocationErrors.Unexpected));
                Assert.That(exception.InnerException.Message, Is.EqualTo("oops"));
                MockAnalyticsService.VerifyLogException<Exception>("oops");
            }
            
            [TestCase(LocationAccuracy.High, GeolocationAccuracy.Best)]
            [TestCase(LocationAccuracy.Medium, GeolocationAccuracy.Medium)]
            [TestCase(LocationAccuracy.Low, GeolocationAccuracy.Low)]
            public async Task SHOULD_use_given_accuracy(LocationAccuracy given, GeolocationAccuracy required)
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(22, 66, DateTimeOffset.Now));

                //Act
                Sut.Connect(new GeolocationRequirements(TimeSpan.FromTicks(1), given))
                    .Subscribe(next => { LocationTaskCompletionSource.SetResult(new List<IGpsLocation> {next}); });
                await LocationTaskCompletionSource.Task;

                //Assert
                MockProxy.Mock.Verify(x => x.GetCurrentLocationAsync(It.Is<GeolocationRequest>(y => y.DesiredAccuracy == required)));
            }

        }

        public class UpdatedLocation : ConnectTests
        {
            
            public override void Setup()
            {
                base.Setup();

                MockProxy.Where_GetLastKnownLocationAsync_returns(null); 
            }
             
            [Test]
            public async Task SHOULD_get_and_publish_current_location_again_after_interval_elapses()
            {
                //Arrange
                MockProxy.Where_GetCurrentLocationAsync_returns_sequence(new List<Location>
                {
                    new Location(0,2),
                    new Location(1,2),
                    new Location(2,4),
                    new Location(3,6),
                }); 
                var result1 = new TaskCompletionSource<IGpsLocation>();
                var result2 = new TaskCompletionSource<IGpsLocation>();
                var result3 = new TaskCompletionSource<IGpsLocation>();
                var count = 0;

                //Act
                var disposable =  Sut.Connect(_requirements)
                    .Subscribe(next =>
                    {
                        count++;
                        if(count == 1) result1.SetResult(next);
                        else if(count == 2) result2.SetResult(next);
                        else if(count == 3) result3.SetResult(next);
                    });

                //Assert
                _testScheduler.AdvanceBy(1000);
                var first = await result1.Task;
                Assert.That(first.Latitude, Is.EqualTo(0));
                
                _testScheduler.AdvanceBy(1000);
                var second = await result2.Task;
                Assert.That(second.Latitude, Is.EqualTo(1));
                
                _testScheduler.AdvanceBy(1000);
                var third = await result3.Task;
                Assert.That(third.Latitude, Is.EqualTo(2));
                 
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "Updated location published", LogSeverity.Verbose, 
                    It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()), Times.Exactly(3));
                disposable.Dispose();
            }
            
            [Test]
            public async Task IF_location_throws_exception_SHOULD_error()
            {
                //Arrange
                var result1 = new TaskCompletionSource<IGpsLocation>();
                var result2 = new TaskCompletionSource<Exception>(); 

                //Act
                var disposable =  Sut.Connect(_requirements)
                    .Subscribe(next =>
                    {
                        result1.SetResult(next);
                    }, ex =>
                    {
                        result2.SetResult(ex);
                    });

                //Assert
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(0,2)); 
                _testScheduler.AdvanceBy(1000);
                var first = await result1.Task; 
                Assert.That(first.Latitude, Is.EqualTo(0));
                
                MockProxy.Where_GetCurrentLocationAsync_throws(new Exception("oops")); 
                _testScheduler.AdvanceBy(1000);
                var second = await result2.Task;

                Assert.That(second.ToError(), Is.EqualTo(GeolocationErrors.Unexpected));
                Assert.That(second.InnerException.Message, Is.EqualTo("oops"));
                MockAnalyticsService.VerifyLogException<Exception>("oops");
                disposable.Dispose();
            }

            [Test]
            public async Task IF_location_returned_is_invalid_SHOULD_error()
            {
                //Arrange 
                var result1 = new TaskCompletionSource<IGpsLocation>();
                var result2 = new TaskCompletionSource<Exception>(); 

                //Act
                var disposable =  Sut.Connect(_requirements)
                    .Subscribe(next =>
                    {
                        result1.SetResult(next);
                    }, ex =>
                    {
                        result2.SetResult(ex);
                    });

                //Assert
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(0,2)); 
                _testScheduler.AdvanceBy(1000);
                var first = await result1.Task; 
                Assert.That(first.Latitude, Is.EqualTo(0));
                
                MockProxy.Where_GetCurrentLocationAsync_returns(new Location(0, 2222, DateTimeOffset.Now));
                _testScheduler.AdvanceBy(1000);
                var second = await result2.Task;
                
                Assert.That(second.ToError(), Is.EqualTo(GeolocationErrors.InvalidLongitude));
                MockAnalyticsService.VerifyTrace(GeolocationErrors.InvalidLongitude.ToString(), LogSeverity.Error);
                disposable.Dispose();
            }
            
            [Test]
            public async Task IF_location_returned_is_null_SHOULD_warn()
            { 

                //Act
                var disposable =  Sut.Connect(_requirements)
                    .Subscribe(next => { });

                //Assert
                MockProxy.Where_GetCurrentLocationAsync_returns(null);  
                _testScheduler.AdvanceBy(1000);
                MockAnalyticsService.VerifyTrace("Updated location is null", LogSeverity.Warning);
                disposable.Dispose();
            }

        }
    }
}