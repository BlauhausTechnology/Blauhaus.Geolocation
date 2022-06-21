using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
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
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation
{
    public class GeolocationService : IGeolocationService
    {
        private readonly IAnalyticsLogger<GeolocationService> _logger;
        private readonly IDevicePermissionsService _devicePermissionsService;
        private readonly IReactiveSchedulers _schedulers;
        private readonly IGeolocationProxy _proxy;

        public GeolocationService(
            IAnalyticsLogger<GeolocationService> logger,
            IDevicePermissionsService devicePermissionsService,
            IReactiveSchedulers schedulers,
            IGeolocationProxy proxy)
        {
            _logger = logger;
            _devicePermissionsService = devicePermissionsService;
            _schedulers = schedulers;
            _proxy = proxy;
        }

        public IObservable<IGpsLocation> Connect(GeolocationRequirements requirements)
        {

            var interval = requirements.UpdateInterval;
            var requiredAccuracy = requirements.Accuracy;

            _logger.LogDebug("New GpsLocation connection requested with accuracy {GeolocationAccuracy} and interval {UpdateInterval}", requiredAccuracy, interval);

            return Observable.Create<IGpsLocation>(async observer =>
            {
                var permissions = await _devicePermissionsService.EnsurePermissionGrantedAsync(DevicePermission.LocationWhenInUse);
                if (permissions.IsFailure)
                {
                    _logger.LogWarning("Failed to access GpsLocation due to permissions failure {Error}", permissions.Error);
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
                    return _logger.LogErrorResponse<IGpsLocation>(permissions.Error);
                }

                var lastKnownLocation = await _proxy.GetLastKnownLocationAsync();
                if (lastKnownLocation != null)
                {
                    var lastLocation = GpsLocation.Create(lastKnownLocation.Latitude, lastKnownLocation.Longitude);
                    if (lastLocation.IsFailure)
                    {
                        return _logger.LogErrorResponse<IGpsLocation>(lastLocation.Error); 
                    }

                    _logger.LogDebug("Last known location returned: {@Location}", lastLocation.Value);
                    return Response.Success(lastLocation.Value);
                }

                var currentLocation = await _proxy.GetCurrentLocationAsync(accuracy.ToGeoLocationRequest());

                var currentGpsLocation = GpsLocation.Create(currentLocation.Latitude, currentLocation.Longitude);
                if (currentGpsLocation.IsFailure)
                {
                    return _logger.LogErrorResponse<IGpsLocation>(currentGpsLocation.Error); 
                }

                _logger.LogTrace("Current location returned: {@Location}", currentGpsLocation);
                return Response.Success(currentGpsLocation.Value);

            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse<IGpsLocation>(GeolocationError.Unexpected, e);
            }
        }

        private async Task PublishLastKnownLocationAsync(IObserver<IGpsLocation> observer)
        {
            try
            {
                var lastKnownLocation = await _proxy.GetLastKnownLocationAsync();
                if (lastKnownLocation == null)
                {
                    _logger.LogDebug("Last known location is null");
                }
                else
                {
                    var lastLocation = GpsLocation.Create(lastKnownLocation.Latitude, lastKnownLocation.Longitude);
                    if (lastLocation.IsFailure)
                    {
                        _logger.LogError(lastLocation.Error);
                        observer.OnError(new ErrorException(lastLocation.Error));
                    }
                    else
                    {
                        _logger.LogTrace("Last known location published: {@Location}", lastLocation.Value);
                        observer.OnNext(lastLocation.Value);
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError(GeolocationError.Unexpected, e);
                observer.OnError(new ErrorException(GeolocationError.Unexpected, e));
            }
        }

        private async Task PublishCurrentLocationAsync(GeolocationRequest geolocationRequest, IObserver<IGpsLocation> observer, string description)
        {
            try
            {
                var currentLocation = await _proxy.GetCurrentLocationAsync(geolocationRequest);
                if (currentLocation == null)
                {
                    _logger.LogWarning($"{description} location is null");
                }
                else
                {
                    var currentGpsLocation = GpsLocation.Create(currentLocation.Latitude, currentLocation.Longitude);
                    if (currentGpsLocation.IsFailure)
                    {
                        _logger.LogError( currentGpsLocation.Error);
                        observer.OnError(new ErrorException(currentGpsLocation.Error));
                    }
                    else
                    {
                        _logger.LogTrace($"{description} location published");
                        observer.OnNext(currentGpsLocation.Value);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(GeolocationError.Unexpected, e);
                observer.OnError(new ErrorException(GeolocationError.Unexpected, e));
            }
            
        }
         
    }
}