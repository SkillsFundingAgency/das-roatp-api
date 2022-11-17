using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Common;

[TestFixture]
public class LarsCodeValidatorTests
{
    Mock<IStandardsReadRepository> _standardsReadRepositoryMock;
    LarsCodeValidator _sut;
    const int ValidLarsCode = 321;

    [SetUp]
    public void Before_Each_Test()
    {
        _standardsReadRepositoryMock = new Mock<IStandardsReadRepository>();
        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.Is<int>( i=> i == ValidLarsCode))).ReturnsAsync(new Standard());
    }

    [TestCase(0, LarsCodeValidator.InvalidMessage, false)]
    [TestCase(99999, LarsCodeValidator.NotFoundMessage, false)]
    [TestCase(ValidLarsCode, "", true)]
    public async Task LarsCode_ProviderCourseComboToNotExist_PassesValidation(int larsCode, string expectedErrorMessage, bool isValid)
    {
        _sut = new LarsCodeValidator(_standardsReadRepositoryMock.Object);
        var testObj = new LarsCodeValidatorTestObject(larsCode);
    
        var result = await _sut.TestValidateAsync(testObj);
    
        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
        else
            result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(expectedErrorMessage);
    }
    
    public class LarsCodeValidatorTestObject : ILarsCode
    {
        public int LarsCode { get; }
        public LarsCodeValidatorTestObject( int larsCode)
        {
            LarsCode = larsCode;
        }
    }
}