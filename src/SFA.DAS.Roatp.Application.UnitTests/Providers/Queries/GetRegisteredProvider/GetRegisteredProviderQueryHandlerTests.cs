using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetRegisteredProvider;
public class GetRegisteredProviderQueryHandlerTests
{
    [Test, RecursiveMoqAutoData()]
    public async Task Handle_ReturnsResult(
            ProviderRegistrationDetail provider,
            [Frozen] Mock<IProviderRegistrationDetailsReadRepository> repoMock,
            GetRegisteredProviderQuery query,
            GetRegisteredProviderQueryHandler sut,
            CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetProviderRegistrationDetail(query.Ukprn, cancellationToken)).ReturnsAsync(provider);
        GetRegisteredProviderQueryResult expected = provider;

        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeEquivalentTo(expected);
    }

    [Test, RecursiveMoqAutoData()]
    public async Task Handle_ReturnsNullResult(
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> repoMock,
        GetRegisteredProviderQuery query,
        GetRegisteredProviderQueryHandler sut,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetProviderRegistrationDetail(query.Ukprn, cancellationToken)).ReturnsAsync((ProviderRegistrationDetail)null);
        var response = await sut.Handle(query, cancellationToken);
        response.Should().NotBeNull();
        response.Result.Should().BeNull();

    }
}
