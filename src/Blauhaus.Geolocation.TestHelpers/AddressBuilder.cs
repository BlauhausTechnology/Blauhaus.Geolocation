using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.TestHelpers.Builders._Base;

namespace Blauhaus.Geolocation.TestHelpers
{
    public class AddressBuilder : BaseBuilder<AddressBuilder, Address>
    {
        private string _countryCode;
        private string _countryName;
        private string _province;
        private string _region;
        private string _town;
        private string _suburb;
        private string _postalCode;
        private string _streetName;
        private string _streetNumber;

        public AddressBuilder()
        {
            _countryCode = "ZA";
            _countryName = "South Africa";
            _province = "Western Cape";
            _region = "Region " + Random.Next(0, 1000);
            _town = "Town " + Random.Next(0, 1000);
            _suburb = "Suburb " + Random.Next(0, 1000);
            _postalCode = Random.Next(0, 1000).ToString();
            _streetName = "Street";
            _streetNumber = $"{Random.Next(0, 100)}";
        }

        public AddressBuilder With_CountryCode(string value)
        {
            _countryCode = value;
            return this;
        }
        public AddressBuilder With_CountryName(string value)
        {
            _countryName = value;
            return this;
        }
        public AddressBuilder With_Province(string value)
        {
            _province = value;
            return this;
        }
        public AddressBuilder With_Region(string value)
        {
            _region = value;
            return this;
        }
        public AddressBuilder With_Town(string value)
        {
            _town = value;
            return this;
        }
        public AddressBuilder With_Suburb(string value)
        {
            _suburb = value;
            return this;
        }
        public AddressBuilder With_PostalCode(string value)
        {
            _postalCode = value;
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

        protected override Address Construct()
        {
            return new Address(
                _countryCode,
                _countryName,
                _province,
                _region,
                _town,
                _suburb,
                _postalCode,
                _streetName,
                _streetNumber);

        }
    }
}