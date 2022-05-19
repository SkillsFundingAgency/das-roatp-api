using FluentValidation;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationsQueryValidator : AbstractValidator<ProviderCourseLocationsQuery>
    {
        public const string InvalidProviderCourseIdErrorMessage = "Invalid ProviderCourseId";
        public ProviderCourseLocationsQueryValidator()
        {
            RuleFor(x => x.ProviderCourseId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage(InvalidProviderCourseIdErrorMessage);
        }
    }
}
