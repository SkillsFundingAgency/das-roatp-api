using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Common
{
    [TestFixture]
    public class LarsCodeValidatorV2Tests
    {
        Mock<IStandardReadRepository> _standardReadRepositoryMock;
        Mock<IProviderCourseReadRepository> _providerCourseReadRepositoryMock;
        LarsCodeValidatorV2 _sut;
        const int ValidComboLarsCode = 123;
        const int ValidLarsCode = 321;
        const int ValidUkprn = 10012002;

        [SetUp]
        public void Before_Each_Test()
        {
            _standardReadRepositoryMock = new Mock<IStandardReadRepository>();
            _standardReadRepositoryMock.Setup(r => r.GetStandard(It.Is<int>(i => i == ValidComboLarsCode || i == ValidLarsCode))).ReturnsAsync(new Standard());

            _providerCourseReadRepositoryMock = new Mock<IProviderCourseReadRepository>();
            _providerCourseReadRepositoryMock.Setup(r => r.GetProviderCourseByUkprn(ValidUkprn, ValidComboLarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());
        }

        [TestCase(0, LarsCodeValidatorV2.InvalidMessage, false)]
        [TestCase(99999, LarsCodeValidatorV2.NotFoundMessage, false)]
        [TestCase(ValidLarsCode, LarsCodeValidatorV2.CombinationNotFoundErrorMessage, false)]
        [TestCase(ValidComboLarsCode, "", true)]
        public async Task LarsCode_ProviderCourseComboToExist_Validation(int larsCode, string expectedErrorMessage, bool isValid)
        {
            _sut = new LarsCodeValidatorV2(_standardReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, true);
            var testObj = new UkprnValidatorTestObject(ValidUkprn, larsCode);

            var result = await _sut.TestValidateAsync(testObj);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(expectedErrorMessage);
        }

        [TestCase(0, LarsCodeValidatorV2.InvalidMessage, false)]
        [TestCase(99999, LarsCodeValidatorV2.NotFoundMessage, false)]
        [TestCase(ValidComboLarsCode, LarsCodeValidatorV2.CombinationAlreadyExistsMessage, false)]
        [TestCase(ValidLarsCode, "", true)]
        public async Task LarsCode_ProviderCourseComboToNotExist_PassesValidation(int larsCode, string expectedErrorMessage, bool isValid)
        {
            _sut = new LarsCodeValidatorV2(_standardReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, false);
            var testObj = new UkprnValidatorTestObject(ValidUkprn, larsCode);

            var result = await _sut.TestValidateAsync(testObj);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(expectedErrorMessage);
        }

        [Test]
        public async Task LarsCode_ProviderCourseComboToNotExist_FailsValidation()
        {
            _sut = new LarsCodeValidatorV2(_standardReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, false);
            var testObj = new UkprnValidatorTestObject(ValidUkprn, ValidComboLarsCode);

            var result = await _sut.TestValidateAsync(testObj);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(LarsCodeValidatorV2.CombinationAlreadyExistsMessage);
        }
    }

    public class UkprnValidatorTestObject : ILarsCode
    {
        public int Ukprn { get; }

        public int LarsCode { get; }
        public UkprnValidatorTestObject(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
