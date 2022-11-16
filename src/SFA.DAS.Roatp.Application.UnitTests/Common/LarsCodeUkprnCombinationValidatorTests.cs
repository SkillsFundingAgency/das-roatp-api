using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Common;

[TestFixture]
public class LarsCodeUkprnCombinationValidatorTests
{
    Mock<IStandardsReadRepository> _standardsReadRepositoryMock;
    Mock<IProviderCoursesReadRepository> _providerCoursesReadRepositoryMock;
    LarsCodeUkprnCombinationValidator _sut;
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

    [Test]
    public async Task LarsCode_ProviderCourseComboToNotExist_FailsValidation()
    {
        _sut = new LarsCodeUkprnCombinationValidator(_providerCoursesReadRepositoryMock.Object, false);
        var testObj = new LarsCodeUkprnValidatorTestObject(ValidUkprn, ValidComboLarsCode);

        var result = await _sut.TestValidateAsync(testObj);

        result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(LarsCodeUkprnCombinationValidator.CombinationAlreadyExistsMessage);
    }

    public class LarsCodeUkprnValidatorTestObject : ILarsCodeUkprn
    {
        public int Ukprn { get; }

        public int LarsCode { get; }
        public LarsCodeUkprnValidatorTestObject(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}