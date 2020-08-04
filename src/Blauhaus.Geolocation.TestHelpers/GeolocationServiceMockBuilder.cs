using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Blauhaus.Errors;
using Blauhaus.Geolocation.Abstractions;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Geolocation.TestHelpers
{
    public class GeolocationServiceMockBuilder : BaseMockBuilder<GeolocationServiceMockBuilder, IGeolocationService>
    {

        public GeolocationServiceMockBuilder()
        {
            Where_Connect_returns(new GpsLocation(1, 2));
        }

        public GeolocationServiceMockBuilder Where_Connect_returns(IGpsLocation location)
        {
            Mock.Setup(x => x.Connect(It.IsAny<GeolocationRequirements>()))
                .Returns(Observable.Create<IGpsLocation>(observer =>
                {
                    observer.OnNext(location);
                    return Disposable.Empty;
                }));
            return this;
        }

        public GeolocationServiceMockBuilder Where_Connect_returns_sequence(IEnumerable<IGpsLocation> locations)
        {
            var queue = new Queue<IGpsLocation>(locations);
            Mock.Setup(x => x.Connect(It.IsAny<GeolocationRequirements>()))
                .Returns(Observable.Create<IGpsLocation>(observer =>
                {
                    observer.OnNext(queue.Dequeue());
                    return Disposable.Empty;
                }));
            return this;
        }

        public GeolocationServiceMockBuilder Where_Connect_returns_exception(Exception e)
        {
            Mock.Setup(x => x.Connect(It.IsAny<GeolocationRequirements>()))
                .Returns(Observable.Create<IGpsLocation>(observer =>
                {
                    observer.OnError(e);
                    return Disposable.Empty;
                }));
            return this;
        }
        
        public GeolocationServiceMockBuilder Where_Connect_returns_error(Error e)
        {
            Mock.Setup(x => x.Connect(It.IsAny<GeolocationRequirements>()))
                .Returns(Observable.Create<IGpsLocation>(observer =>
                {
                    observer.OnError(new ErrorException(e));
                    return Disposable.Empty;
                }));
            return this;
        }


        
        public Action<IGpsLocation> Where_Location_can_be_updated()
        {
            Mock.Setup(x => x.Connect(It.IsAny<GeolocationRequirements>()))
                .Returns(Observable.Create<IGpsLocation>(observer =>
                {
                    void Update(object sender, LocationUpdatedEventArgs args)
                    {
                        observer.OnNext(args.NewLocation);
                    }

                    var updateSubscription = Observable.FromEventPattern<LocationUpdatedEventArgs>(
                        x => UpdateEvent += Update,
                        x => UpdateEvent -= Update).Subscribe();

                    return new CompositeDisposable(updateSubscription);
                }));

            return x => UpdateEvent?.Invoke(this, new LocationUpdatedEventArgs(x));
        }
        
        private event EventHandler<LocationUpdatedEventArgs> UpdateEvent;
        private class LocationUpdatedEventArgs : EventArgs
        {
            public LocationUpdatedEventArgs(IGpsLocation newLocation)
            {
                NewLocation = newLocation;
            }

            public IGpsLocation NewLocation { get; }
        }

    }
}