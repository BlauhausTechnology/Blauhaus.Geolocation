using Blauhaus.Geolocation.Abstractions.ValueObjects;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.AddressTests
{
    public class EqualityTests
    {
        [Test]
        public void SHOULD_return_true_WHEN_all__fields_equal()
        {
            //Arrange
            var one = new Address("1", "2", "3", "4", "5", "6", "7", "8", "9", "A");
            var two = new Address("1", "2", "3", "4", "5", "6", "7", "8", "9", "A");

            //Assert
            Assert.That(one, Is.EqualTo(two));
            Assert.That(one.Equals(two), Is.True);
            Assert.That(one == two, Is.True);
            Assert.That(one != two, Is.False);
            Assert.That(one.GetHashCode(), Is.EqualTo(two.GetHashCode()));
        }

        [Test]
        public void SHOULD_return_false_WHEN_any_field_different()
        {
            //Arrange
            var one = new Address("1", "1", "1", "1", "1", "1", "1", "1", "1", "1");

            //Assert
            Assert.That(one, Is.Not.EqualTo(new Address("2", "1", "1", "1", "1", "1", "1", "1", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "2", "1", "1", "1", "1", "1", "1", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "2", "1", "1", "1", "1", "1", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "1", "2", "1", "1", "1", "1", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "1", "1", "2", "1", "1", "1", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "1", "1", "1", "2", "1", "1", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "1", "1", "1", "1", "2", "1", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "1", "1", "1", "1", "1", "2", "1", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "1", "1", "1", "1", "1", "1", "2", "1")));
            Assert.That(one, Is.Not.EqualTo(new Address("1", "1", "1", "1", "1", "1", "1", "1", "1", "2")));
        }
    }
}