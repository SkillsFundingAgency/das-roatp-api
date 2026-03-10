using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseForecasts.Command.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastsCommandValidatorTests
{
    private const int ValidUkprn = 10012002;
    private const string ValidLarsCode = "ZSC10001";
    private readonly UpsertProviderCourseForecastModel ValidForecast = new("AY2627", 1, 10);

    [Test]
    public async Task Validate_WhenCommandIsValid_ReturnsSuccess()
    {
        // Arrange
        var sut = GetValidator();
        var command = new UpsertProviderCourseForecastsCommand(ValidUkprn, ValidLarsCode, [ValidForecast]);
        // Act
        var result = await sut.TestValidateAsync(command);
        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public async Task ValidateCourse_CourseNotAddedByProvider_IsInvalid()
    {
        // Arrange
        var providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
        providerCoursesReadRepositoryMock.Setup(x => x.GetProviderCourseByUkprn(ValidUkprn, ValidLarsCode)).ReturnsAsync((Domain.Entities.ProviderCourse)null);
        var sut = GetValidator(providerCoursesReadRepositoryMock);
        var command = new UpsertProviderCourseForecastsCommand(ValidUkprn, ValidLarsCode, [ValidForecast]);
        // Act
        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(UpsertProviderCourseForecastsCommand.LarsCode)));
        // Assert
        result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(ShortCourseValidator.MustBeAddedToTheProviderProfileValidationMessage);
    }

    [Test]
    public async Task ValidateCourse_CourseIsAddedByProvider_IsValid()
    {
        // Arrange
        var providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
        providerCoursesReadRepositoryMock.Setup(x => x.GetProviderCourseByUkprn(ValidUkprn, ValidLarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());
        var sut = GetValidator(providerCoursesReadRepositoryMock, null);
        var command = new UpsertProviderCourseForecastsCommand(ValidUkprn, ValidLarsCode, [ValidForecast]);
        // Act
        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(UpsertProviderCourseForecastsCommand.LarsCode)));
        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
    }

    [Test]
    public async Task ValidateForecasts_WhenEmpty_IsInvalid()
    {
        // Arrange
        var sut = GetValidator();
        var command = new UpsertProviderCourseForecastsCommand(ValidUkprn, ValidLarsCode, []);
        // Act
        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(UpsertProviderCourseForecastsCommand.Forecasts)));
        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Forecasts).WithErrorMessage(UpsertProviderCourseForecastsCommandValidator.MissingForecastsErrorMessage);
    }

    [Test]
    public async Task ValidateForecasts_WhenTooMany_IsInvalid()
    {
        // Arrange
        var forecastQuartersRepositoryMock = new Mock<IForecastQuartersRepository>();
        forecastQuartersRepositoryMock.Setup(x => x.GetForecastQuarters(It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new ForecastQuarter { Quarter = 2, TimePeriod = "AY2627" },
            new ForecastQuarter { Quarter = 3, TimePeriod = "AY2627" }
        ]);
        var sut = GetValidator(null, forecastQuartersRepositoryMock);
        var command = new UpsertProviderCourseForecastsCommand(ValidUkprn, ValidLarsCode, [ValidForecast, ValidForecast, ValidForecast]);
        // Act
        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(UpsertProviderCourseForecastsCommand.Forecasts)));
        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Forecasts).WithErrorMessage(UpsertProviderCourseForecastsCommandValidator.MaximumAllowedForecastErrorMessage);
    }

    [Test]
    public async Task ValidateForecasts_CountMatchesQuarters_IsValid()
    {
        // Arrange
        var forecastQuartersRepositoryMock = new Mock<IForecastQuartersRepository>();
        forecastQuartersRepositoryMock.Setup(x => x.GetForecastQuarters(It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new ForecastQuarter { Quarter = 2, TimePeriod = "AY2627" },
            new ForecastQuarter { Quarter = 3, TimePeriod = "AY2627" }
        ]);
        var sut = GetValidator(null, forecastQuartersRepositoryMock);
        var command = new UpsertProviderCourseForecastsCommand(ValidUkprn, ValidLarsCode, [ValidForecast, ValidForecast]);
        // Act
        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(UpsertProviderCourseForecastsCommand.Forecasts)));
        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Forecasts);
    }

    private static UpsertProviderCourseForecastsCommandValidator GetValidator(
        Mock<IProviderCoursesReadRepository> providerCoursesReadRepositoryMock = null,
        Mock<IForecastQuartersRepository> forecastQuartersRepositoryMock = null)
    {
        var ukprn = 10012002;
        Mock<IProvidersReadRepository> providersRepoMock = new();
        providersRepoMock.Setup(x => x.GetByUkprn(ukprn)).ReturnsAsync(new Provider());

        Mock<IStandardsReadRepository> standardsRepoMock = new();
        standardsRepoMock.Setup(x => x.GetStandard(It.IsAny<string>())).ReturnsAsync(new Standard() { CourseType = CourseType.ShortCourse });

        Mock<IProviderCourseTypesReadRepository> courseTypesRepoMock = new();
        courseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(ValidUkprn, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderCourseType> { new ProviderCourseType { CourseType = CourseType.ShortCourse } });

        Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock = new();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(ValidUkprn, CourseType.ShortCourse, It.IsAny<CancellationToken>())).ReturnsAsync([new ProviderAllowedCourse { LarsCode = ValidLarsCode }]);

        if (providerCoursesReadRepositoryMock == null)
        {
            providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
            providerCoursesReadRepositoryMock.Setup(x => x.GetProviderCourseByUkprn(ValidUkprn, ValidLarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());
        }
        if (forecastQuartersRepositoryMock == null)
        {
            forecastQuartersRepositoryMock = new Mock<IForecastQuartersRepository>();
            forecastQuartersRepositoryMock.Setup(x => x.GetForecastQuarters(It.IsAny<CancellationToken>())).ReturnsAsync(
            [
                new ForecastQuarter { Quarter = 4, TimePeriod = "AY2526" },
                new ForecastQuarter { Quarter = 1, TimePeriod = "AY2627" },
                new ForecastQuarter { Quarter = 2, TimePeriod = "AY2627" },
                new ForecastQuarter { Quarter = 3, TimePeriod = "AY2627" }
            ]);
        }
        return new UpsertProviderCourseForecastsCommandValidator(
            providersRepoMock.Object,
            courseTypesRepoMock.Object,
            standardsRepoMock.Object,
            providerAllowedCoursesRepoMock.Object,
            providerCoursesReadRepositoryMock?.Object ?? Mock.Of<IProviderCoursesReadRepository>(),
            forecastQuartersRepositoryMock?.Object ?? Mock.Of<IForecastQuartersRepository>());
    }
}
