using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Queries
{
    [TestFixture]
    public class ProviderCourseLocationsQueryValidatorTests
    {
        [TestCase(10000001, 1, true)]
        [TestCase(10000000, 1, false)]
        [TestCase(100000000, 1, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, int providerCourseId, bool expectedResult)
        {
            var query = new ProviderCourseLocationsQuery(ukprn, providerCourseId);
            var repoMock = new Mock<IProviderReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(ukprn)).ReturnsAsync(new Provider());
            var sut = new ProviderCourseLocationsQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }

        [TestCase(1, 0, 0, ProviderCourseLocationsQueryValidator.InvalidUkprnErrorMessage)]
        [TestCase(10012002, 1, 1, ProviderCourseLocationsQueryValidator.ProviderNotFoundErrorMessage)]
        public async Task Validate_ExecutesAsyncValidatorForValidUkprnOnly(int ukprn, int providerCourseId, int expectedTimesRepoIsInvoked, string expectedErrorMessage)
        {
            var query = new ProviderCourseLocationsQuery(ukprn, providerCourseId);
            var repoMock = new Mock<IProviderReadRepository>();
            var sut = new ProviderCourseLocationsQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage, result.Errors[0].ErrorMessage);
        }
    }
}
