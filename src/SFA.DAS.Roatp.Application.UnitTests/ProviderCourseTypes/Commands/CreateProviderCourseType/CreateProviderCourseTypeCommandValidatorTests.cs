using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Commands.CreateProviderCourseType;

public class CreateProviderCourseTypeCommandValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenCourseTypesIsEmpty_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] CreateProviderCourseTypeCommandValidator sut)
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes = [],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseTypes)
            .WithErrorMessage(CreateProviderCourseTypeCommandValidator.CourseTypesNotProvided);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsDuplicated_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] CreateProviderCourseTypeCommandValidator sut)
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes =
            [
                "Apprenticeship",
                "Apprenticeship"
            ],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseTypes)
            .WithErrorMessage(CreateProviderCourseTypeCommandValidator.CourseTypesDuplicated);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeAlreadyExistsForProvider_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] CreateProviderCourseTypeCommandValidator sut)
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes = ["Apprenticeship"],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                Ukprn = ukprn,
                CourseType = CourseType.Apprenticeship
            }
        };

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(CreateProviderCourseTypeCommandValidator.CourseTypesExist);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypesDoNotExistForProvider_ThenValidationShouldPass(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] CreateProviderCourseTypeCommandValidator sut)
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes =
            [
                "Apprenticeship",
                "ShortCourse"
            ],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CourseTypes);
        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}
