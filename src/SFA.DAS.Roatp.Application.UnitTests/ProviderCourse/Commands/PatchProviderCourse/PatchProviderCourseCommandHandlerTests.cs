using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.PatchProviderCourse;

[TestFixture]
public class PatchProviderCourseCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_NoDataFound_ThrowsInvalidOperationException(
        [Frozen] Mock<IProviderCoursesWriteRepository> editRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> readRepoMock,
        PatchProviderCourseCommandHandler sut,
        PatchProviderCourseCommand command,
        CancellationToken cancellationToken)
    {
        readRepoMock.Setup(r => r.GetProviderCourseByUkprn(It.Is<int>(i => i == command.Ukprn), It.Is<string>(i => i == command.LarsCode))).ReturnsAsync((Domain.Entities.ProviderCourse)null);

        Func<Task> action = () => sut.Handle(command, cancellationToken);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_DataFound_Patch(
        [Frozen] Mock<IProviderCoursesReadRepository> readRepoMock,
        [Frozen] Mock<IProviderCoursesWriteRepository> editRepoMock,
        PatchProviderCourseCommandHandler sut,
        Domain.Models.PatchProviderCourse patch,
        Domain.Entities.ProviderCourse providerCourse,
        string userId,
        string userDisplayName,
        CancellationToken cancellationToken)
    {
        var ukprn = 10000001;
        var larsCode = "1";
        readRepoMock.Setup(r => r.GetProviderCourseByUkprn(It.Is<int>(i => i == ukprn), It.Is<string>(i => i == larsCode))).ReturnsAsync(providerCourse);

        var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
        patchCommand.Replace(path => path.ContactUsEmail, patch.ContactUsEmail);
        patchCommand.Replace(path => path.ContactUsPhoneNumber, patch.ContactUsPhoneNumber);
        patchCommand.Replace(path => path.StandardInfoUrl, patch.StandardInfoUrl);
        patchCommand.Replace(path => path.IsApprovedByRegulator, patch.IsApprovedByRegulator);
        patchCommand.Replace(path => path.HasOnlineDeliveryOption, patch.HasOnlineDeliveryOption);

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            UserId = userId,
            UserDisplayName = userDisplayName,
            Patch = patchCommand
        };

        await sut.Handle(command, cancellationToken);

        editRepoMock.Verify(e => e.PatchProviderCourse(It.Is<Domain.Entities.ProviderCourse>(
            c => c.ContactUsEmail == patch.ContactUsEmail &&
                 c.ContactUsPhoneNumber == patch.ContactUsPhoneNumber &&
                 c.StandardInfoUrl == patch.StandardInfoUrl &&
                 c.IsApprovedByRegulator == patch.IsApprovedByRegulator &&
                 c.HasOnlineDeliveryOption == patch.HasOnlineDeliveryOption), command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.UpdateProviderCourseDetails));
    }
}