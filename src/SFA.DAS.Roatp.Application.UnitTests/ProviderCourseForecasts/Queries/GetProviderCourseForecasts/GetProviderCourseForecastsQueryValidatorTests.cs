using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryValidatorTests
{
    [TestCase(123456789, UkprnValidator.InvalidUkprnErrorMessage)]
    [TestCase(1234567, UkprnValidator.InvalidUkprnErrorMessage)]
    [TestCase(10012002, UkprnValidator.ProviderNotFoundErrorMessage)]
    public async Task Validate_Ukprn_ReturnsErrorForInvalidNumbers(int ukprn, string errorMessage)
    {
        var sut = new GetProviderCourseForecastsQueryValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCourseTypesReadRepository>(), Mock.Of<IStandardsReadRepository>(), Mock.Of<IProviderAllowedCoursesRepository>());

        var result = await sut.TestValidateAsync(new GetProviderCourseForecastsQuery(ukprn, "ZSC10001"), options => options.IncludeProperties(nameof(GetProviderCourseForecastsQuery.Ukprn)));

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Ukprn).WithErrorMessage(errorMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Validate_Ukprn_ReturnsValidResponse(Provider provider)
    {
        var ukprn = 10012002;
        Mock<IProvidersReadRepository> providersRepoMock = new();
        providersRepoMock.Setup(x => x.GetByUkprn(ukprn)).ReturnsAsync(provider);
        var sut = new GetProviderCourseForecastsQueryValidator(providersRepoMock.Object, Mock.Of<IProviderCourseTypesReadRepository>(), Mock.Of<IStandardsReadRepository>(), Mock.Of<IProviderAllowedCoursesRepository>());

        var result = await sut.TestValidateAsync(new GetProviderCourseForecastsQuery(ukprn, "ZSC10001"), options => options.IncludeProperties(nameof(GetProviderCourseForecastsQuery.Ukprn)));

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Ukprn);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Validate_LarsCode_ReturnsErrorForNonShortCourseLarsCode(GetProviderCourseForecastsQuery query, Standard standard)
    {
        standard.CourseType = CourseType.Apprenticeship;
        Mock<IStandardsReadRepository> standardsRepoMock = new();
        standardsRepoMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);
        var sut = new GetProviderCourseForecastsQueryValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCourseTypesReadRepository>(), standardsRepoMock.Object, Mock.Of<IProviderAllowedCoursesRepository>());

        var result = await sut.TestValidateAsync(query, options => options.IncludeProperties(nameof(GetProviderCourseForecastsQuery.LarsCode)));

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.LarsCode).WithErrorMessage(ShortCourseValidator.MustBeAShortCourseTypeValidationMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Validate_LarsCode_ReturnsErrorForShortCourseNotAllowed(GetProviderCourseForecastsQuery query, Standard standard)
    {
        standard.CourseType = CourseType.ShortCourse;
        Mock<IStandardsReadRepository> standardsRepoMock = new();
        standardsRepoMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);

        Mock<IProviderCourseTypesReadRepository> courseTypesRepoMock = new();
        courseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderCourseType> { new ProviderCourseType { CourseType = CourseType.Apprenticeship } });

        var sut = new GetProviderCourseForecastsQueryValidator(Mock.Of<IProvidersReadRepository>(), courseTypesRepoMock.Object, standardsRepoMock.Object, Mock.Of<IProviderAllowedCoursesRepository>());

        var result = await sut.TestValidateAsync(query, options => options.IncludeProperties(nameof(GetProviderCourseForecastsQuery.LarsCode)));

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.LarsCode).WithErrorMessage(ShortCourseValidator.MustBeAllowedToDeliverTheShortCourseValidationMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Validate_LarsCode_ReturnsErrorForCourseNotAllowed(GetProviderCourseForecastsQuery query, Standard standard)
    {
        standard.CourseType = CourseType.ShortCourse;
        Mock<IStandardsReadRepository> standardsRepoMock = new();
        standardsRepoMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);

        Mock<IProviderCourseTypesReadRepository> courseTypesRepoMock = new();
        courseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderCourseType> { new ProviderCourseType { CourseType = CourseType.ShortCourse } });

        Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock = new();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, CourseType.ShortCourse, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderAllowedCourse>());

        var sut = new GetProviderCourseForecastsQueryValidator(Mock.Of<IProvidersReadRepository>(), courseTypesRepoMock.Object, standardsRepoMock.Object, providerAllowedCoursesRepoMock.Object);

        var result = await sut.TestValidateAsync(query, options => options.IncludeProperties(nameof(GetProviderCourseForecastsQuery.LarsCode)));

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.LarsCode).WithErrorMessage(ShortCourseValidator.MustBeAllowedToDeliverTheShortCourseValidationMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Validate_LarsCode_ReturnsValidAsCourseAllowed(GetProviderCourseForecastsQuery query, Standard standard)
    {
        standard.CourseType = CourseType.ShortCourse;
        Mock<IStandardsReadRepository> standardsRepoMock = new();
        standardsRepoMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);

        Mock<IProviderCourseTypesReadRepository> courseTypesRepoMock = new();
        courseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderCourseType> { new ProviderCourseType { CourseType = CourseType.ShortCourse } });

        Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock = new();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, CourseType.ShortCourse, It.IsAny<CancellationToken>())).ReturnsAsync([new ProviderAllowedCourse { LarsCode = query.LarsCode }]);

        var sut = new GetProviderCourseForecastsQueryValidator(Mock.Of<IProvidersReadRepository>(), courseTypesRepoMock.Object, standardsRepoMock.Object, providerAllowedCoursesRepoMock.Object);

        var result = await sut.TestValidateAsync(query, options => options.IncludeProperties(nameof(GetProviderCourseForecastsQuery.LarsCode)));

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.LarsCode);
    }
}
