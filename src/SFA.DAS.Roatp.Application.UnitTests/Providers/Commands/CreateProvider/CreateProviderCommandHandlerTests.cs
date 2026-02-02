using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.CreateProvider
{
    [TestFixture]
    public class CreateProviderCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_CreatesProvider_WithExistingRegistrationDetails(
            [Frozen] Mock<IProvidersWriteRepository> providersWriteRepositoryMock,
            [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> providerRegistrationDetailsWriteRepositoryMock,
            ProviderRegistrationDetail providerRegistrationDetail,
            CreateProviderCommandHandler sut,
            CreateProviderCommand command
        )
        {
            var provider = (Provider)command;
            providersWriteRepositoryMock.Setup(p => p.Create(command, command.UserId, command.UserDisplayName, AuditEventTypes.CreateProvider)).ReturnsAsync(provider);
            providerRegistrationDetailsWriteRepositoryMock.Setup(p => p.GetProviderRegistrationDetail(provider.Ukprn))
                .ReturnsAsync(providerRegistrationDetail);
            await sut.Handle(command, new CancellationToken());
            providersWriteRepositoryMock.Verify(p => p.Create(It.Is<Provider>(c => c.Ukprn == provider.Ukprn && !c.IsImported && c.LegalName == provider.LegalName && c.TradingName == provider.TradingName && c.Email == provider.Email && c.Phone == provider.Phone && c.Website == provider.Website && c.ProviderRegistrationDetail != null), command.UserId, command.UserDisplayName, AuditEventTypes.CreateProvider));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_CreatesProvider_WithNewRegistrationDetails(
            [Frozen] Mock<IProvidersWriteRepository> providersWriteRepositoryMock,
            [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> providerRegistrationDetailsWriteRepositoryMock,
            ProviderRegistrationDetail providerRegistrationDetail,
            CreateProviderCommandHandler sut,
            CreateProviderCommand command
        )
        {
            var provider = (Provider)command;
            providersWriteRepositoryMock.Setup(p => p.Create(command, command.UserId, command.UserDisplayName, AuditEventTypes.CreateProvider)).ReturnsAsync(provider);
            providerRegistrationDetailsWriteRepositoryMock.Setup(p => p.GetProviderRegistrationDetail(provider.Ukprn))
                .ReturnsAsync(() => null);
            await sut.Handle(command, new CancellationToken());
            providersWriteRepositoryMock.Verify(p => p.Create(It.Is<Provider>(c => c.ProviderRegistrationDetail != null && c.ProviderRegistrationDetail.Ukprn == command.Ukprn && c.ProviderRegistrationDetail.LegalName == command.LegalName && c.ProviderRegistrationDetail.StatusId == (int)OrganisationStatus.Onboarding && c.ProviderRegistrationDetail.ProviderTypeId == (int)ProviderType.Main && c.ProviderRegistrationDetail.OrganisationTypeId == 0), command.UserId, command.UserDisplayName, AuditEventTypes.CreateProvider));
        }
    }
}
