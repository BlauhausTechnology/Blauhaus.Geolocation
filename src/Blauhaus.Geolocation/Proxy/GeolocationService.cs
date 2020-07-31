using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Results;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.Errors;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions;

namespace Blauhaus.Geolocation.Proxy
{
    public class GeolocationService : IGeolocationService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IDevicePermissionsService _devicePermissionsService;
        private readonly IGeolocationProxy _proxy;

        public GeolocationService(
            IAnalyticsService analyticsService,
            IDevicePermissionsService devicePermissionsService,
            IGeolocationProxy proxy)
        {
            _analyticsService = analyticsService;
            _devicePermissionsService = devicePermissionsService;
            _proxy = proxy;
        }

        public IObservable<IGpsLocation> Connect(TimeSpan interval)
        {
            return Observable.Create<IGpsLocation>(async observer =>
            {
                var disposable = new CompositeDisposable();

                try
                {
                    var lastKnownLocation = await _proxy.GetLastKnownLocationAsync();
                    if (lastKnownLocation != null)
                    {
                        var location = GpsLocation.Create(lastKnownLocation.Latitude, lastKnownLocation.Longitude);
                        if (location.IsFailure)
                        {
                            _analyticsService.Trace(this, location.Error.ToError().ToString(), LogSeverity.Error, lastKnownLocation.ToObjectDictionary());
                            observer.OnError(new ErrorException(location.Error.ToError()));
                        }
                        else
                        {
                            observer.OnNext(location.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    _analyticsService.LogException(this, e);
                    observer.OnError(e);
                }

                return disposable;
            });
        }
    }
}