using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert
{
    public class BulkInsertProviderLocationsCommandValidator : AbstractValidator<BulkInsertProviderLocationsCommand>
    {
        public BulkInsertProviderLocationsCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
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
