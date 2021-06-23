using Blauhaus.Geolocation.TestHelpers;
using NUnit.Framework;

namespace Blauhaus.Geolocation.Tests.Tests.AddressTests
{
    public class ToStringTests
    {
        [Test]
        public void SHOULD_add_all_fields_excluding_countryCode_separated_by_comma()
        {
            //Arrange
            var result = new AddressBuilder()
                .With_PlaceName("Top Floor Flat")
                .With_StreetNumber("5")
                .With_StreetName("Windward Turn")
                .With_Suburb("Atlantic Beach Golf Estate")
                .With_County("Blaauwbergstrand")
                .With_City("Cape City")
                .With_State("Western Cape")
                .With_PostalCode("7441")
                .With_Country("South Africa")
                .With_CountryCode("ZA")
                    .Object.ToString(); 
             
            //Assert
            Assert.That(result, Is.EqualTo("Top Floor Flat, 5 Windward Turn, Atlantic Beach Golf Estate, Blaauwbergstrand, Cape City, Western Cape, 7441, South Africa"));
        }
        
        [Test]
        public void IF_PlaceName_and_StreetNumber_are_same_SHOULD_omit_placename()
        {
            //Arrange
            var result = new AddressBuilder()
                .With_PlaceName("5")
                .With_StreetNumber("5")
                .With_StreetName("Windward Turn")
                .With_Suburb("Atlantic Beach Golf Estate")
                .With_County("")
                .With_City("Cape City")
                .With_State("Western Cape")
                .With_PostalCode("")
                .With_Country("South Africa")
                .With_CountryCode("ZA")
                .Object.ToString(); 
             
            //Assert
            Assert.That(result, Is.EqualTo("5 Windward Turn, Atlantic Beach Golf Estate, Cape City, Western Cape, South Africa"));
        }
        
        [Test]
        public void SHOULD_omit_missing_fields()
        {
            //Arrange
            var result = new AddressBuilder()
                .With_PlaceName("")
                .With_StreetNumber("5")
                .With_StreetName("Windward Turn")
                .With_Suburb("Atlantic Beach Golf Estate")
                .With_County("")
                .With_City("Cape City")
                .With_State("Western Cape")
                .With_PostalCode("")
                .With_Country("South Africa")
                .With_CountryCode("ZA")
                .Object.ToString(); 
             
            //Assert
            Assert.That(result, Is.EqualTo("5 Windward Turn, Atlantic Beach Golf Estate, Cape City, Western Cape, South Africa"));
        }

        [Test]
        public void IF_address_includes_street_number_and_name_SHOULD_append()
        {
            //Arrange
            var result = new AddressBuilder()
                .With_PlaceName("5")
                .With_StreetNumber("5")
                .With_StreetName("Windward Turn")
                    .Object.ToString(); 
             
            //Assert
            Assert.That(result.StartsWith("5 Windward Turn"));
        }


         
    }
}