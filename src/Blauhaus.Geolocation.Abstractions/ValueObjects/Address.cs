using Blauhaus.Common.ValueObjects._Base;

namespace Blauhaus.Geolocation.Abstractions.ValueObjects
{
    public class Address : BaseValueObject<Address>
    {
        public Address(
            string countryCode, 
            string countryName, 
            string province, 
            string region, 
            string town, 
            string suburb, 
            string postalCode, 
            string streetName, 
            string streetNumber)
        {
            CountryCode = countryCode;
            CountryName = countryName;
            Province = province;
            Region = region;
            Town = town;
            Suburb = suburb;
            PostalCode = postalCode;
            StreetName = streetName;
            StreetNumber = streetNumber;
        }

        public string CountryCode { get; }
        public string CountryName { get; }
        public string Province { get; }
        public string Region { get; }
        public string Town { get; }
        public string Suburb { get; }
        public string PostalCode { get; }
        public string StreetName { get; }
        public string StreetNumber { get; }


        protected override int GetHashCodeCore()
        {
            return CountryCode.GetHashCode() ^
                   CountryName.GetHashCode() ^
                   Province.GetHashCode() ^
                   Region.GetHashCode() ^
                   Town.GetHashCode() ^
                   Suburb.GetHashCode() ^
                   PostalCode.GetHashCode() ^
                   StreetName.GetHashCode() ^
                   StreetNumber.GetHashCode();
        }

        protected override bool EqualsCore(Address other)
        {
            return CountryCode == other.CountryCode &&
                   CountryName == other.CountryName &&
                   Province == other.Province &&
                   Region == other.Region &&
                   Town == other.Town &&
                   Suburb == other.Suburb &&
                   PostalCode == other.PostalCode&&
                   StreetName == other.StreetName &&
                   StreetNumber == other.StreetNumber;
        }
    }
}