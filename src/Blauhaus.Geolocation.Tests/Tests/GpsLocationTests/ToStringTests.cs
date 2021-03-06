﻿using System.Globalization;
using System.Threading;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.GpsLocationTests
{
    public class ToStringTests 
    {
        
        [Test]
        public void SHOULD_serialize_ignoring_culture()
        { 
            //Arrange
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var gpsLoaction = new GpsLocation(10.11212d, 10.11212d);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act 
            var result = gpsLoaction.ToString();

            //Assert 
            Assert.That(result, Is.EqualTo("10.11212, 10.11212")); 
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
         
    }
}