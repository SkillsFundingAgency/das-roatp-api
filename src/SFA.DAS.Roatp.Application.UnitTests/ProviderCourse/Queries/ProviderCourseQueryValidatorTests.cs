using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries
{
    [TestFixture]
    public class ProviderCourseQueryValidatorTests
    {
        [TestCase(10000001, 1, true)]
        [TestCase(10000000, 1, false)]
        [TestCase(100000000, 1, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, int larsCode, bool expectedResult)
        {
            var query = new GetProviderCourseQuery(ukprn, larsCode);
            var repoMock = new Mock<IProvidersReadRepository>();
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            repoMockProvideCourse.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Domain.Entities.ProviderCourse());
            var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object);

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }

        [Test]
        public async Task Validate_InvalidUkprnLarsCode()
        {
            int ukprn = 1;
            int larsCode = 0;
            int expectedTimesRepoIsInvoked = 0;
            string expectedErrorMessage1 = UkprnValidator.InvalidUkprnErrorMessage;
            string expectedErrorMessage2 = LarsCodeUkprnCombinationValidator.InvalidLarsCodeErrorMessage;
            var query = new GetProviderCourseQuery(ukprn, larsCode);
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            var repoMock = new Mock<IProvidersReadRepository>();
            var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object);
        
            var result = await sut.ValidateAsync(query);
        
            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 2);
            Assert.AreEqual(expectedErrorMessage1, result.Errors[0].ErrorMessage);
            Assert.AreEqual(expectedErrorMessage2, result.Errors[1].ErrorMessage);
        }
        
        [Test]
        public async Task Validate_InvalidUkprnLarsCode_CourseDataNotFound()
        {
            int ukprn = 10012002;
            int larsCode = 1;
            int expectedTimesRepoIsInvoked = 2;
            string expectedErrorMessage1 = LarsCodeUkprnCombinationValidator.ProviderCourseNotFoundErrorMessage;
            var query = new GetProviderCourseQuery(ukprn, larsCode);
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            var repoMock = new Mock<IProvidersReadRepository>();
            repoMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            repoMockProvideCourse.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Domain.Entities.ProviderCourse)null);
            var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object);
        
            var result = await sut.ValidateAsync(query);
        
            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage1, result.Errors[0].ErrorMessage);
        }
    }
}