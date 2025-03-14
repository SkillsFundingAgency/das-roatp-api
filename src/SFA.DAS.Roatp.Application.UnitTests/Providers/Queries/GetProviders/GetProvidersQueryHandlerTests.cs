using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviders;

[TestFixture]
public class GetProvidersQueryHandlerTests
{
    [Test]
    [RecursiveMoqAutoData]
    public async Task Handle_LiveIsFalse_GetActiveProviderRegistrationsIsCalled(
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> repoMock,
        List<ProviderRegistrationDetail> providerRegistrationDetails,
        GetProvidersQueryHandler sut,
        CancellationToken cancellationToken
    )
    {
        var query = new GetProvidersQuery() { Live = false };

        repoMock.Setup(r =>
            r.GetActiveProviderRegistrations(It.IsAny<CancellationToken>())
        ).ReturnsAsync(providerRegistrationDetails);

        var result = await sut.Handle(query, cancellationToken);

        repoMock.Verify(r =>
            r.GetActiveProviderRegistrations(It.IsAny<CancellationToken>()),
            Times.Once
        );

        repoMock.Verify(r =>
            r.GetActiveAndMainProviderRegistrations(It.IsAny<CancellationToken>()),
            Times.Never
        );

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<GetProvidersQueryResult>());

            var expectedResult = providerRegistrationDetails.Select(a => (ProviderSummary)a).ToList();
            var actualResult = result.RegisteredProviders.ToList();

            Assert.That(actualResult, Has.Count.EqualTo(expectedResult.Count));

            for (int i = 0; i < expectedResult.Count; i++)
            {
                Assert.That(actualResult[i].Ukprn, Is.EqualTo(expectedResult[i].Ukprn));
                Assert.That(actualResult[i].Name, Is.EqualTo(expectedResult[i].Name));
                Assert.That(actualResult[i].ProviderTypeId, Is.EqualTo(expectedResult[i].ProviderTypeId));
                Assert.That(actualResult[i].TradingName, Is.EqualTo(expectedResult[i].TradingName));
                Assert.That(actualResult[i].Email, Is.EqualTo(expectedResult[i].Email));
                Assert.That(actualResult[i].Phone, Is.EqualTo(expectedResult[i].Phone));
                Assert.That(actualResult[i].ContactUrl, Is.EqualTo(expectedResult[i].ContactUrl));
                Assert.That(actualResult[i].StatusId, Is.EqualTo(expectedResult[i].StatusId));

                Assert.That(actualResult[i].Address, Is.Not.Null);
                Assert.That(actualResult[i].Address.AddressLine1, Is.EqualTo(expectedResult[i].Address.AddressLine1));
                Assert.That(actualResult[i].Address.AddressLine2, Is.EqualTo(expectedResult[i].Address.AddressLine2));
                Assert.That(actualResult[i].Address.AddressLine3, Is.EqualTo(expectedResult[i].Address.AddressLine3));
                Assert.That(actualResult[i].Address.AddressLine4, Is.EqualTo(expectedResult[i].Address.AddressLine4));
                Assert.That(actualResult[i].Address.Town, Is.EqualTo(expectedResult[i].Address.Town));
                Assert.That(actualResult[i].Address.Postcode, Is.EqualTo(expectedResult[i].Address.Postcode));
                Assert.That(actualResult[i].Address.Latitude, Is.EqualTo(expectedResult[i].Address.Latitude));
                Assert.That(actualResult[i].Address.Longitude, Is.EqualTo(expectedResult[i].Address.Longitude));
            }
        });
    }

    [Test]
    [RecursiveMoqAutoData]
    public async Task Handle_LiveIsTrue_GetActiveAndMainProviderRegistrationsIsCalled(
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> repoMock,
        List<ProviderRegistrationDetail> providerRegistrationDetails,
        GetProvidersQueryHandler sut,
        CancellationToken cancellationToken
    )
    {
        var query = new GetProvidersQuery() { Live = true };

        repoMock.Setup(r =>
            r.GetActiveAndMainProviderRegistrations(It.IsAny<CancellationToken>())
        ).ReturnsAsync(providerRegistrationDetails);

        var result = await sut.Handle(query, cancellationToken);

        repoMock.Verify(r =>
            r.GetActiveProviderRegistrations(It.IsAny<CancellationToken>()),
            Times.Never
        );

        repoMock.Verify(r =>
            r.GetActiveAndMainProviderRegistrations(It.IsAny<CancellationToken>()),
            Times.Once
        );

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<GetProvidersQueryResult>());

            var expectedResult = providerRegistrationDetails.Select(a => (ProviderSummary)a).ToList();
            var actualResult = result.RegisteredProviders.ToList();

            Assert.That(actualResult, Has.Count.EqualTo(expectedResult.Count));

            for (int i = 0; i < expectedResult.Count; i++)
            {
                Assert.That(actualResult[i].Ukprn, Is.EqualTo(expectedResult[i].Ukprn));
                Assert.That(actualResult[i].Name, Is.EqualTo(expectedResult[i].Name));
                Assert.That(actualResult[i].ProviderTypeId, Is.EqualTo(expectedResult[i].ProviderTypeId));
                Assert.That(actualResult[i].TradingName, Is.EqualTo(expectedResult[i].TradingName));
                Assert.That(actualResult[i].Email, Is.EqualTo(expectedResult[i].Email));
                Assert.That(actualResult[i].Phone, Is.EqualTo(expectedResult[i].Phone));
                Assert.That(actualResult[i].ContactUrl, Is.EqualTo(expectedResult[i].ContactUrl));
                Assert.That(actualResult[i].StatusId, Is.EqualTo(expectedResult[i].StatusId));

                Assert.That(actualResult[i].Address, Is.Not.Null);
                Assert.That(actualResult[i].Address.AddressLine1, Is.EqualTo(expectedResult[i].Address.AddressLine1));
                Assert.That(actualResult[i].Address.AddressLine2, Is.EqualTo(expectedResult[i].Address.AddressLine2));
                Assert.That(actualResult[i].Address.AddressLine3, Is.EqualTo(expectedResult[i].Address.AddressLine3));
                Assert.That(actualResult[i].Address.AddressLine4, Is.EqualTo(expectedResult[i].Address.AddressLine4));
                Assert.That(actualResult[i].Address.Town, Is.EqualTo(expectedResult[i].Address.Town));
                Assert.That(actualResult[i].Address.Postcode, Is.EqualTo(expectedResult[i].Address.Postcode));
                Assert.That(actualResult[i].Address.Latitude, Is.EqualTo(expectedResult[i].Address.Latitude));
                Assert.That(actualResult[i].Address.Longitude, Is.EqualTo(expectedResult[i].Address.Longitude));
            }
        });
    }
}