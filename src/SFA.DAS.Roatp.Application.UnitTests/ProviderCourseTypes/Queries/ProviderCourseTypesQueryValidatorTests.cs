using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;

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
            var sut = new GetProviderCourseTypesQueryValidator();

            var result = await sut.ValidateAsync(query);

            expectedResult.Should().Be(result.IsValid);
        }

        [Test]
        public async Task Validate_InvalidUkprn()
        {
            int ukprn = 1;
            string expectedErrorMessage = UkprnValidator.InvalidUkprnErrorMessage;

            var query = new GetProviderCourseTypesQuery(ukprn);
            var sut = new GetProviderCourseTypesQueryValidator();

            var result = await sut.ValidateAsync(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            expectedErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
        }
    }
}
