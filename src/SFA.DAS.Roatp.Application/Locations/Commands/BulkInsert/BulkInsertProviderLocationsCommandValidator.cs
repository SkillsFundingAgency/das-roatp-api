using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert
{
    public class BulkInsertProviderLocationsCommandValidator : AbstractValidator<BulkInsertProviderLocationsCommand>
    {
        public BulkInsertProviderLocationsCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();
        }
    }
}
