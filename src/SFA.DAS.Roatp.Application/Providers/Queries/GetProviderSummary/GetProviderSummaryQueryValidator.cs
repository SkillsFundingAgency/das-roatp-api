using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary
{
    public class GetProviderSummaryQueryValidator : AbstractValidator<GetProviderSummaryQuery>
    {
        public GetProviderSummaryQueryValidator(IProvidersReadRepository providersReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));
        }
    }
}
