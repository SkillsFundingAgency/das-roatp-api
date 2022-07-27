using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetProviderLocationDetailsQueryValidatorTests
    {
        //[TestCase(10000001, "95C59C6D-F1CD-4F76-A8F5-4D30A3E1E18F", true)]
        //[TestCase(10000000, "95C59C6D-F1CD-4F76-A8F5-4D30A3E1E18F", false)]
        //[TestCase(100000000, "95C59C6D-F1CD-4F76-A8F5-4D30A3E1E18F", false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, Guid id, bool expectedResult)
        {
            var query = new GetProviderLocationDetailsQuery(ukprn, id);
            var repoMock = new Mock<IProviderReadRepository>();
            var repoMockProviderLocations = new Mock<IProviderLocationsReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            var sut = new GetProviderLocationDetailsQueryValidator(repoMock.Object,  repoMockProviderLocations.Object);

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }

        
        public async Task Validate_InvalidUkprnLarsCode()
        {
            int ukprn = 1;
            Guid id = Guid.Empty;
            int expectedTimesRepoIsInvoked = 0;
            string expectedErrorMessage1 = UkprnValidator.InvalidUkprnErrorMessage;
            string expectedErrorMessage2 = LarsCodeValidator.InvalidLarsCodeErrorMessage;
            var query = new GetProviderLocationDetailsQuery(ukprn, id);
            var repoMock = new Mock<IProviderReadRepository>();
            var repoMockProviderLocations = new Mock<IProviderLocationsReadRepository>();
            var sut = new GetProviderLocationDetailsQueryValidator(repoMock.Object, repoMockProviderLocations.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 2);
            Assert.AreEqual(expectedErrorMessage1, result.Errors[0].ErrorMessage);
            Assert.AreEqual(expectedErrorMessage2, result.Errors[1].ErrorMessage);
        }

      
        public async Task Validate_InvalidUkprnLarsCode_CourseDataNotFound()
        {
            int ukprn = 10012002;
            Guid id = Guid.NewGuid();
            int expectedTimesRepoIsInvoked = 2;
            string expectedErrorMessage1 = LarsCodeValidator.ProviderCourseNotFoundErrorMessage;
            var query = new GetProviderLocationDetailsQuery(ukprn, id);
            var repoMock = new Mock<IProviderReadRepository>();
            var repoMockProviderLocations = new Mock<IProviderLocationsReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            var sut = new GetProviderLocationDetailsQueryValidator(repoMock.Object, repoMockProviderLocations.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage1, result.Errors[0].ErrorMessage);
        }
    }
}
