using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.DeleteProviderCourse
{
    [TestFixture]
    public class DeleteProviderCourseCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_Records(
            [Frozen] Mock<IProviderCoursesWriteRepository> providerCourseDeleteRepositoryyMock,
            DeleteProviderCourseCommand command,
            DeleteProviderCourseCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var response = await sut.Handle(command, cancellationToken);

            providerCourseDeleteRepositoryyMock.Verify(d => d.Delete(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            response.Result.Should().BeTrue();
        }
    }

}
