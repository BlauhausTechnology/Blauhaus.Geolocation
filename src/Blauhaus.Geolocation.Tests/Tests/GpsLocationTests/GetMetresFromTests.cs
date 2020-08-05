using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.Geolocation.Tests.Tests._Base;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.GpsLocationTests
{
    public class GetMetresFromTests 
    {
        [Test]
        public void SHOULD_return_great_circle_distance()
        {
            //Arrange
            var one = new GpsLocation(-33.7468433, 18.4568598);
            var two = new GpsLocation(-33.9122294, 18.433351);

            //Act
            var result = one.GetMetresFrom(two);

            //Assert
            Assert.That(result, Is.EqualTo(18533).Within(1));
        }
         
        
    }
}