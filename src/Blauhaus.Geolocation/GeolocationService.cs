using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.Errors;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Extensions;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Reactive.Abstractions.Schedulers;

namespace Blauhaus.Geolocation
{
    public class GeolocationService : IGeolocationService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IDevicePermissionsService _devicePermissionsService;
        private readonly IReactiveSchedulers _schedulers;
        private readonly IGeolocationProxy _proxy;

        public GeolocationService(
            IAnalyticsService analyticsService,
            IDevicePermissionsService devicePermissionsService,
            IReactiveSchedulers schedulers,
            IGeolocationProxy proxy)
        {
            _analyticsService = analyticsService;
            _devicePermissionsService = devicePermissionsService;
            _schedulers = schedulers;
            _proxy = proxy;
        }

        public IObservable<IGpsLocation> Connect(GeolocationRequirements requirements)
        {

            var interval = requirements.UpdateInterval;
            var requiredAccuracy = requirements.Accuracy;

            _analyticsService.TraceVerbose(this, $"New GpsLocation connection requested with accuracy {requiredAccuracy} and interval {interval}");

            return Observable.Create<IGpsLocation>(async observer =>
            {
                var permissions = await _devicePermissionsService.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse);
                if (permissions.IsFailure)
                {
                    _analyticsService.TraceWarning(this, "Failed to access GpsLocation due to permissions failure");
                    observer.OnError(new ErrorException(permissions));
                }

                var geolocationRequest = requiredAccuracy.ToGeoLocationRequest();

                var disposable = new CompositeDisposable();

                try
                {
                    var lastKnownLocation = await _proxy.GetLastKnownLocationAsync();
                    if (lastKnownLocation == null)
                    {
                        _analyticsService.Trace(this, "Last known location is null");
                    }
                    else
                    {
                        var lastLocation = GpsLocation.Create(lastKnownLocation.Latitude, lastKnownLocation.Longitude);
                        if (lastLocation.IsFailure)
                        {
                            _analyticsService.TraceError(this, lastLocation.Error.ToError().ToString(), lastKnownLocation.ToObjectDictionary());
                            observer.OnError(new ErrorException(lastLocation));
                        }
                        else
                        {
                            _analyticsService.Trace(this, "Last known location published");
                            observer.OnNext(lastLocation.Value);
                        }
                    }

                    var currentLocation = await _proxy.GetCurrentLocationAsync(geolocationRequest);
                    if (currentLocation == null)
                    {
                        _analyticsService.TraceWarning(this, "Current location is null");
                    }
                    else
                    {
                        var currentGpsLocation = GpsLocation.Create(currentLocation.Latitude, currentLocation.Longitude);
                        if (currentGpsLocation.IsFailure)
                        {
                            _analyticsService.TraceError(this, currentGpsLocation.Error.ToError().ToString(), currentLocation.ToObjectDictionary());
                            observer.OnError(new ErrorException(currentGpsLocation));
                        }
                        else
                        {
                            _analyticsService.Trace(this, "Current location published");
                            observer.OnNext(currentGpsLocation.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    _analyticsService.LogException(this, e);
                    observer.OnError(new ErrorException(GeolocationErrors.Unexpected, e));
                }

                var sub = Observable.Interval(interval, _schedulers.ThreadPool)
                    .Subscribe(async tick =>
                    {
                        try
                        {
                            var location = await _proxy.GetCurrentLocationAsync(requiredAccuracy.ToGeoLocationRequest());
                            if (location == null)
                            {
                                _analyticsService.TraceWarning(this, "Updated location is null");
                            }
                            else
                            {
                                var latestGpsLocation = GpsLocation.Create(location.Latitude, location.Longitude);
                                if (latestGpsLocation.IsFailure)
                                {
                                    _analyticsService.TraceError(this, latestGpsLocation.Error.ToError().ToString(), latestGpsLocation.ToObjectDictionary());
                                    observer.OnError(new ErrorException(latestGpsLocation));
                                }
                                else
                                {
                                    _analyticsService.Trace(this, "Updated location published");
                                    observer.OnNext(latestGpsLocation.Value);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _analyticsService.LogException(this, e);
                            observer.OnError(new ErrorException(GeolocationErrors.Unexpected, e));
                        }
                    });

                disposable.Add(sub);

                return disposable;
            });
        }
         
    }
}