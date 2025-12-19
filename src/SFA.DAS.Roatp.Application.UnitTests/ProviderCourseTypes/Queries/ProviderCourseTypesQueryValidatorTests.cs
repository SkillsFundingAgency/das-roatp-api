using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Queries
{
    [TestFixture]
    public class ProviderCourseTypesQueryValidatorTests
    {
        [TestCase(10000001, true)]
        [TestCase(10000000, false)]
        [TestCase(100000000, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            var query = new GetProviderCourseTypesQuery(ukprn);
            var repoMock = new Mock<IProvidersReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            var sut = new GetProviderCourseTypesQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            expectedResult.Should().Be(result.IsValid);
        }

        [Test]
        public async Task Validate_InvalidUkprn()
        {
            int ukprn = 1;
            string expectedErrorMessage = UkprnValidator.InvalidUkprnErrorMessage;

            var query = new GetProviderCourseTypesQuery(ukprn);
            var repoMock = new Mock<IProvidersReadRepository>();
            var sut = new GetProviderCourseTypesQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            expectedErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
        }
    }
}
