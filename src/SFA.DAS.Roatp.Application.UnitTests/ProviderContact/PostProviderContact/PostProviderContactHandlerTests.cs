using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderContact.Commands.CreateProviderContact;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderContact.PostProviderContact;
public class PostProviderContactHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_HasSelectedExistingSubregions_CreatesProviderContact(
       [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
       [Frozen] Mock<IProviderContactsWriteRepository> providerContactsWriteRepositoryMock,
       CreateProviderContactCommandHandler sut,
       CreateProviderContactCommand command,
       Provider provider,
       Domain.Entities.ProviderContact providerContact,
       int ukprn,
       List<int> providerCourseIds)
    {
        command.ProviderCourseIds = providerCourseIds;
        command.Ukprn = ukprn;

        providersReadRepositoryMock.Setup(p => p.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);

        await sut.Handle(command, new CancellationToken());

        providersReadRepositoryMock.Verify(p => p.GetByUkprn(It.Is<int>(c => c == ukprn)));
        providerContactsWriteRepositoryMock.Verify(p => p.CreateProviderContact(It.Is<Domain.Entities.ProviderContact>(c => c.ProviderId == provider.Id),
            It.Is<int>(c => c == ukprn), It.Is<string>(c => c == command.UserId), It.Is<string>(c => c == command.UserDisplayName), It.Is<List<int>>(c => c == command.ProviderCourseIds)));
    }
}
