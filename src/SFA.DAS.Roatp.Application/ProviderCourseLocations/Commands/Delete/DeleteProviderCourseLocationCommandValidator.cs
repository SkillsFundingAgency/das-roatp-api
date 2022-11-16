using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommandValidator : AbstractValidator<DeleteProviderCourseLocationCommand>
    {
        public const string InvalidProviderCourseLocationIdErrorMessage = "Invalid location id";
        public const string ProviderCourseLocationNotFoundErrorMessage = "Location details not found for given location id";
        public DeleteProviderCourseLocationCommandValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository, IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            Include(new LarsCodeUkprnCombinationValidator(providersReadRepository, providerCoursesReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(c => c.LocationId).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage(InvalidProviderCourseLocationIdErrorMessage)
                .MustAsync(async (model, navigationId, cancellation) =>
                {
                    var providerCourseLocations = await providerCourseLocationsReadRepository.GetAllProviderCourseLocations(model.Ukprn, model.LarsCode);
                    return providerCourseLocations.Exists(l=>l.NavigationId == navigationId);
                })
               .WithMessage(ProviderCourseLocationNotFoundErrorMessage);
        }
    }
}
