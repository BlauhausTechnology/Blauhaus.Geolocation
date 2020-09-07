using System;
using System.Globalization;
using System.Threading;
using Blauhaus.Errors.Extensions;
using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Abstractions.Errors;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Tests.Tests._Base;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.GpsLocationTests
{
    public class ParseTests 
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("1.2")]
        [TestCase("1;2")]
        [TestCase("1222")]
        [TestCase("80, 2000")]
        [TestCase("a, 1")]
        [TestCase("2, bbb")]
        [TestCase("-2000, 80")]
        public void IF_string_is_invalid_SHOULD_fail(string serializedLocation)
        { 
            //Act
            var result = GpsLocation.Parse(serializedLocation);

            //Assert
            Assert.That(result.IsFailure);
        }

        [TestCase("1.001, -87.1", 1.001d, -87.1d)]
        [TestCase("-33.7481709,18.4528183", -33.7481709d, 18.4528183d)]
        public void SHOULD_parse_valid_string(string parsedGpsLocation, double latitude, double longitude)
        { 
            //Act
            var result = GpsLocation.Parse(parsedGpsLocation);

            //Assert
            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Latitude, Is.EqualTo(latitude));
            Assert.That(result.Value.Longitude, Is.EqualTo(longitude));
        }

        [Test]
        public void SHOULD_parse_using_invariant_culture()
        { 
            //Arrange
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var gpsString = 10.11212d + "," + 10.11212d;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act 
            var result = GpsLocation.Parse(gpsString);

            //Assert
            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Latitude, Is.EqualTo(10.11212d));
            Assert.That(result.Value.Longitude, Is.EqualTo(10.11212d));
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
         
    }
}