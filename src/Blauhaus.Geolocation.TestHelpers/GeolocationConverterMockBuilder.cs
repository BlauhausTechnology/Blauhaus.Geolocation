using Blauhaus.Errors;
using Blauhaus.Geolocation.Abstractions.LocationConverter;
using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.TestHelpers.MockBuilders;
using CSharpFunctionalExtensions;
using Moq;

namespace Blauhaus.Geolocation.TestHelpers
{
    public class GeolocationConverterMockBuilder : BaseMockBuilder<GeolocationConverterMockBuilder, IGeolocationConverter>
    {

        public GeolocationConverterMockBuilder()
        {
            Where_FromAddressAsync_returns(GpsLocation.Default);
            Where_ToAddressAsync_returns(Address.Empty);
        }

        public GeolocationConverterMockBuilder Where_FromAddressAsync_returns(IGpsLocation location, string address = null)
        {
            if (address == null)
            {
                Mock.Setup(x => x.FromAddressAsync(It.IsAny<string>()))
                    .ReturnsAsync(Result.Success(location));
            }
            else
            {
                Mock.Setup(x => x.FromAddressAsync(address))
                    .ReturnsAsync(Result.Success(location));
            }
            return this;
        }
        public GeolocationConverterMockBuilder Where_FromAddressAsync_fails(Error error)
        {
            Mock.Setup(x => x.FromAddressAsync(It.IsAny<string>()))
                .ReturnsAsync(Result.Failure<IGpsLocation>(error.ToString()));
            return this;
        }

        public GeolocationConverterMockBuilder Where_ToAddressAsync_returns(Address address)
        {
            Mock.Setup(x => x.ToAddressAsync(It.IsAny<GpsLocation>()))
                .ReturnsAsync(Result.Success(address));
            return this;
        }
        public GeolocationConverterMockBuilder Where_ToAddressAsync_fails(Error error)
        {
            Mock.Setup(x => x.ToAddressAsync(It.IsAny<GpsLocation>()))
                .ReturnsAsync(Result.Failure<Address>(error.ToString()));
            return this;
        }
    }
}