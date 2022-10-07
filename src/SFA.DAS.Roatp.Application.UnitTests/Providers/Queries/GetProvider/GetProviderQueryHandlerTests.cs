using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvider;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProvider
{
    [TestFixture]
    public class GetProviderQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            Domain.Entities.Provider provider,
            [Frozen] Mock<IProvidersReadRepository> repoMock,
            GetProviderQuery query,
            GetProviderQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetByUkprn(query.Ukprn)).ReturnsAsync(provider);

            var result = await sut.Handle(query, cancellationToken);

            result.Should().NotBeNull();
        }
    }
}