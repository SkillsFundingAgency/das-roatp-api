using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class GetProviderRestrictedApprenticeshipsQueryValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenProviderIsRestrictedForApprenticeships_ThenValidationShouldFail(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryValidator sut,
        GetProviderRestrictedApprenticeshipsQuery query)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.Apprenticeship,
                IsRestrictedProvider = true
            }
        };

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(query.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(GetProviderRestrictedApprenticeshipsQueryValidator.ProviderNotRestricted);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderIsNotRestrictedForApprenticeships_ThenValidationShouldPass(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryValidator sut,
        GetProviderRestrictedApprenticeshipsQuery query)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.Apprenticeship,
                IsRestrictedProvider = false
            }
        };

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(query.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderHasNoCourseTypes_ThenValidationShouldPass(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryValidator sut,
        GetProviderRestrictedApprenticeshipsQuery query)
    {
        // Arrange
        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(query.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderCourseType>());

        // Act
        var result = await sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderIsRestrictedForDifferentCourseType_ThenValidationShouldPass(
    [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
    [Greedy] GetProviderRestrictedApprenticeshipsQueryValidator sut,
    GetProviderRestrictedApprenticeshipsQuery query)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.ShortCourse,
                IsRestrictedProvider = true
            }
        };

        providerCourseTypesRepository
            .Setup(r => r.GetProviderCourseTypesByUkprn(query.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var result = await sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}
