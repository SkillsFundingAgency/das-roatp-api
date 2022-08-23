using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.DeleteProviderCourse
{
    [TestFixture]
    public class DeleteProviderCourseCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_Records(
            [Frozen] Mock<IProviderCourseDeleteRepository> providerCourseDeleteRepositoryyMock,
            DeleteProviderCourseCommand command,
            DeleteProviderCourseCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var result = await sut.Handle(command, cancellationToken);

            providerCourseDeleteRepositoryyMock.Verify(d => d.Delete(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.IsTrue(result);
        }
    }

}
