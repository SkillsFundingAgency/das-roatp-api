using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviders
{
    [TestFixture]
    public class ProviderAddressModelTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(ProviderAddress source)
        {
            var model = (ProviderAddressModel)source;

            model.Id.Should().Be(source.Id);
            model.ProviderId.Should().Be(source.ProviderId);
            model.AddressLine1.Should().Be(source.AddressLine1);
            model.AddressLine2.Should().Be(source.AddressLine2);
            model.AddressLine3.Should().Be(source.AddressLine3);
            model.AddressLine4.Should().Be(source.AddressLine4);
            model.Town.Should().Be(source.Town);
            model.Postcode.Should().Be(source.Postcode);
            model.Latitude.Should().Be(source.Latitude);
            model.Longitude.Should().Be(source.Longitude);
            model.AddressUpdateDate.Should().Be(source.AddressUpdateDate);
            model.CoordinatesUpdateDate.Should().Be(source.CoordinatesUpdateDate);
        }
    }
}
