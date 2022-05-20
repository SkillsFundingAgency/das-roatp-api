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
        [TestCase(0, false)]
        [TestCase(-1, false)]
        [TestCase(1, true)]
        public async Task Validate_ProviderCourseId(int providerCourseId, bool expectedResult)
        {
            var query = new ProviderCourseLocationsQuery(providerCourseId);
            var repoMock = new Mock<IProviderCourseLocationReadRepository>();
            repoMock.Setup(x => x.GetAllProviderCourseLocations(providerCourseId)).ReturnsAsync(new List<ProviderCourseLocation>());
            var sut = new ProviderCourseLocationsQueryValidator();

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }

        [TestCase(0, ProviderCourseLocationsQueryValidator.InvalidProviderCourseIdErrorMessage)]
        public async Task Validate_ExecutesAsyncValidatorForProviderCourseId(int providerCourseId, string expectedErrorMessage)
        {
            var query = new ProviderCourseLocationsQuery(providerCourseId);
            var sut = new ProviderCourseLocationsQueryValidator();

            var result = await sut.ValidateAsync(query);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage, result.Errors[0].ErrorMessage);
        }
    }
}
