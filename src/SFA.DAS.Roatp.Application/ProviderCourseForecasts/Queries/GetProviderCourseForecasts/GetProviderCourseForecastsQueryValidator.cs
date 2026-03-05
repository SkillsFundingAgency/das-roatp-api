using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryValidator : AbstractValidator<GetProviderCourseForecastsQuery>
{
    public GetProviderCourseForecastsQueryValidator(IProvidersReadRepository providersReadRepository, IProviderCourseTypesReadRepository providerCourseTypesReadRepository, IStandardsReadRepository standardsReadRepository, IProviderAllowedCoursesRepository providerAllowedCoursesRepository)
    {
        Include(new UkprnValidator(providersReadRepository));
        RuleFor(x => x.LarsCode)
            .Cascade(CascadeMode.Stop)
            .MustBeAShortCourseType(standardsReadRepository)
            .MustBeAllowedToDeliverTheShortCourse(standardsReadRepository, providerCourseTypesReadRepository, providerAllowedCoursesRepository);
    }
}
