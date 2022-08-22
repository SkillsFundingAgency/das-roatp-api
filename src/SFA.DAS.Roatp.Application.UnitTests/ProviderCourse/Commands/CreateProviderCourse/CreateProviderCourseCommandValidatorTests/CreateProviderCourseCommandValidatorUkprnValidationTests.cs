using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCourseCommandValidatorUkprnValidationTests : CreateProviderCourseCommandValidatorTestBase
    {
        [TestCase(0, false)]
        [TestCase(ValidUkprn, true)]
        public async Task Ukprn_Validation(int ukprn, bool isValid)
        {
            var command = new CreateProviderCourseCommand { Ukprn = ukprn };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }
    }
}
