using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ContactDetails.GetLatestProviderContact;

[TestFixture]
public sealed class GetLatestProviderContactHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsExpectedResult(
        Domain.Entities.ProviderContact providerContact,
        [Frozen] Mock<IContactDetailsReadRepository> contactDetailsReadRepository,
        GetLatestProviderContactQuery query,
        GetLatestProviderContactQueryHandler sut,
        CancellationToken cancellationToken)
    {
        contactDetailsReadRepository.Setup(r => r.GetLatestProviderContact(query.Ukprn)).ReturnsAsync(providerContact);

        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        var result = response.Result;
        result.Should().BeEquivalentTo(providerContact, c => c
            .Excluding(s => s.ProviderId)
            .Excluding(s => s.Provider)
            .Excluding(s => s.Id)
            .Excluding(s => s.CreatedDate)
        );
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_NullResult_ReturnNullResult(
        [Frozen] Mock<IContactDetailsReadRepository> contactDetailsReadRepository,
        GetLatestProviderContactQuery query,
        GetLatestProviderContactQueryHandler sut,
        CancellationToken cancellationToken)
    {
        contactDetailsReadRepository.Setup(r => r.GetLatestProviderContact(query.Ukprn)).ReturnsAsync((Domain.Entities.ProviderContact)null);

        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeNull();
    }
}
