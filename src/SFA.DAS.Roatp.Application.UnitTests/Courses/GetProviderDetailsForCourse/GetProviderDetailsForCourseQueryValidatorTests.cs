﻿using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class GetProviderDetailsForCourseQueryValidatorTests
    {
        private Mock<IProvidersReadRepository> _providersReadRepo;
        private Mock<IStandardsReadRepository> _standardsReadRepo;

        [SetUp]
        public void Before_each_test()
        {
            _providersReadRepo = new Mock<IProvidersReadRepository>();
            _providersReadRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            _standardsReadRepo = new Mock<IStandardsReadRepository>();
            _standardsReadRepo.Setup(x => x.GetStandard(It.IsAny<int>())).ReturnsAsync(new Standard());
        }

        [TestCase(1, 10000001, null, null, true)]
        [TestCase(1, 10000000, null, null, false)]
        [TestCase(1, 100000000, null, null, false)]
        public async Task Validate_Ukprn(int larsCode, int ukprn, decimal? lat, decimal? lon,
            bool isValid)
        {
            var validator =
                new GetProviderDetailsForCourseQueryValidator(_providersReadRepo.Object, _standardsReadRepo.Object);

            var result =
                await validator.TestValidateAsync(new GetProviderDetailsForCourseQuery(larsCode, ukprn, lat, lon));

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(-1, false)]
        public async Task Validate_LarsCode(int larsCode, bool isValid)
        {
            var ukprn = 10000001;

            var validator = new GetProviderDetailsForCourseQueryValidator(_providersReadRepo.Object, _standardsReadRepo.Object);

            var result = await validator.TestValidateAsync(new GetProviderDetailsForCourseQuery(larsCode, ukprn, null, null));

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode);
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
            var ukprn = 10000001;
            var validator = new GetProviderDetailsForCourseQueryValidator(_providersReadRepo.Object, _standardsReadRepo.Object);
            var result = await validator.TestValidateAsync(new GetProviderDetailsForCourseQuery(larsCode, ukprn, lat, lon));

            result.IsValid.Should().Be(isValid);
            if (!result.IsValid) result.Errors[0].ErrorMessage.Should().Be(errorMessage);
        }
    }
}