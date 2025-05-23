using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.CreateLocation
{
    [TestFixture]
    public class CreateProviderLocationCommandValidatorTests
    {
        [TestCase(10012002, true)]
        [TestCase(10000002, false)]
        public async Task Validates_Ukprn(int ukprn, bool isValid)
        {
            var command = new CreateProviderLocationCommand { Ukprn = ukprn };
            var providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providersReadRepositoryMock.Setup(r => r.GetByUkprn(10012002)).ReturnsAsync(new Provider());
            var sut = new CreateProviderLocationCommandValidator(providersReadRepositoryMock.Object, Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase(" ", false)]
        [TestCase("DATA", true)]
        public async Task Validate_UserId(string userId, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { UserId = userId });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.UserId);
            else
                result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase(" ", false)]
        public async Task Validate_LocationName(string locationName, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { LocationName = locationName });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LocationName);
            else
                result.ShouldHaveValidationErrorFor(c => c.LocationName);
        }
        [Test]
        public async Task Validate_LocationNameTooLong_IsInvalid()
        {
            var locationName = new string('a', 51);
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { LocationName = locationName });
            result.ShouldHaveValidationErrorFor(c => c.LocationName);
        }

        const string DuplicateName = "Duplicate name";
        [TestCase("unique name", true)]
        [TestCase(DuplicateName, false)]
        [TestCase("Duplicate Name", false)]
        public async Task Validate_LocationNameDuplicate_IsInvalid(string name, bool isValid)
        {
            var command = new CreateProviderLocationCommand { LocationName = name };
            var providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providersReadRepositoryMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            var providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { LocationName = DuplicateName, LocationType = LocationType.Provider } });
            var sut = new CreateProviderLocationCommandValidator(providersReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LocationName);
            else
                result.ShouldHaveValidationErrorFor(c => c.LocationName).WithErrorMessage(CreateProviderLocationCommandValidator.LocationNameAlreadyUsedMessage);

        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase(" ", false)]
        [TestCase("DATA", true)]
        public async Task Validate_AddressLine1(string addressline1, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { AddressLine1 = addressline1 });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.AddressLine1);
            else
                result.ShouldHaveValidationErrorFor(c => c.AddressLine1);
        }
        [Test]
        public async Task Validate_AddressLine1TooLong_IsInvalid()
        {
            var addressLine1 = new string('a', 251);
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { AddressLine1 = addressLine1 });
            result.ShouldHaveValidationErrorFor(c => c.AddressLine1);
        }

        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("DATA", true)]
        public async Task Validate_AddressLine2(string addressLine2, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { AddressLine2 = addressLine2 });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.AddressLine2);
            else
                result.ShouldHaveValidationErrorFor(c => c.AddressLine2);
        }
        [Test]
        public async Task Validate_AddressLine2TooLong_IsInvalid()
        {
            var addressLine2 = new string('a', 251);
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { AddressLine2 = addressLine2 });
            result.ShouldHaveValidationErrorFor(c => c.AddressLine2);
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase(" ", false)]
        [TestCase("DATA", true)]
        public async Task Validate_Town(string town, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { Town = town });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Town);
            else
                result.ShouldHaveValidationErrorFor(c => c.Town);
        }
        [Test]
        public async Task Validate_TownTooLong_IsInvalid()
        {
            var town = new string('a', 51);
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { Town = town });
            result.ShouldHaveValidationErrorFor(c => c.Town);
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase(" ", false)]
        [TestCase("M1", false)]
        [TestCase("M1 1AA", true)]
        [TestCase("M60 1NW", true)]
        [TestCase("CR2 6HP", true)]
        [TestCase("DN55 1PT", true)]
        [TestCase("W1P 1HQ", true)]
        [TestCase("EC1A 1BB", true)]
        [TestCase("eC1A1bB", true)]
        public async Task Validate_Postcode(string postcode, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { Postcode = postcode });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Postcode);
            else
                result.ShouldHaveValidationErrorFor(c => c.Postcode);
        }
        [Test]
        public async Task Validate_PostcodeTooLong_IsInvalid()
        {
            var postcode = new string('a', 11);
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { Postcode = postcode });
            result.ShouldHaveValidationErrorFor(c => c.Postcode);
        }

        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("DATA", true)]
        public async Task Validate_County(string county, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { County = county });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.County);
            else
                result.ShouldHaveValidationErrorFor(c => c.County);
        }
        [Test]
        public async Task Validate_CountyTooLong_IsInvalid()
        {
            var county = new string('a', 51);
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { County = county });
            result.ShouldHaveValidationErrorFor(c => c.County);
        }

        [TestCase(null, false)]
        [TestCase(-90.1, false)]
        [TestCase(90.1, false)]
        [TestCase(-90, true)]
        [TestCase(90, true)]
        public async Task Validate_Latitude(decimal? latitude, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { Latitude = latitude });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Latitude);
            else
                result.ShouldHaveValidationErrorFor(c => c.Latitude);
        }

        [TestCase(null, false)]
        [TestCase(-180.1, false)]
        [TestCase(180.1, false)]
        [TestCase(-180, true)]
        [TestCase(180, true)]
        public async Task Validate_Longitude(decimal? longitude, bool isValid)
        {
            var validator = GetDefaultValidator();

            var result = await validator.TestValidateAsync(new CreateProviderLocationCommand { Longitude = longitude });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Longitude);
            else
                result.ShouldHaveValidationErrorFor(c => c.Longitude);
        }

        private CreateProviderLocationCommandValidator GetDefaultValidator() => new CreateProviderLocationCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());
    }
}
