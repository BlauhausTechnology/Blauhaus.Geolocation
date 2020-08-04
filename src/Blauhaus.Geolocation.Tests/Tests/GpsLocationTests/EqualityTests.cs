using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Tests.Tests._Base;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.GpsLocationTests
{
    public class EqualityTests 
    {
        [Test]
        public void Equality_SHOULD_return_true_when_lat_and_long_are_same()
        {
            var one = new GpsLocation(1, 2);
            var two = new GpsLocation(1, 2);

            //Assert
            Assert.That(one, Is.EqualTo(two));
            Assert.That(one.Equals(two), Is.True);
            Assert.That(one == two, Is.True);
            Assert.That(one != two, Is.False);
        }
        
    }
}