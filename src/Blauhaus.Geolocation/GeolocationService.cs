using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Permissions;
using Blauhaus.Errors;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Extensions;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Reactive.Abstractions.Schedulers;
using Blauhaus.Responses;
using Xamarin.Essentials;

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
                    observer.OnError(new ErrorException(permissions.Error));
                }

                var geolocationRequest = requiredAccuracy.ToGeoLocationRequest();

                await PublishLastKnownLocationAsync(observer);
                await PublishCurrentLocationAsync(geolocationRequest, observer, "Current");
                
                var disposable = new CompositeDisposable();

                var sub = Observable.Interval(interval, _schedulers.ThreadPool)
                    .Subscribe(async tick =>
                    {
                        await PublishCurrentLocationAsync(geolocationRequest, observer, "Updated");
                    });

                disposable.Add(sub);

                return disposable;
            });
        }

        public async Task<Response<IGpsLocation>> GetCurrentLocationAsync(LocationAccuracy accuracy = LocationAccuracy.Medium)
        {
            try
            {
                var permissions = await _devicePermissionsService.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse);
                if (permissions.IsFailure)
                { 
                    return _analyticsService.TraceErrorResponse<IGpsLocation>(this, permissions.Error);
                }

                var lastKnownLocation = await _proxy.GetLastKnownLocationAsync();
                if (lastKnownLocation != null)
                {
                    var lastLocation = GpsLocation.Create(lastKnownLocation.Latitude, lastKnownLocation.Longitude);
                    if (lastLocation.IsFailure)
                    {
                        return _analyticsService.TraceErrorResponse<IGpsLocation>(this, lastLocation.Error); 
                    }

                    _analyticsService.TraceVerbose(this, "Last known location returned", lastLocation.Value.ToObjectDictionary());
                    return Response.Success(lastLocation.Value);
                }

                var currentLocation = await _proxy.GetCurrentLocationAsync(accuracy.ToGeoLocationRequest());

                var currentGpsLocation = GpsLocation.Create(currentLocation.Latitude, currentLocation.Longitude);
                if (currentGpsLocation.IsFailure)
                {
                    return _analyticsService.TraceErrorResponse<IGpsLocation>(this, currentGpsLocation.Error); 
                }

                _analyticsService.TraceVerbose(this, "Current location returned", currentGpsLocation.ToObjectDictionary());
                return Response.Success(currentGpsLocation.Value);

            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse<IGpsLocation>(this, e, GeolocationErrors.Unexpected);
            }
        }

        private async Task PublishLastKnownLocationAsync(IObserver<IGpsLocation> observer)
        {
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
                        _analyticsService.TraceError(this, lastLocation.Error, lastKnownLocation.ToObjectDictionary());
                        observer.OnError(new ErrorException(lastLocation.Error));
                    }
                    else
                    {
                        _analyticsService.Trace(this, "Last known location published");
                        observer.OnNext(lastLocation.Value);
                    }
                }

            }
            catch (Exception e)
            {
                _analyticsService.LogException(this, e);
                observer.OnError(new ErrorException(GeolocationErrors.Unexpected, e));
            }
        }

        private async Task PublishCurrentLocationAsync(GeolocationRequest geolocationRequest, IObserver<IGpsLocation> observer, string description)
        {
            try
            {
                var currentLocation = await _proxy.GetCurrentLocationAsync(geolocationRequest);
                if (currentLocation == null)
                {
                    _analyticsService.TraceWarning(this, $"{description} location is null");
                }
                else
                {
                    var currentGpsLocation = GpsLocation.Create(currentLocation.Latitude, currentLocation.Longitude);
                    if (currentGpsLocation.IsFailure)
                    {
                        _analyticsService.TraceError(this, currentGpsLocation.Error, currentLocation.ToObjectDictionary());
                        observer.OnError(new ErrorException(currentGpsLocation.Error));
                    }
                    else
                    {
                        _analyticsService.Debug($"{description} location published");
                        observer.OnNext(currentGpsLocation.Value);
                    }
                }
            }
            catch (Exception e)
            {
                _analyticsService.LogException(this, e);
                observer.OnError(new ErrorException(GeolocationErrors.Unexpected, e));
            }
            
        }
         
    }
}