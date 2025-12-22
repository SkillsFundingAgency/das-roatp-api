using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Standards.GetStandardForLarsCode;

[TestFixture]
public class GetStandardForLarsCodeQueryValidatorTests
{
    private Mock<IStandardsReadRepository> _standardsReadRepo;

    [SetUp]
    public void Before_each_test()
    {
        _standardsReadRepo = new Mock<IStandardsReadRepository>();
        _standardsReadRepo.Setup(x => x.GetStandard(It.IsAny<string>())).ReturnsAsync(new Standard());
    }

    [TestCase("", false)]
    [TestCase("1", true)]
    public async Task Validate_LarsCode(string larsCode, bool isValid)
    {
        var validator = new GetStandardForLarsCodeQueryValidator(_standardsReadRepo.Object);

        var result = await validator.TestValidateAsync(new GetStandardForLarsCodeQuery(larsCode));

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
            result.Errors[0].ErrorMessage.Should().Be(LarsCodeValidator.InvalidMessage);
        }
    }
}