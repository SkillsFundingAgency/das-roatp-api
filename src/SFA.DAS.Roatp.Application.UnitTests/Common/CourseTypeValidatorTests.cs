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
public class CourseTypeValidatorTests
{
    private Mock<IProviderCourseTypesReadRepository> _providerCourseTypesReadRepositoryMock;
    private Mock<IStandardsReadRepository> _standardsReadRepositoryMock;
    private CourseTypeValidator _sut;

    const string ValidLarsCode = "321";
    const int ValidUkprn = 10000001;

    [SetUp]
    public void BeforeEach()
    {
        _providerCourseTypesReadRepositoryMock = new Mock<IProviderCourseTypesReadRepository>();
        _standardsReadRepositoryMock = new Mock<IStandardsReadRepository>();
    }

    [Test]
    public async Task Validate_StandardNotFound_ShouldHaveValidationError()
    {
        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.Is<string>(s => s == ValidLarsCode)))
            .ReturnsAsync((Standard)null);

        _sut = new CourseTypeValidator(_providerCourseTypesReadRepositoryMock.Object, _standardsReadRepositoryMock.Object);

        var testObj = new CourseTypeValidatorTestObject(ValidUkprn, ValidLarsCode);

        var result = await _sut.TestValidateAsync(testObj);

        result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(CourseTypeValidator.ProviderCourseTypeNotFoundErrorMessage);
    }

    [Test]
    public async Task Validate_ProviderCourseTypesNull_ShouldHaveValidationError()
    {
        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.Is<string>(s => s == ValidLarsCode)))
            .ReturnsAsync(new Standard { CourseType = CourseType.Apprenticeship });

        _providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(It.IsAny<int>()))
            .ReturnsAsync((List<ProviderCourseType>)null);

        _sut = new CourseTypeValidator(_providerCourseTypesReadRepositoryMock.Object, _standardsReadRepositoryMock.Object);

        var testObj = new CourseTypeValidatorTestObject(ValidUkprn, ValidLarsCode);

        var result = await _sut.TestValidateAsync(testObj);

        result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(CourseTypeValidator.ProviderCourseTypeNotFoundErrorMessage);
    }

    [Test]
    public async Task Validate_NoMatchingProviderCourseType_ShouldHaveValidationError()
    {
        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.Is<string>(s => s == ValidLarsCode)))
            .ReturnsAsync(new Standard { CourseType = CourseType.Apprenticeship });

        _providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(It.IsAny<int>()))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new ProviderCourseType { CourseType = CourseType.ApprenticeshipUnit }
            });

        _sut = new CourseTypeValidator(_providerCourseTypesReadRepositoryMock.Object, _standardsReadRepositoryMock.Object);

        var testObj = new CourseTypeValidatorTestObject(ValidUkprn, ValidLarsCode);

        var result = await _sut.TestValidateAsync(testObj);

        result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(CourseTypeValidator.ProviderCourseTypeNotFoundErrorMessage);
    }

    [Test]
    public async Task Validate_MatchingProviderCourseType_ShouldNotHaveValidationError()
    {
        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.Is<string>(s => s == ValidLarsCode)))
            .ReturnsAsync(new Standard { CourseType = CourseType.Apprenticeship });

        _providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(It.IsAny<int>()))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new ProviderCourseType { CourseType = CourseType.Apprenticeship },
                new ProviderCourseType { CourseType = CourseType.ApprenticeshipUnit }
            });

        _sut = new CourseTypeValidator(_providerCourseTypesReadRepositoryMock.Object, _standardsReadRepositoryMock.Object);

        var testObj = new CourseTypeValidatorTestObject(ValidUkprn, ValidLarsCode);

        var result = await _sut.TestValidateAsync(testObj);

        result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
    }

    public class CourseTypeValidatorTestObject : ICourseType
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public CourseTypeValidatorTestObject(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}