using System;
using Blauhaus.Common.ValueObjects._Base;
using Blauhaus.Errors;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace Blauhaus.Geolocation.Abstractions.ValueObjects
{
    public class GpsLocation : BaseValueObject<IGpsLocation>, IGpsLocation
    {

        private GpsLocation()
        {}

        public GpsLocation(double latitude, double longitude)
        {
            if (latitude > 90 || latitude < -90)
                throw new ErrorException(GeolocationErrors.InvalidLatitude); 

            if (longitude > 180 || longitude < -180)
                throw new ErrorException(GeolocationErrors.InvalidLongitude); 

            Latitude = latitude;
            Longitude = longitude;
        }

        public static Result<IGpsLocation> Create(double latitude, double longitude)
        {
            if (latitude > 90 || latitude < -90)
                return GeolocationErrors.InvalidLatitude.ToResult<IGpsLocation>(); 

            if (longitude > 180 || longitude < -180)
                return GeolocationErrors.InvalidLongitude.ToResult<IGpsLocation>();

            return new GpsLocation(latitude, longitude);
        }

        public static Result<IGpsLocation> Parse(string serializedGpsLocation)
        {
            if (string.IsNullOrEmpty(serializedGpsLocation))
                return GeolocationErrors.InvalidSerialization.ToResult<IGpsLocation>();

            var coordinateStrings = serializedGpsLocation.Split(',');

            if(coordinateStrings.Length != 2)
                return GeolocationErrors.InvalidSerialization.ToResult<IGpsLocation>();

            if(!double.TryParse(coordinateStrings[0], out var latitude) || !double.TryParse(coordinateStrings[1], out var longitude))
                return GeolocationErrors.InvalidSerialization.ToResult<IGpsLocation>();

            return Create(latitude, longitude);

        }

        public static GpsLocation Default = new GpsLocation(0,0);

        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public double GetMetresFrom(IGpsLocation otherLocation)
        {
            var d1 = Latitude * (Math.PI / 180.0);
            var num1 = Longitude * (Math.PI / 180.0);
            var d2 = otherLocation.Latitude * (Math.PI / 180.0);
            var num2 = otherLocation.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        protected override int GetHashCodeCore()
        {
            return (Longitude.GetHashCode() * 397) ^ Latitude.GetHashCode();
        }

        protected override bool EqualsCore(IGpsLocation otherLocation)
        {
            const double epsilon = 0.000001; //Accuracy to roughly 10cm
            
            return Math.Abs(otherLocation.Latitude - Latitude) < epsilon &&
                   (Math.Abs(otherLocation.Longitude - Longitude) < epsilon);
        }
         

        public override string ToString()
        {
            return $"Lat: {Latitude} | Long: {Longitude}";
        }
         

    }
}