using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Proxy;
using Blauhaus.Reactive._Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Geolocation._Ioc
{
    public static class ServiceCollectionExtensions 
    {
        public static IServiceCollection AddGeolocation(this IServiceCollection services)
        {
            services.AddSingleton<IGeolocationService, GeolocationService>();
            services.AddTransient<IGeolocationProxy, XamarinEssentialsGeolocationProxy>();
            services.AddReactiveScheduler();
            return services;
        }
    }
}