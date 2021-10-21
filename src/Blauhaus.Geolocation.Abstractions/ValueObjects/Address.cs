using System.Text;
using Blauhaus.Common.ValueObjects.Base;

namespace Blauhaus.Geolocation.Abstractions.ValueObjects
{
    public class Address : BaseValueObject<Address>
    {
        public Address(
            string countryCode, 
            string country, 
            string postalCode, 
            string state, 
            string county, 
            string city, 
            string suburb, 
            string streetName, 
            string streetNumber, 
            string placeName)
        {
            CountryCode = countryCode;
            Country = country;
            PostalCode = postalCode;
            State = state;
            County = county;
            City = city;
            Suburb = suburb;
            StreetName = streetName;
            StreetNumber = streetNumber;
            PlaceName = placeName;
        }

        public string CountryCode { get; }
        public string Country { get; }
        public string PostalCode { get; }
        public string State { get; }
        public string County { get; }
        public string City { get; }
        public string Suburb { get; }
        public string StreetName { get; }
        public string StreetNumber { get; }
        public string PlaceName { get; }


        protected override int GetHashCodeCore()
        {
            return CountryCode.GetHashCode() ^
                   Country.GetHashCode() ^
                   PostalCode.GetHashCode() ^
                   State.GetHashCode() ^
                   County.GetHashCode() ^
                   City.GetHashCode() ^
                   Suburb.GetHashCode() ^
                   StreetName.GetHashCode() ^
                   StreetNumber.GetHashCode() ^
                   PlaceName.GetHashCode();
        }

        protected override bool EqualsCore(Address other)
        {
            return CountryCode == other.CountryCode &&
                   Country == other.Country &&
                   PostalCode == other.PostalCode &&
                   State == other.State &&
                   County == other.County &&
                   City == other.City &&
                   Suburb == other.Suburb &&
                   StreetName == other.StreetName &&
                   StreetNumber == other.StreetNumber &&
                   PlaceName == other.PlaceName;
        }

        public override string ToString()
        {
            var s = new StringBuilder();

            if (!string.IsNullOrEmpty(PlaceName) && PlaceName != StreetNumber)
                s.Append(PlaceName).Append(", ");
            
            s.Append($"{StreetNumber} {StreetName}, ");

            if (!string.IsNullOrEmpty(Suburb))
                s.Append(Suburb).Append(", ");
            
            if(!string.IsNullOrEmpty(County))
                s.Append(County).Append(", ");

            if(!string.IsNullOrEmpty(City))
                s.Append(City).Append(", ");
            
            if(!string.IsNullOrEmpty(State))
                s.Append(State).Append(", ");

            if(!string.IsNullOrEmpty(PostalCode))
                s.Append(PostalCode).Append(", ");

            if(!string.IsNullOrEmpty(Country))
                s.Append(Country).Append(", ");

            if (s[s.Length-2] == ',' && s[s.Length-1] == ' ')
                s.Length -= 2;

            return s.ToString();
        }

        public static Address Empty = new Address(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
    }
}