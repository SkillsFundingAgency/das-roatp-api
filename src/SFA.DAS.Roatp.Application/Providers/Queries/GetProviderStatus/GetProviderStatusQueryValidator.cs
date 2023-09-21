using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderStatus
{
    public class GetProviderStatusQueryValidator : AbstractValidator<GetProviderStatusQuery>
    {
        public GetProviderStatusQueryValidator(IProvidersReadRepository providersReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));
        }
    }
}
