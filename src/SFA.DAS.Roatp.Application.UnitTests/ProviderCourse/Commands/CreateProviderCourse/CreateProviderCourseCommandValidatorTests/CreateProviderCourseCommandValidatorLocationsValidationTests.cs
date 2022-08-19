using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCourseCommandValidatorLocationsValidationTests : CreateProviderCourseCommandValidatorTestBase
    {
        [TestCase(true, true, false, false, "SubregionIds", CreateProviderCourseCommandValidator.EitherNationalOrRegionalMessage)]
        [TestCase(true, false, false, true, "", "")]
        [TestCase(false, true, false, true, "", "")]
        [TestCase(false, false, true, true, "", "")]
        [TestCase(false, false, false, false, "ProviderLocations", CreateProviderCourseCommandValidator.AtleastOneLocationIsRequiredMessage)]
        public async Task Locations_Validation(bool hasNationalDeliveryOption, bool addRegions, bool addProviderLocation, bool isValid, string propertyName, string expectedErrorMessage)
        {
            var command = new CreateProviderCourseCommand { HasNationalDeliveryOption = hasNationalDeliveryOption };
            if (addProviderLocation) command.ProviderLocations = new List<ProviderCourseLocationCommandModel> { new ProviderCourseLocationCommandModel() };
            if (addRegions) command.SubregionIds = new List<int> { 1 };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
            {
                result.ShouldNotHaveValidationErrorFor(propertyName);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(propertyName).WithErrorMessage(expectedErrorMessage);
            }
        }
    }
}
