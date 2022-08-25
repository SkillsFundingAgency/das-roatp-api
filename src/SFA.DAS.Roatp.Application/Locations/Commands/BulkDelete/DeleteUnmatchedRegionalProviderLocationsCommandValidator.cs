using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class DeleteUnmatchedRegionalProviderLocationsCommandValidator : AbstractValidator<DeleteUnmatchedRegionalProviderLocationsCommand>
    {
        public DeleteUnmatchedRegionalProviderLocationsCommandValidator(IProviderReadRepository providerReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            RuleFor(c => c.UserId).NotEmpty();
        }
    }
}