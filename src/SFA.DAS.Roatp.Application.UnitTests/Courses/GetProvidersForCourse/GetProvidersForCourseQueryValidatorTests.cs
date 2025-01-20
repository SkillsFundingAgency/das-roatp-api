using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForCourse
{
    [TestFixture]
    public class GetProvidersForCourseQueryValidatorTests
    {
        private Mock<IStandardsReadRepository> _standardsReadRepo;

        [SetUp]
        public void Before_each_test()
        {
            _standardsReadRepo = new Mock<IStandardsReadRepository>();
            _standardsReadRepo.Setup(x => x.GetStandard(It.IsAny<int>())).ReturnsAsync(new Standard());
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(-1, false)]
        public async Task Validate_LarsCode(int larsCode, bool isValid)
        {
            decimal? lat = null;
            decimal? lon = null;
            var validator = new GetProvidersForCourseQueryValidator(_standardsReadRepo.Object);

            var result = await validator.TestValidateAsync(new GetProvidersForCourseQuery(larsCode, lat, lon));

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.LarsCode);
                result.Errors[0].ErrorMessage.Should().Be(LarsCodeValidator.InvalidMessage);
            }
        }

        [Test]
        public async Task Validate_LarsCode_NoMatchingStandard()
        {
            var larsCode = 100;
            decimal? lat = null;
            decimal? lon = null;
            _standardsReadRepo.Setup(x => x.GetStandard(larsCode)).ReturnsAsync((Standard)null);
            var validator = new GetProvidersForCourseQueryValidator(_standardsReadRepo.Object);

            var result = await validator.TestValidateAsync(new GetProvidersForCourseQuery(larsCode, lat, lon));

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
            result.Errors[0].ErrorMessage.Should().Be(LarsCodeValidator.NotFoundMessage);
        }

        [TestCase(90, 0, true, "")]
        [TestCase(-90, 0, true, "")]
        [TestCase(0, 180, true, "")]
        [TestCase(0, -180, true, "")]
        [TestCase(null, null, true, "")]
        [TestCase(90.0001, 0, false, CoordinatesValidator.LatitudeOutsideAcceptableRange)]
        [TestCase(-90.0001, 0, false, CoordinatesValidator.LatitudeOutsideAcceptableRange)]
        [TestCase(0, 180.0001, false, CoordinatesValidator.LongitudeOutsideAcceptableRange)]
        [TestCase(0, -180.0001, false, CoordinatesValidator.LongitudeOutsideAcceptableRange)]
        [TestCase(56, null, false, CoordinatesValidator.LatitudeAndNotLongitude)]
        [TestCase(null, 0, false, CoordinatesValidator.NotLatitudeAndLongitude)]
        public async Task Validate_LatitudeLongitude(decimal? lat, decimal? lon, bool isValid, string errorMessage)
        {
            var larsCode = 1;
            var validator = new GetProvidersForCourseQueryValidator(_standardsReadRepo.Object);

            var result = await validator.TestValidateAsync(new GetProvidersForCourseQuery(larsCode, lat, lon));

            isValid.Should().Be(result.IsValid);
            if (!result.IsValid) result.Errors[0].ErrorMessage.Should().Be(errorMessage);
        }
    }
}
