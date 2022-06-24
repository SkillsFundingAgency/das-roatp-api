using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class BulkDeleteProviderLocationsCommandValidator : AbstractValidator<BulkDeleteProviderLocationsCommand>
    {
        public BulkDeleteProviderLocationsCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            RuleFor(c => c.Ukprn)
                .GreaterThan(10000000)
                .LessThan(99999999);

            RuleFor(c => c.LarsCode)
                .GreaterThan(0);

            RuleFor(c => c.UserId).NotEmpty();
        }
    }
}
