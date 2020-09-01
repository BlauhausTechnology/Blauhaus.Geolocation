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

        [Test]
        public void SHOULD_parse_valid_string()
        { 
            //Act
            var result = GpsLocation.Parse("1.001, -87.1");

            //Assert
            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Latitude, Is.EqualTo(1.001d));
            Assert.That(result.Value.Longitude, Is.EqualTo(-87.1d));
        }
         
    }
}