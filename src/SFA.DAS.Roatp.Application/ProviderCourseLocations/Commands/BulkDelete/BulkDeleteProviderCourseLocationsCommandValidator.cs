using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommandValidator : AbstractValidator<BulkDeleteProviderCourseLocationsCommand>
    {
        public BulkDeleteProviderCourseLocationsCommandValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            Include(new LarsCodeUkprnCheckerValidator(providersReadRepository, providerCoursesReadRepository));

            Include(new UserInfoValidator());

            RuleFor(c => c.DeleteProviderCourseLocationOptions)
                .NotEqual(DeleteProviderCourseLocationOption.None)
                .WithMessage("Delete option not specified, expected DeleteProviderLocations or DeleteEmployerLocations");
        }
    }
}
