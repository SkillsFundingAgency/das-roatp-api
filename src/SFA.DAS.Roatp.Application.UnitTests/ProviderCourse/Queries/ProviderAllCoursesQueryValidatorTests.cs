using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries
{
    [TestFixture]
    public class ProviderAllCoursesQueryValidatorTests
    {
        [TestCase(10000001, true)]
        [TestCase(10000000, false)]
        [TestCase(100000000, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            var query = new GetAllProviderCoursesQuery(ukprn);
            var repoMock = new Mock<IProvidersReadRepository>();

            repoMock.Setup(x => x.GetByUkprn(ukprn)).ReturnsAsync(new Provider());

            var sut = new GetAllProviderCoursesQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            result.IsValid.Should().Be(expectedResult);
        }

        [Test]
        public async Task Validate_InvalidUkprn()
        {
            var ukprn = 1;
            var expectedTimesRepoIsInvoked = 0;
            var expectedErrorMessage1 = UkprnValidator.InvalidUkprnErrorMessage;

            var query = new GetAllProviderCoursesQuery(ukprn);

            var repoMock = new Mock<IProvidersReadRepository>();
            var sut = new GetAllProviderCoursesQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            expectedErrorMessage1.Should().Be(result.Errors[0].ErrorMessage);
        }
    }
}
