using Blauhaus.Geolocation.Abstractions.ValueObjects;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Geolocation.TestHelpers
{
    public class GpsLocationMockBuilder : BaseMockBuilder<GpsLocationMockBuilder, IGpsLocation>
    {
        public GpsLocationMockBuilder Where_GetMetresFrom_returns(double value)
        {
            Mock.Setup(x => x.GetMetresFrom(It.IsAny<IGpsLocation>()))
                .Returns(value);
            return this;
        }
        public GpsLocationMockBuilder Where_GetMetresFrom_returns(double value, IGpsLocation other)
        {
            Mock.Setup(x => x.GetMetresFrom(other))
                .Returns(value);
            return this;
        }
    }
}