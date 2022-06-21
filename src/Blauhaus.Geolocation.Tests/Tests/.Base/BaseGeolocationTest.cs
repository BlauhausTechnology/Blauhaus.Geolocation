using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.DeviceServices.Abstractions.Thread;
using Blauhaus.DeviceServices.TestHelpers.Dummies;
using Blauhaus.DeviceServices.TestHelpers.MockBuilders;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Geolocation.Tests.MockBuilders;
using Blauhaus.Reactive.TestHelpers._Ioc;
using Blauhaus.Reactive.TestHelpers.MockBuilders;
using Blauhaus.TestHelpers.BaseTests;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.Base
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

            AddService(MockLogger.Object);
            AddService(MockProxy.Object);
            AddService(MockDevicePermissionsService.Object);
            AddService(MockReactiveSchedulers.Object);
            AddService<IThreadService>(x => new DummyThreadService());
        }

        protected AnalyticsLoggerMockBuilder<TSut> MockLogger => AddMock<AnalyticsLoggerMockBuilder<TSut>, IAnalyticsLogger<TSut>>().Invoke();
        protected GeolocationProxyMockBuilder MockProxy => AddMock<GeolocationProxyMockBuilder, IGeolocationProxy>().Invoke();
        protected DevicePermissionsServiceMockBuilder MockDevicePermissionsService => AddMock<DevicePermissionsServiceMockBuilder, IDevicePermissionsService>().Invoke();
        protected ReactiveSchedulersMockBuilder MockReactiveSchedulers => Mocks.AddMockReactiveSchedulers();

    }
}