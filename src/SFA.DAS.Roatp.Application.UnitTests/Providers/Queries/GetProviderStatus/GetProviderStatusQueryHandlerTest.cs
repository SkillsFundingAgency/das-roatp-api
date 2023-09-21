using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderStatus;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviderStatus
{
    [TestFixture]
    public class GetProviderStatusQueryHandlerTest
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_When_Type_And_Status_Valid_ReturnsResult_True(
            ProviderRegistrationDetail providerRegistrationDetail,
            [Frozen] Mock<IProviderRegistrationDetailsReadRepository> repoMockProviderRegistrationDetailsReadRepository,
            GetProviderStatusQuery query,
            GetProviderStatusQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerRegistrationDetail.ProviderTypeId = (int) ProviderType.Main;
            providerRegistrationDetail.StatusId = (int) ProviderStatusType.Active;
            repoMockProviderRegistrationDetailsReadRepository.Setup(r => r.GetProviderRegistrationDetail(query.Ukprn)).ReturnsAsync(providerRegistrationDetail);

            var result = await sut.Handle(query, cancellationToken);

            result.Should().NotBeNull();
            result.Result.IsValidProvider.Should().BeTrue();
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_When_Type_And_Status_InValid_ReturnsResult_False(
            ProviderRegistrationDetail providerRegistrationDetail,
            [Frozen] Mock<IProviderRegistrationDetailsReadRepository> repoMockProviderRegistrationDetailsReadRepository,
            GetProviderStatusQuery query,
            GetProviderStatusQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerRegistrationDetail.ProviderTypeId = (int)ProviderType.Supporting;
            providerRegistrationDetail.StatusId = (int)ProviderStatusType.ActiveButNotTakingOnApprentices;
            repoMockProviderRegistrationDetailsReadRepository.Setup(r => r.GetProviderRegistrationDetail(query.Ukprn)).ReturnsAsync(providerRegistrationDetail);

            var result = await sut.Handle(query, cancellationToken);

            result.Should().NotBeNull();
            result.Result.IsValidProvider.Should().BeFalse();
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_When_Provider_NotFound_ReturnsResult_False(
            [Frozen] Mock<IProviderRegistrationDetailsReadRepository> repoMockProviderRegistrationDetailsReadRepository,
            GetProviderStatusQuery query,
            GetProviderStatusQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMockProviderRegistrationDetailsReadRepository.Setup(r => r.GetProviderRegistrationDetail(query.Ukprn)).ReturnsAsync((ProviderRegistrationDetail)null);

            var result = await sut.Handle(query, cancellationToken);

            result.Should().NotBeNull();
            result.Result.IsValidProvider.Should().BeFalse();
        }
    }
}
