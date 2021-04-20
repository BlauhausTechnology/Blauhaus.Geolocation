using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Reactive.Abstractions.Schedulers;
using Blauhaus.Reactive.Schedulers;

namespace Blauhaus.Geolocation.Ioc
{
    public static class IocServiceExtensions
    {
        public static IIocService AddGeolocation(this IIocService iocService)
        {
            iocService.RegisterImplementation<IGeolocationService, GeolocationService>(IocLifetime.Singleton);
            iocService.RegisterImplementation<IGeolocationProxy, XamarinEssentialsGeolocationProxy>();
            iocService.RegisterImplementation<IReactiveSchedulers, ReactiveSchedulers>();
            return iocService;
        }
    }
}