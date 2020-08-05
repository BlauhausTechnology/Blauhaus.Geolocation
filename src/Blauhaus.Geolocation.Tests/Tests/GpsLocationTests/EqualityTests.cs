using Blauhaus.Geolocation.Abstractions;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
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
            Assert.That(one.GetHashCode(), Is.EqualTo(two.GetHashCode())); 
        }

        [Test]
        public void Equality_SHOULD_return_false_when_lat_or_long_are_different()
        {
            var one = new GpsLocation(1, 2);
            var two = new GpsLocation(1, 3);

            //Assert
            Assert.That(one, Is.Not.EqualTo(two));
            Assert.That(one.Equals(two), Is.False);
            Assert.That(one == two, Is.False);
            Assert.That(one != two, Is.True); 
            Assert.That(one.GetHashCode(), Is.Not.EqualTo(two.GetHashCode())); 
        }
        
    }
}