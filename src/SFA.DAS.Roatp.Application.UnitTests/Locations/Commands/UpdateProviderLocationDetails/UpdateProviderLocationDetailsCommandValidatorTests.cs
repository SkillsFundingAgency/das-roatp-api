﻿using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.UpdateProviderLocationDetails
{
    [TestFixture]
    public class UpdateProviderLocationDetailsCommandValidatorTests
    {
        [TestCase(10012002, true)]
        [TestCase(10000002, false)]
        public async Task Validates_Ukprn(int ukprn, bool isValid)
        {
            var command = new UpdateProviderLocationDetailsCommand { Ukprn = ukprn };
            var providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providersReadRepositoryMock.Setup(r => r.GetByUkprn(10012002)).ReturnsAsync(new Provider());
            var sut = new UpdateProviderLocationDetailsCommandValidator(providersReadRepositoryMock.Object, Mock.Of<IProviderLocationsReadRepository>());

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

            var result = await validator.TestValidateAsync(new UpdateProviderLocationDetailsCommand { UserId = userId });

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

            var result = await validator.TestValidateAsync(new UpdateProviderLocationDetailsCommand { LocationName = locationName });

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

            var result = await validator.TestValidateAsync(new UpdateProviderLocationDetailsCommand { LocationName = locationName });
            result.ShouldHaveValidationErrorFor(c => c.LocationName);
        }

        [Test]
        public async Task Validate_LocationNameUnqiue_IsInvalid()
        {
            var command = new UpdateProviderLocationDetailsCommand { Id = Guid.NewGuid(), LocationName = "unique name" };
            var providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providersReadRepositoryMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            var providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { NavigationId = command.Id, LocationName = "unique name", LocationType = LocationType.Provider } });
            var sut = new UpdateProviderLocationDetailsCommandValidator(providersReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(c => c.LocationName);
        }

        [Test]
        public async Task Validate_LocationNameDuplicate_IsInvalid()
        {
            var command = new UpdateProviderLocationDetailsCommand { Id = Guid.NewGuid(), LocationName = "Duplicate name" };
            var providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providersReadRepositoryMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            var providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { NavigationId = Guid.NewGuid(), LocationName = "Duplicate name", LocationType = LocationType.Provider } });
            var sut = new UpdateProviderLocationDetailsCommandValidator(providersReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LocationName).WithErrorMessage(UpdateProviderLocationDetailsCommandValidator.LocationNameAlreadyUsedMessage);
        }

        private UpdateProviderLocationDetailsCommandValidator GetDefaultValidator() => new UpdateProviderLocationDetailsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());
    }
}
