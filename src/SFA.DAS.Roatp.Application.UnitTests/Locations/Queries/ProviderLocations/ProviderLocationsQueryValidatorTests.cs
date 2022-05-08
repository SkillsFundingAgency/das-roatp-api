using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class ProviderLocationsQueryValidatorTests
    {
        [TestCase(10000001, true)]
        [TestCase(10000000, false)]
        [TestCase(100000000, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            var query = new ProviderLocationsQuery(ukprn);
            var repoMock = new Mock<IProviderReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(ukprn)).ReturnsAsync(new Provider());
            var sut = new ProviderLocationsQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }

        [TestCase(1, 0, ProviderLocationsQueryValidator.InvalidUkprnErrorMessage)]
        [TestCase(10012002, 1, ProviderLocationsQueryValidator.ProviderNotFoundErrorMessage)]
        public async Task Validate_ExecutesAsyncValidatorForValidUkprnOnly(int ukprn, int expectedTimesRepoIsInvoked, string expectedErrorMessage)
        {
            var query = new ProviderLocationsQuery(ukprn);
            var repoMock = new Mock<IProviderReadRepository>();
            var sut = new ProviderLocationsQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage, result.Errors[0].ErrorMessage);
        }
    }
}
