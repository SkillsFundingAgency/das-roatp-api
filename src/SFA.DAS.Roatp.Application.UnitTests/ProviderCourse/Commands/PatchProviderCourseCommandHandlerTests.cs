using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Domain.Constants;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands
{
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
            readRepoMock.Setup(r => r.GetProviderCourseByUkprn(It.Is<int>(i => i == command.Ukprn), It.Is<int>(i => i == command.LarsCode))).ReturnsAsync((Domain.Entities.ProviderCourse)null);

            Func<Task> action = () => sut.Handle(command, cancellationToken);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_DataFound_Patch(
            [Frozen] Mock<IProviderCoursesReadRepository> readRepoMock,
            [Frozen] Mock<IProviderCoursesWriteRepository> editRepoMock,
            PatchProviderCourseCommandHandler sut,
            PatchProviderCourse patch,
            CancellationToken cancellationToken,
            Domain.Entities.ProviderCourse providerCourse, string userId, string userDisplayName)
        {
            var ukprn = 10000001;
            var larsCode = 1;
            readRepoMock.Setup(r => r.GetProviderCourseByUkprn(It.Is<int>(i => i == ukprn), It.Is<int>(i => i == larsCode))).ReturnsAsync(providerCourse);

           var patchCommand = new JsonPatchDocument<PatchProviderCourse>();
           patchCommand.Replace(path => path.ContactUsEmail, patch.ContactUsEmail);
           patchCommand.Replace(path => path.ContactUsPageUrl, patch.ContactUsPageUrl);
           patchCommand.Replace(path => path.ContactUsPhoneNumber, patch.ContactUsPhoneNumber);
           patchCommand.Replace(path => path.StandardInfoUrl, patch.StandardInfoUrl);
           patchCommand.Replace(path => path.IsApprovedByRegulator, patch.IsApprovedByRegulator);

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
                      c.ContactUsPageUrl == patch.ContactUsPageUrl && 
                      c.ContactUsPhoneNumber == patch.ContactUsPhoneNumber && 
                      c.StandardInfoUrl == patch.StandardInfoUrl &&
                      c.IsApprovedByRegulator == patch.IsApprovedByRegulator),command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.UpdateProviderCourseContactDetailsOrConfirmingRegulator));
        }
    }
}
