using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCourseCommandValidatorSubregionIdsValidationTests : CreateProviderCourseCommandValidatorTestBase
    {
        [TestCase(ValidRegionId, true)]
        [TestCase(999, false)]
        public async Task SubregionIds_Validation(int regionId, bool isValid)
        {
            var command = new CreateProviderCourseCommand 
            {
                HasNationalDeliveryOption = false, 
                SubregionIds = new List<int> { regionId }
            };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
            {
                result.ShouldNotHaveValidationErrorFor(r => r.SubregionIds);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(r => r.SubregionIds).WithErrorMessage(CreateProviderCourseCommandValidator.RegionIdNotFoundMessage);
            }

        }
    }
}
