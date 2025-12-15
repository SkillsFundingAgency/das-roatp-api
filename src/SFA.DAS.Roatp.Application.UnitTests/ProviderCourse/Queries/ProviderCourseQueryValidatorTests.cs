using System.Threading.Tasks;
using FluentAssertions;
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
        [TestCase(10000001, true)]
        [TestCase(10000000, false)]
        [TestCase(100000000, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            var larsCode = "1";
            var query = new GetProviderCourseQuery(ukprn, larsCode);
            var repoMock = new Mock<IProvidersReadRepository>();
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            repoMockProvideCourse.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());
            var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object);

            var result = await sut.ValidateAsync(query);

            result.IsValid.Should().Be(expectedResult);
        }

        [Test]
        public async Task Validate_InvalidUkprnLarsCode()
        {
            int ukprn = 1;
            string larsCode = "";
            int expectedTimesRepoIsInvoked = 0;
            string expectedErrorMessage1 = UkprnValidator.InvalidUkprnErrorMessage;
            string expectedErrorMessage2 = LarsCodeUkprnCombinationValidator.InvalidLarsCodeErrorMessage;
            var query = new GetProviderCourseQuery(ukprn, larsCode);
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            var repoMock = new Mock<IProvidersReadRepository>();
            var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            expectedErrorMessage1.Should().Be(result.Errors[0].ErrorMessage);
            expectedErrorMessage2.Should().Be(result.Errors[1].ErrorMessage);
        }

        [Test]
        public async Task Validate_InvalidUkprnLarsCode_CourseDataNotFound()
        {
            int ukprn = 10012002;
            var larsCode = "1";
            int expectedTimesRepoIsInvoked = 2;
            string expectedErrorMessage1 = LarsCodeUkprnCombinationValidator.ProviderCourseNotFoundErrorMessage;
            var query = new GetProviderCourseQuery(ukprn, larsCode);
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            var repoMock = new Mock<IProvidersReadRepository>();
            repoMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            repoMockProvideCourse.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((Domain.Entities.ProviderCourse)null);
            var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            expectedErrorMessage1.Should().Be(result.Errors[0].ErrorMessage);
        }
    }
}