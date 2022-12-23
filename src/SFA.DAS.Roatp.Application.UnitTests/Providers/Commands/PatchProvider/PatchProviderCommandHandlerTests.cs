using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.PatchProvider
{
    [TestFixture]
    public class PatchProviderCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_NoDataFound_ThrowsInvalidOperationException(
            [Frozen] Mock<IProvidersReadRepository> readRepoMock,
            PatchProviderCommandHandler sut,
            PatchProviderCommand command,
            CancellationToken cancellationToken)
        {
            readRepoMock.Setup(r => r.GetByUkprn(It.Is<int>(i => i == command.Ukprn))).ReturnsAsync((Domain.Entities.Provider)null);

            Func<Task> action = () => sut.Handle(command, cancellationToken);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_DataFound_Patch(
            [Frozen] Mock<IProvidersReadRepository> readRepoMock,
            [Frozen] Mock<IProvidersWriteRepository> editRepoMock,
            PatchProviderCommandHandler sut,
            Domain.Models.PatchProvider patch,
            Domain.Entities.Provider provider, 
            string userId, 
            string userDisplayName,
            CancellationToken cancellationToken)
        {
            var ukprn = 10000001;
            readRepoMock.Setup(r => r.GetByUkprn(It.Is<int>(i => i == ukprn))).ReturnsAsync(provider);

            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProvider>();
            patchCommand.Replace(path => path.MarketingInfo, patch.MarketingInfo);

            var command = new PatchProviderCommand
            {
                Ukprn = ukprn,
                UserId = userId,
                UserDisplayName = userDisplayName,
                Patch = patchCommand
            };

            await sut.Handle(command, cancellationToken);

            editRepoMock.Verify(e => e.Patch(It.Is<Domain.Entities.Provider>(
                c => c.MarketingInfo == patch.MarketingInfo ), command.UserId, command.UserDisplayName, AuditEventTypes.UpdateProviderDescription));
        }
    }
}
