using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries.GetProvidersCountForCourse
{
    [TestFixture]
    public class GetProvidersCountForCourseQueryHandlerTests
    {
        [Test]
        public async Task Handler_GetsCountFromRepository()
        {
            var larsCode = 0;
            var expectedCount = 10;
            var repoMock = new Mock<IProviderCoursesReadRepository>();
            repoMock.Setup(x => x.GetProvidersCount(larsCode)).ReturnsAsync(expectedCount);
            var sut = new GetProvidersCountForCourseQueryHandler(repoMock.Object);

            var response = await sut.Handle(new GetProvidersCountForCourseQuery(larsCode), new CancellationToken());

            Assert.AreEqual(expectedCount, response.Result.ProvidersCount);
        }
    }
}
