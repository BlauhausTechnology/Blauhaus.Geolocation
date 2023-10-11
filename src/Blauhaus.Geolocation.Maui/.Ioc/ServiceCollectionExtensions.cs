using Blauhaus.Geolocation.Abstractions.Service;
using Blauhaus.Geolocation.Maui.Proxy;
using Blauhaus.Reactive._Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Geolocation.Maui.Ioc
{
    public static class ServiceCollectionExtensions 
    {
        public static IServiceCollection AddGeolocation(this IServiceCollection services)
        {
            services.AddSingleton<IGeolocationService, GeolocationService>();
            services.AddTransient<IGeolocationProxy, MauiEssentialsGeolocationProxy>();
            services.AddTransient<IGeolocationPublisher, GeolocationPublisher>();
            services.AddReactiveScheduler();
            return services;
        }
    }
}