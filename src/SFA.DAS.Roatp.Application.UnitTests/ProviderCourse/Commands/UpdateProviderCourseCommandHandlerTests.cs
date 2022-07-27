using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateProviderCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands
{
    [TestFixture]
    public class UpdateProviderCourseCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_NoDataFound_ThrowsInvalidOperationException(
            [Frozen] Mock<IProviderCourseReadRepository> readRepoMock,
            UpdateProviderCourseCommandHandler sut,
            UpdateProviderCourseCommand command,
            CancellationToken cancellationToken)
        {
            readRepoMock.Setup(r => r.GetProviderCourseByUkprn(It.Is<int>(i => i == command.Ukprn), It.Is<int>(i => i == command.LarsCode))).ReturnsAsync((Domain.Entities.ProviderCourse)null);

            Func<Task> action = () => sut.Handle(command, cancellationToken);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_DataFound_SaveChanges(
            [Frozen] Mock<IProviderCourseReadRepository> readRepoMock,
            [Frozen] Mock<IProviderCourseEditRepository> editRepoMock,
            UpdateProviderCourseCommandHandler sut,
            UpdateProviderCourseCommand command,
            CancellationToken cancellationToken,
            Domain.Entities.ProviderCourse providerCourse)
        {
            readRepoMock.Setup(r => r.GetProviderCourseByUkprn(It.Is<int>(i => i == command.Ukprn), It.Is<int>(i => i == command.LarsCode))).ReturnsAsync(providerCourse);

            await sut.Handle(command, cancellationToken);

            editRepoMock.Verify(e => e.UpdateProviderCourse(It.Is<Domain.Entities.ProviderCourse>(c => c.ContactUsEmail == command.ContactUsEmail && c.ContactUsPageUrl == command.ContactUsPageUrl && c.ContactUsPhoneNumber == command.ContactUsPhoneNumber && c.StandardInfoUrl == command.StandardInfoUrl)));
        }
    }
}
