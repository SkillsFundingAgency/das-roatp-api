using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Common;

[TestFixture]
public class LarsCodeUkprnAlreadyExistsValidatorTests
{
    Mock<IStandardsReadRepository> _standardsReadRepositoryMock;
    Mock<IProviderCoursesReadRepository> _providerCoursesReadRepositoryMock;
    LarsCodeUkprnAlreadyExistsValidator _sut;
    const int ValidComboLarsCode = 123;
    const int ValidLarsCode = 321;
    const int ValidUkprn = 10012002;

    [SetUp]
    public void Before_Each_Test()
    {
        _standardsReadRepositoryMock = new Mock<IStandardsReadRepository>();
        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.Is<int>(i => i == ValidComboLarsCode || i == ValidLarsCode))).ReturnsAsync(new Standard());

        _providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
        _providerCoursesReadRepositoryMock.Setup(r => r.GetProviderCourseByUkprn(ValidUkprn, ValidComboLarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());
    }

    [TestCase(0, LarsCodeUkprnAlreadyExistsValidator.InvalidMessage, false)]
    [TestCase(99999, LarsCodeUkprnAlreadyExistsValidator.NotFoundMessage, false)]
    [TestCase(ValidLarsCode, LarsCodeUkprnAlreadyExistsValidator.CombinationNotFoundErrorMessage, false)]
    [TestCase(ValidComboLarsCode, "", true)]
    public async Task LarsCode_ProviderCourseComboToExist_Validation(int larsCode, string expectedErrorMessage, bool isValid)
    {
        _sut = new LarsCodeUkprnAlreadyExistsValidator(_standardsReadRepositoryMock.Object, _providerCoursesReadRepositoryMock.Object, true);
        var testObj = new larsCodeUkprnValidatorTestObject(ValidUkprn, larsCode);

        var result = await _sut.TestValidateAsync(testObj);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
        else
            result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(expectedErrorMessage);
    }

    [TestCase(0, LarsCodeUkprnAlreadyExistsValidator.InvalidMessage, false)]
    [TestCase(99999, LarsCodeUkprnAlreadyExistsValidator.NotFoundMessage, false)]
    [TestCase(ValidComboLarsCode, LarsCodeUkprnAlreadyExistsValidator.CombinationAlreadyExistsMessage, false)]
    [TestCase(ValidLarsCode, "", true)]
    public async Task LarsCode_ProviderCourseComboToNotExist_PassesValidation(int larsCode, string expectedErrorMessage, bool isValid)
    {
        _sut = new LarsCodeUkprnAlreadyExistsValidator(_standardsReadRepositoryMock.Object, _providerCoursesReadRepositoryMock.Object, false);
        var testObj = new larsCodeUkprnValidatorTestObject(ValidUkprn, larsCode);

        var result = await _sut.TestValidateAsync(testObj);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
        else
            result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(expectedErrorMessage);
    }

    [Test]
    public async Task LarsCode_ProviderCourseComboToNotExist_FailsValidation()
    {
        _sut = new LarsCodeUkprnAlreadyExistsValidator(_standardsReadRepositoryMock.Object, _providerCoursesReadRepositoryMock.Object, false);
        var testObj = new larsCodeUkprnValidatorTestObject(ValidUkprn, ValidComboLarsCode);

        var result = await _sut.TestValidateAsync(testObj);

        result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(LarsCodeUkprnAlreadyExistsValidator.CombinationAlreadyExistsMessage);
    }


    public class larsCodeUkprnValidatorTestObject : ILarsCodeUkprn
    {
        public int Ukprn { get; }

        public int LarsCode { get; }
        public larsCodeUkprnValidatorTestObject(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}