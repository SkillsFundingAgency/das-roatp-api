using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkDeleteProviderCourseLocationsCommandValidator : AbstractValidator<BulkInsertProviderCourseLocationsCommand>
    {
        public BulkDeleteProviderCourseLocationsCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
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
