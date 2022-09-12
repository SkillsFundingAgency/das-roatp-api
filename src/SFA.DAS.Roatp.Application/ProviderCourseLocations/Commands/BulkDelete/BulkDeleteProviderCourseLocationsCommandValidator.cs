using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommandValidator : AbstractValidator<BulkDeleteProviderCourseLocationsCommand>
    {
        public BulkDeleteProviderCourseLocationsCommandValidator(IProvidersReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(c => c.DeleteProviderCourseLocationOptions)
                .NotEqual(DeleteProviderCourseLocationOption.None)
                .WithMessage("Delete option not specified, expected DeleteProviderLocations or DeleteEmployerLocations");
        }
    }
}
