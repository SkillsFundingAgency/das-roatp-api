using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCourseCommandValidatorLocationsValidationTests : CreateProviderCourseCommandValidatorTestBase
    {
        [TestCase(true, true, false, ProviderLocationOption.None, false, "SubregionIds", CreateProviderCourseCommandValidator.EitherNationalOrRegionalMessage)]
        [TestCase(true, false, false, ProviderLocationOption.None, true, "", "")]
        [TestCase(false, true, false, ProviderLocationOption.None, true, "", "")]
        [TestCase(false, false, false, ProviderLocationOption.ValidLocation, true, "", "")]
        [TestCase(false, false, false, ProviderLocationOption.None, false, "ProviderLocations", CreateProviderCourseCommandValidator.AtleastOneLocationIsRequiredMessage)]
        [TestCase(false, false, true, ProviderLocationOption.None, true, "ProviderLocations", "")]
        [TestCase(false, false, false, ProviderLocationOption.InvalidLocation, false, "ProviderLocations", CreateProviderCourseCommandValidator.LocationIdNotFoundMessage)]
        public async Task Locations_Validation(bool hasNationalDeliveryOption, bool addRegions, bool hasOnlineDeliveryOption, ProviderLocationOption providerLocationOption, bool isValid, string propertyName, string expectedErrorMessage)
        {
            var command = new CreateProviderCourseCommand { HasNationalDeliveryOption = hasNationalDeliveryOption, Ukprn = ValidUkprn, HasOnlineDeliveryOption = hasOnlineDeliveryOption };
            if (providerLocationOption == ProviderLocationOption.ValidLocation) command.ProviderLocations = new List<ProviderCourseLocationCommandModel> { new ProviderCourseLocationCommandModel { ProviderLocationId = NavigationId } };
            if (providerLocationOption == ProviderLocationOption.InvalidLocation) command.ProviderLocations = new List<ProviderCourseLocationCommandModel> { new ProviderCourseLocationCommandModel { ProviderLocationId = Guid.NewGuid() } };

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

        public enum ProviderLocationOption
        {
            None,
            ValidLocation,
            InvalidLocation
        }
    }
}
