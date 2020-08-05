using System;
using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Abstractions.Service;
using Xamarin.Essentials;

namespace Blauhaus.Geolocation.Extensions
{
    public static class LocationAccuracyExtensions
    {
        public static GeolocationRequest ToGeoLocationRequest(this LocationAccuracy accuracy)
        {
            GeolocationAccuracy requiredAccuracy;


            switch (accuracy)
            {
                case LocationAccuracy.High:
                    requiredAccuracy = GeolocationAccuracy.Best;
                    break;
                case LocationAccuracy.Medium:
                    requiredAccuracy = GeolocationAccuracy.Medium;
                    break;
                case LocationAccuracy.Low:
                    requiredAccuracy = GeolocationAccuracy.Low;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(accuracy), accuracy, null);
            }

            return new GeolocationRequest(requiredAccuracy);
        }
    }
}