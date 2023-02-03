using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetProviderLocationDetailsQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            ProviderLocation location, 
            [Frozen]Mock<IProviderLocationsReadRepository> repoMock, 
            GetProviderLocationDetailsQuery query, 
            GetProviderLocationDetailsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetProviderLocation(query.Ukprn, query.Id)).ReturnsAsync(location);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Should().NotBeNull();
        }
    }
}
