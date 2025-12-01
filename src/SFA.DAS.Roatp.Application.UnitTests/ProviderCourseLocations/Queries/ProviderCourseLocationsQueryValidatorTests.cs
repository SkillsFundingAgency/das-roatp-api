using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Queries
{
    [TestFixture]
    public class ProviderCourseLocationsQueryValidatorTests
    {
        [TestCase(10000001, true)]
        [TestCase(10000000, false)]
        [TestCase(100000000, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            const string larsCode = "1";
            var query = new GetProviderCourseLocationsQuery(ukprn, larsCode);
            var repoMock = new Mock<IProvidersReadRepository>();
            var repoMockProvideCourses = new Mock<IProviderCoursesReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            repoMockProvideCourses.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());
            var sut = new GetProviderCourseLocationsQueryValidator(repoMock.Object, repoMockProvideCourses.Object);

            var result = await sut.ValidateAsync(query);

            expectedResult.Should().Be(result.IsValid);
        }

        [Test]
        public async Task Validate_InvalidUkprnLarsCode()
        {
            int ukprn = 1;
            string larsCode = "";
            int expectedTimesRepoIsInvoked = 0;
            string expectedErrorMessage1 = UkprnValidator.InvalidUkprnErrorMessage;
            string expectedErrorMessage2 = LarsCodeUkprnCombinationValidator.InvalidLarsCodeErrorMessage;
            var query = new GetProviderCourseLocationsQuery(ukprn, larsCode);
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            var repoMock = new Mock<IProvidersReadRepository>();
            var sut = new GetProviderCourseLocationsQueryValidator(repoMock.Object, repoMockProvideCourse.Object);

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
            string larsCode = "1";
            int expectedTimesRepoIsInvoked = 2;
            string expectedErrorMessage1 = LarsCodeUkprnCombinationValidator.ProviderCourseNotFoundErrorMessage;
            var query = new GetProviderCourseLocationsQuery(ukprn, larsCode);
            var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
            var repoMock = new Mock<IProvidersReadRepository>();
            repoMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            repoMockProvideCourse.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((Domain.Entities.ProviderCourse)null);
            var sut = new GetProviderCourseLocationsQueryValidator(repoMock.Object, repoMockProvideCourse.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            expectedErrorMessage1.Should().Be(result.Errors[0].ErrorMessage);
        }
    }
}
