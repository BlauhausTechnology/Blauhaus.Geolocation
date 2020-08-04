using Blauhaus.Geolocation.Abstractions.ValueObjects;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.AddressTests
{
    public class ToStringTests
    {
        [Test]
        public void IF_address_includes_street_number_and_name_SHOULD_append()
        {
            //Arrange
            var result = new Address(
                countryCode: string.Empty, 
                countryName: string.Empty, 
                province: string.Empty, 
                region: string.Empty, 
                town: string.Empty, 
                suburb: string.Empty, 
                postalCode: string.Empty, 
                streetName: "Terrace Road", 
                streetNumber: "6")
                    .ToString(); 
             
            //Assert
            Assert.That(result.StartsWith("6 Terrace Road"));
        }


         
    }
}