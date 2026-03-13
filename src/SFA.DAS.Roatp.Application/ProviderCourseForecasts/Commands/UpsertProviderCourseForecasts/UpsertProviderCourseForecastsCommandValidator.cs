using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastsCommandValidator : AbstractValidator<UpsertProviderCourseForecastsCommand>
{
    public const string MissingForecastsErrorMessage = "At least one forecast must be provided";
    public const string MaximumAllowedForecastErrorMessage = "A maximum of 4 forecasts can be provided";

    public UpsertProviderCourseForecastsCommandValidator(
        IProvidersReadRepository providersReadRepository,
        IProviderCourseTypesReadRepository providerCourseTypesReadRepository,
        IStandardsReadRepository standardsReadRepository,
        IProviderAllowedCoursesRepository providerAllowedCoursesRepository,
        IProviderCoursesReadRepository providerCoursesReadRepository,
        IForecastQuartersRepository forecastQuartersRepository)
    {
        Include(new UkprnValidator(providersReadRepository));
        RuleFor(x => x.LarsCode)
            .Cascade(CascadeMode.Stop)
            .MustBeAShortCourseType(standardsReadRepository)
            .MustBeAllowedToDeliverTheShortCourse(standardsReadRepository, providerCourseTypesReadRepository, providerAllowedCoursesRepository)
            .MustBeAddedToTheProviderProfile(providerCoursesReadRepository);
        RuleFor(x => x.Forecasts)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(MissingForecastsErrorMessage)
            .MustAsync(async (forecasts, cancellationToken) =>
            {
                var quarters = await forecastQuartersRepository.GetForecastQuarters(cancellationToken);
                return forecasts.Count() <= quarters.Count;
            })
            .WithMessage(MaximumAllowedForecastErrorMessage);
        RuleForEach(x => x.Forecasts)
            .SetValidator(new UpsertProviderCourseForecastModelValidator());
    }
}
