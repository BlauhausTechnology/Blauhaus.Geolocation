﻿using System;
using Blauhaus.Common.ValueObjects._Base;
using Blauhaus.Errors.Extensions;
using CSharpFunctionalExtensions;

namespace Blauhaus.Geolocation.Abstractions
{
    public class GpsLocation : BaseValueObject<GpsLocation>, IGpsLocation
    {

        private GpsLocation()
        {}

        public GpsLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Result<GpsLocation> Create(double latitude, double longitude)
        {
            if (latitude > 90 || latitude < -90)
                return GeolocationErrors.InvalidLatitude.ToResult<GpsLocation>(); 

            if (longitude > 180 || longitude < -180)
                return GeolocationErrors.InvalidLongitude.ToResult<GpsLocation>();

            return new GpsLocation(latitude, longitude);
        }

        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public double GetMetresFrom(GpsLocation otherLocation)
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

        protected override bool EqualsCore(GpsLocation other)
        {
            return other.Latitude == Latitude && other.Longitude == Longitude;
        }

        public static bool operator ==(GpsLocation left, GpsLocation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GpsLocation left, GpsLocation right)
        {
            return !Equals(left, right);
        }


        public override string ToString()
        {
            return $"Lat: {Latitude} | Long: {Longitude}";
        }
         

    }
}