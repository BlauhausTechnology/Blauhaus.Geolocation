﻿using System;
using System.Threading.Tasks;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Responses;
using CSharpFunctionalExtensions;

namespace Blauhaus.Geolocation.Abstractions.LocationConverter
{
    public interface IGeolocationConverter
    {
        Task<Response<Address>> ToAddressAsync(IGpsLocation gpsLocation);
        Task<Response<IGpsLocation>> FromAddressAsync(string address);
    }
}