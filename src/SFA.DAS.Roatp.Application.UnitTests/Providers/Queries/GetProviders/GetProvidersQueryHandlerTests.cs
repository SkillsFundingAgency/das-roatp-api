using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviders
{
    [TestFixture]
    public class GetProvidersQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            [Frozen] Mock<IProvidersReadRepository> repoMock,
            GetProvidersQuery query,
            List<Provider> providers,
            GetProvidersQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllProviders()).ReturnsAsync(providers);

            var result = await sut.Handle(query, cancellationToken);

            result.Should().NotBeNull();
        }
    }
}