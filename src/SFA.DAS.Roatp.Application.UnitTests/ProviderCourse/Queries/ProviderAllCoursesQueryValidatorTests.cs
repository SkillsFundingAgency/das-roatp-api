using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAllCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries
{
    [TestFixture]
    public class ProviderAllCoursesQueryValidatorTests
    {
        [TestCase(10000001,  true)]
        [TestCase(10000000, false)]
        [TestCase(100000000,  false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            var query = new GetProviderAllCoursesQuery(ukprn);
            var repoMock = new Mock<IProviderReadRepository>();

            repoMock.Setup(x => x.GetByUkprn(ukprn)).ReturnsAsync(new Provider());
          
            var sut = new GetProviderAllCoursesQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }

        [Test]
        public async Task Validate_InvalidUkprn()
        {
            var ukprn = 1;
            var expectedTimesRepoIsInvoked = 0;
            var expectedErrorMessage1 = UkprnValidator.InvalidUkprnErrorMessage;
           
            var query = new GetProviderAllCoursesQuery(ukprn);
          
            var repoMock = new Mock<IProviderReadRepository>();
            var sut = new GetProviderAllCoursesQueryValidator(repoMock.Object);
        
            var result = await sut.ValidateAsync(query);
        
            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage1, result.Errors[0].ErrorMessage);
        }
    }
}
