using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCourseCommandValidatorIsApprovedByRegulatorValidationTests : CreateProviderCourseCommandValidatorTestBase
    {
        [TestCase(RegulatedLarsCode, true, true, "")]
        [TestCase(RegulatedLarsCode, false, false, CreateProviderCourseCommandValidator.RegulatedStandardMustBeApprovedMessage)]
        [TestCase(NonRegulatedLarsCode, null, true, "")]
        [TestCase(NonRegulatedLarsCode, false, false, CreateProviderCourseCommandValidator.RegulatorsApprovalNotRequired)]
        [TestCase(NonRegulatedLarsCode, true, false, CreateProviderCourseCommandValidator.RegulatorsApprovalNotRequired)]
        public async Task IsApprovedByRegulator_Validation(int larsCode, bool? isApproved, bool isValid, string expectedErrorMessage)
        {
            var command = new CreateProviderCourseCommand { LarsCode = larsCode, Ukprn = ValidUkprn, IsApprovedByRegulator = isApproved };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.IsApprovedByRegulator);
            else
                result.ShouldHaveValidationErrorFor(c => c.IsApprovedByRegulator).WithErrorMessage(expectedErrorMessage);
        }
    }
}
