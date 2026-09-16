using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Common;

public class ProviderCourseTypeRestrictionValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenProviderIsRestrictedForRequestedCourseType_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] ProviderCourseTypeRestrictionValidator sut,
        IProviderCourseTypeRestriction request)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = request.CourseType,
                IsRestrictedProvider = true
            }
        };

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ProviderCourseTypeRestrictionValidator.CourseTypeRestricted);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderIsNotRestrictedForRequestedCourseType_ThenValidationShouldPass(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] ProviderCourseTypeRestrictionValidator sut,
        IProviderCourseTypeRestriction request)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = request.CourseType,
                IsRestrictedProvider = false
            }
        };

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenRequestedCourseTypeDoesNotExistForProvider_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] ProviderCourseTypeRestrictionValidator sut,
        IProviderCourseTypeRestriction request)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>();

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ProviderCourseTypeRestrictionValidator.CourseTypeNotFound);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderHasDifferentCourseType_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] ProviderCourseTypeRestrictionValidator sut,
        IProviderCourseTypeRestriction request)
    {
        // Arrange
        request.CourseType = CourseType.Apprenticeship;

        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.ShortCourse,
                IsRestrictedProvider = false
            }
        };

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ProviderCourseTypeRestrictionValidator.CourseTypeNotFound);
    }
}