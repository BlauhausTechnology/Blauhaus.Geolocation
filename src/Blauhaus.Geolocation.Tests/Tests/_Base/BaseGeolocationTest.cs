using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.DeviceServices.Abstractions.Thread;
using Blauhaus.DeviceServices.TestHelpers.MockBuilders;
using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Geolocation.Tests.MockBuilders;
using Blauhaus.Reactive.TestHelpers._Ioc;
using Blauhaus.Reactive.TestHelpers.MockBuilders;
using Blauhaus.TestHelpers.BaseTests;
using NUnit.Framework;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Tests.Tests._Base
{
    public class BaseGeolocationTest<TSut> : BaseServiceTest<TSut> where TSut : class
    {

        protected TaskCompletionSource<List<IGpsLocation>> LocationTaskCompletionSource;
        protected TaskCompletionSource<Exception> ExceptionTaskCompletionSource;

        [SetUp]
        public virtual void Setup()
        {
            Cleanup();

            LocationTaskCompletionSource = new TaskCompletionSource<List<IGpsLocation>>();
            ExceptionTaskCompletionSource = new TaskCompletionSource<Exception>();

            AddService(MockProxy.Object);
            AddService(MockAnalyticsService.Object);
            AddService(MockDevicePermissionsService.Object);
            AddService(MockReactiveSchedulers.Object);
            AddService<IThreadService>(x => new FakeThreadService());
        }

        
        protected GeolocationProxyMockBuilder MockProxy => AddMock<GeolocationProxyMockBuilder, IGeolocationProxy>().Invoke();
        protected AnalyticsServiceMockBuilder MockAnalyticsService => AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();
        protected DevicePermissionsServiceMockBuilder MockDevicePermissionsService => AddMock<DevicePermissionsServiceMockBuilder, IDevicePermissionsService>().Invoke();
        protected ReactiveSchedulersMockBuilder MockReactiveSchedulers => Mocks.AddMockReactiveSchedulers();
        //protected ThreadServiceMockBuilder MockThreadService => Mocks.AddMock<ThreadServiceMockBuilder, IThreadService>().Invoke();

    }
}