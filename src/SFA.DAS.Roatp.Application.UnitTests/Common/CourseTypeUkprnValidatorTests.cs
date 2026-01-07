using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Common;

[TestFixture]
public class CourseTypeUkprnValidatorTests
{
    Mock<IProviderCourseTypesReadRepository> _providerCourseTypesReadRepositoryMock;
    CourseTypeUkprnValidator _sut;
    const int ValidUkprn = 123;
    const CourseType ValidCourseTypeValue = CourseType.Apprenticeship;

    [SetUp]
    public void Before_Each_Test()
    {
        _providerCourseTypesReadRepositoryMock = new Mock<IProviderCourseTypesReadRepository>();

        _providerCourseTypesReadRepositoryMock
            .Setup(r => r.GetProviderCourseTypesByUkprn(It.Is<int>(i => i == ValidUkprn)))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new ProviderCourseType { Ukprn = ValidUkprn, CourseType = ValidCourseTypeValue }
            });
    }

    [TestCase(ValidUkprn, ValidCourseTypeValue, "", true)]
    [TestCase(ValidUkprn, 99, CourseTypeUkprnValidator.ProviderCourseTypeNotFoundErrorMessage, false)]
    [TestCase(99999, ValidCourseTypeValue, CourseTypeUkprnValidator.ProviderCourseTypeNotFoundErrorMessage, false)]
    public async Task CourseTypeUkprn_ProviderCourseComboToNotExist_PassesValidation(int ukprn, CourseType courseTypeValue, string expectedErrorMessage, bool isValid)
    {
        _sut = new CourseTypeUkprnValidator(_providerCourseTypesReadRepositoryMock.Object);
        var testObj = new CourseTypeUkprnValidatorTestObject(ukprn, (CourseType)courseTypeValue);

        var result = await _sut.TestValidateAsync(testObj);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
        else
            result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(expectedErrorMessage);
    }

    public class CourseTypeUkprnValidatorTestObject : ICourseTypeUkprn
    {
        public int Ukprn { get; }
        public CourseType CourseType { get; }

        public CourseTypeUkprnValidatorTestObject(int ukprn, CourseType courseType)
        {
            Ukprn = ukprn;
            CourseType = courseType;
        }
    }
}