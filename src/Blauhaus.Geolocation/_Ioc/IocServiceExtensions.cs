using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Ioc.Abstractions;

namespace Blauhaus.Geolocation._Ioc
{
    public static class IocServiceExtensions
    {
        public static IIocService AddGeolocation(this IIocService iocService)
        {
            iocService.RegisterImplementation<IGeolocationService, GeolocationService>(IocLifetime.Singleton);
            iocService.RegisterImplementation<IGeolocationProxy, XamarinEssentialsGeolocationProxy>();
            return iocService;
        }
    }
}