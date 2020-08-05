using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.TestHelpers.Builders._Base;

namespace Blauhaus.Geolocation.TestHelpers
{
    public class AddressBuilder : BaseBuilder<AddressBuilder, Address>
    {
        private string _countryCode;
        private string _country;
        private string _postalCode;
        private string _state;
        private string _county;
        private string _city;
        private string _suburb;
        private string _streetName;
        private string _streetNumber;
        private string _placeName;

        public AddressBuilder()
        {
            var randomNumber = Random.Next(0, 100).ToString();
            _countryCode = "ZA";
            _country = "South Africa";
            _postalCode = randomNumber;
            _state = "Western Cape";
            _county = "County " +randomNumber;
            _city = "City " + randomNumber;
            _suburb = "Suburb " + randomNumber;
            _streetName = "Street";
            _streetNumber = $"{randomNumber}";
            _placeName = "Place " + randomNumber;
        }

        public AddressBuilder With_All_Empty()
        {
            _countryCode = string.Empty;
            _country = string.Empty;
            _postalCode = string.Empty;
            _state = string.Empty;
            _county = string.Empty;
            _city = string.Empty;
            _suburb = string.Empty;
            _streetName = string.Empty;
            _streetNumber = string.Empty;
            return this;
        }

        public AddressBuilder With_CountryCode(string value)
        {
            _countryCode = value;
            return this;
        }
        public AddressBuilder With_Country(string value)
        {
            _country = value;
            return this;
        }
        public AddressBuilder With_PostalCode(string value)
        {
            _postalCode = value;
            return this;
        }
        public AddressBuilder With_State(string value)
        {
            _state = value;
            return this;
        }
        public AddressBuilder With_County(string value)
        {
            _county = value;
            return this;
        }
        public AddressBuilder With_City(string value)
        {
            _city = value;
            return this;
        }
        public AddressBuilder With_Suburb(string value)
        {
            _suburb = value;
            return this;
        }
        public AddressBuilder With_StreetName(string value)
        {
            _streetName = value;
            return this;
        }
        public AddressBuilder With_StreetNumber(string value)
        {
            _streetNumber = value;
            return this;
        }
        public AddressBuilder With_PlaceName(string value)
        {
            _placeName = value;
            return this;
        }

        protected override Address Construct()
        {
            return new Address(
                _countryCode,
                _country,
                _postalCode,
                _state,
                _county,
                _city,
                _suburb,
                _streetName,
                _streetNumber,
                _placeName);

        }
    }
}