using FluentValidation;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastModelValidator : AbstractValidator<UpsertProviderCourseForecastModel>
{
    public const string TimePeriodIsRequiredErrorMessage = "Time period must be provided";
    public const string TimePeriodShouldBeCorrectFormatErrorMessage = "Time period must be in the format 'AY' followed by 4 digits, e.g. AY2526";
    public const string QuarterMustBeAValidValueErrorMessage = "Quarter must be between 1 and 4";
    public const string QuarterIsRequiredErrorMessage = "Quarter must be provided";
    public const string EstimatedLearnersMustBeValidNumberErrorMessage = "Estimated learners must be a non-negative number";

    public UpsertProviderCourseForecastModelValidator()
    {
        RuleFor(x => x.TimePeriod)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(TimePeriodIsRequiredErrorMessage)
            .Matches(@"^(AY)\d{4}$")
            .WithMessage(TimePeriodShouldBeCorrectFormatErrorMessage);

        RuleFor(x => x.Quarter)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(QuarterIsRequiredErrorMessage)
            .InclusiveBetween(1, 4)
            .WithMessage(QuarterMustBeAValidValueErrorMessage);

        RuleFor(x => x.EstimatedLearners)
            .GreaterThanOrEqualTo(0)
            .WithMessage(EstimatedLearnersMustBeValidNumberErrorMessage);
    }
}
