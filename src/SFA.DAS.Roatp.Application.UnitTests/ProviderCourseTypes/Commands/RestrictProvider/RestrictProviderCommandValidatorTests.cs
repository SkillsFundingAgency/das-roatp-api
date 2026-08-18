using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.RestrictProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Commands.RestrictProvider;

public class RestrictProviderCommandValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenUkprnAndCourseTypeCombinationDoesNotExist_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] RestrictProviderCommandValidator sut)
    {
        // Arrange
        var command = new RestrictProviderCommand
        {
            Ukprn = 12345678,
            CourseType = CourseType.Apprenticeship,
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        providersReadRepository
            .Setup(r => r.GetByUkprn(command.Ukprn))
            .ReturnsAsync(new Provider
            {
                Ukprn = command.Ukprn
            });

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(
                command.Ukprn,
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new()
                {
                    CourseType = CourseType.ShortCourse,
                    IsRestrictedProvider = false
                }
            });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseType)
            .WithErrorMessage(RestrictProviderCommandValidator.ProviderNotFound);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderIsAlreadyRestricted_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] RestrictProviderCommandValidator sut)
    {
        // Arrange
        var command = new RestrictProviderCommand
        {
            Ukprn = 12345678,
            CourseType = CourseType.Apprenticeship,
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        providersReadRepository
            .Setup(r => r.GetByUkprn(command.Ukprn))
            .ReturnsAsync(new Provider
            {
                Ukprn = command.Ukprn
            });

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(
                command.Ukprn,
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new()
                {
                    CourseType = command.CourseType,
                    IsRestrictedProvider = true
                }
            });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseType)
            .WithErrorMessage(RestrictProviderCommandValidator.ProviderAlreadyRestricted);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderExistsAndIsNotRestricted_ThenValidationShouldPass(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] RestrictProviderCommandValidator sut)
    {
        // Arrange
        var command = new RestrictProviderCommand
        {
            Ukprn = 12345678,
            CourseType = CourseType.Apprenticeship,
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        providersReadRepository
            .Setup(r => r.GetByUkprn(command.Ukprn))
            .ReturnsAsync(new Provider
            {
                Ukprn = command.Ukprn
            });

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(
                command.Ukprn,
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new()
                {
                    CourseType = command.CourseType,
                    IsRestrictedProvider = false
                }
            });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CourseType);
    }
}