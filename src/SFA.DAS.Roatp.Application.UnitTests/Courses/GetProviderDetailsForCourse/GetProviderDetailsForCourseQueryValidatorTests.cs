﻿using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class GetProviderDetailsForCourseQueryValidatorTests
    {
        private Mock<IProvidersReadRepository> _providersReadRepo;
        private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepo;

        [SetUp]
        public void Before_each_test()
        {
            _providersReadRepo = new Mock<IProvidersReadRepository>();
            _providerCoursesReadRepo = new Mock<IProviderCoursesReadRepository>();
            _providersReadRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            _providerCoursesReadRepo.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.Entities.ProviderCourse());
        }

        [TestCase(1, 10000001, null, null, true)]
        [TestCase(1, 10000000, null, null, false)]
        [TestCase(1, 100000000, null, null, false)]
        public async Task Validate_Ukprn(int larsCode, int ukprn, double? lat, double? lon,
            bool isValid)
        {
            var validator =
                new GetProviderDetailsForCourseQueryValidator(_providersReadRepo.Object,
                    _providerCoursesReadRepo.Object);

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
            Double? lat = null;
            Double? lon = null;
            var validator = new GetProviderDetailsForCourseQueryValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);

            var result = await validator.TestValidateAsync(new GetProviderDetailsForCourseQuery(larsCode, ukprn, lat, lon));

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase(56, 0, true, "")]
        [TestCase(null, null, true, "")]
        [TestCase(58, 0, false, GetProviderDetailsForCourseQueryValidator.LatitudeOutsideUk)]
        [TestCase(56, 1.75, false,GetProviderDetailsForCourseQueryValidator.LongitudeOutsideUk)]
        [TestCase(56, null, false, GetProviderDetailsForCourseQueryValidator.LatitudeAndNotLongitude)]
        [TestCase(null, 0, false, GetProviderDetailsForCourseQueryValidator.NotLatitudeAndLongitude)]
        public async Task Validate_LatitudeLongitude(double? lat, double? lon, bool isValid, string ErrorMessage)
        {
            var larsCode = 1;
            var ukprn = 10000001;
            var validator = new GetProviderDetailsForCourseQueryValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);

            var result = await validator.TestValidateAsync(new GetProviderDetailsForCourseQuery(larsCode, ukprn, lat, lon));

           Assert.AreEqual(isValid, result.IsValid);
           if (!result.IsValid)
                Assert.AreEqual(result.Errors[0].ErrorMessage,ErrorMessage);
        }

    }
}