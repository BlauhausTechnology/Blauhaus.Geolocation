using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.Thread;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Blauhaus.Geolocation
{ 
    public class GeolocationPublisher : BaseImmediatePublisher<IGpsLocation>, IGeolocationPublisher
    {
        private readonly IAnalyticsLogger<GeolocationPublisher> _logger;
        private readonly IThreadService _threadService;
        private GeolocationRequirements _geolocationRequirements = null!;
        private Timer? _timer;
        private GpsLocation? _currentLocation;

        public GeolocationPublisher(
            IAnalyticsLogger<GeolocationPublisher> logger,
            IThreadService threadService)
        {
            _logger = logger;
            _threadService = threadService;
        }
        public  Task InitializeAsync(GeolocationRequirements geolocationRequirements)
        {
            _geolocationRequirements = geolocationRequirements;
            return Task.CompletedTask;
        }

        public override async Task<IDisposable> SubscribeAsync(Func<IGpsLocation, Task> handler, Func<IGpsLocation, bool>? filter = null)
        {
            if (_geolocationRequirements == null)
            {
                throw new InvalidOperationException("GeolocationPublisher must be initialized before use");
            }

            var currentLocation = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
            if (currentLocation != null)
            {
                _currentLocation = new GpsLocation(currentLocation.Latitude, currentLocation.Longitude);
                _logger.LogTrace("Last known location: Lat {Latitude}, | Long {Longitude}", _currentLocation.Latitude, _currentLocation.Longitude);
            }
            
            _logger.LogDebug("Subscribing to {GeolocationAccuracy} geolocation updates with frequency {UpdateInternal}", 
                _geolocationRequirements.UpdateInterval,_geolocationRequirements.Accuracy );

            var disposable = await base.SubscribeAsync(handler, filter);
            
            _timer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(1), _geolocationRequirements.UpdateInterval);

            
             return new ActionDisposable(() =>
             {
                 _logger.LogTrace("Disposing GpsLocationPublisher");
                 _timer.Dispose();
                 disposable.Dispose();
             });
        }

        private async void TimerCallback(object state)
        {
            try
            {
                await _threadService.InvokeOnMainThreadAsync(async () =>
                {
                    var location =
                        await Xamarin.Essentials.Geolocation.GetLocationAsync() ??
                        await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
                        
                    if (location != null)
                    {
                        _currentLocation = new GpsLocation(location.Latitude, location.Longitude);
                        await UpdateSubscribersAsync(_currentLocation); 
                    }
                });
                
            }
            catch (Exception e)
            {              
                _logger.LogError(GeolocationError.Unexpected, e);
            }
        }

        protected override Task<IGpsLocation> GetAsync()
        {
            if (_currentLocation == null)
            {
                _logger.LogWarning("No gps location available");
                return Task.FromResult<IGpsLocation>(GpsLocation.Default);
            }
            _logger.LogTrace("Publishing current location Lat {Latitude}, | Long {Longitude}", _currentLocation.Latitude, _currentLocation.Longitude);
            return Task.FromResult<IGpsLocation>(_currentLocation);
        }
         
    }
}