using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseForecasts.Command.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastModelValidatorTests
{
    private readonly UpsertProviderCourseForecastModelValidator _sut = new();

    [TestCase("", UpsertProviderCourseForecastModelValidator.TimePeriodIsRequiredErrorMessage)]
    [TestCase("  ", UpsertProviderCourseForecastModelValidator.TimePeriodIsRequiredErrorMessage)]
    [TestCase(null, UpsertProviderCourseForecastModelValidator.TimePeriodIsRequiredErrorMessage)]
    [TestCase("AY21", UpsertProviderCourseForecastModelValidator.TimePeriodShouldBeCorrectFormatErrorMessage)]
    [TestCase("2021", UpsertProviderCourseForecastModelValidator.TimePeriodShouldBeCorrectFormatErrorMessage)]
    [TestCase("AY202A", UpsertProviderCourseForecastModelValidator.TimePeriodShouldBeCorrectFormatErrorMessage)]
    public void Validate_TimePeriod_ReturnsErrorForInvalidValues(string timePeriod, string expectedErrorMessage)
    {
        // Arrange
        var model = new UpsertProviderCourseForecastModel(timePeriod, 1, null);
        // Act
        var result = _sut.TestValidate(model);
        // Assert
        result.ShouldHaveValidationErrorFor(m => m.TimePeriod).WithErrorMessage(expectedErrorMessage);
    }

    [TestCase(0, UpsertProviderCourseForecastModelValidator.QuarterIsRequiredErrorMessage)]
    [TestCase(5, UpsertProviderCourseForecastModelValidator.QuarterMustBeAValidValueErrorMessage)]
    [TestCase(-1, UpsertProviderCourseForecastModelValidator.QuarterMustBeAValidValueErrorMessage)]
    public void Validate_Quarter_ReturnsErrorForInvalidValues(int quarter, string expectedErrorMessage)
    {
        // Arrange
        var model = new UpsertProviderCourseForecastModel("AY2021", quarter, 10);
        // Act
        var result = _sut.TestValidate(model);
        // Assert
        result.ShouldHaveValidationErrorFor(m => m.Quarter).WithErrorMessage(expectedErrorMessage);
    }

    [Test]
    public void Validate_EstimatedLearners_ReturnsErrorForNegativeValues()
    {
        // Arrange
        var model = new UpsertProviderCourseForecastModel("AY2021", 1, -1);
        // Act
        var result = _sut.TestValidate(model);
        // Assert
        result.ShouldHaveValidationErrorFor(m => m.EstimatedLearners).WithErrorMessage(UpsertProviderCourseForecastModelValidator.EstimatedLearnersMustBeValidNumberErrorMessage);
    }

    [TestCase(1, 0)]
    [TestCase(2, null)]
    [TestCase(3, 10)]
    [TestCase(4, null)]
    public void Validate_WhenModelIsValid_ReturnsSuccess(int quarter, int estimatedLearners)
    {
        // Arrange
        var model = new UpsertProviderCourseForecastModel("AY2021", quarter, estimatedLearners);
        // Act
        var result = _sut.TestValidate(model);
        // Assert
        Assert.IsTrue(result.IsValid);
    }
}
