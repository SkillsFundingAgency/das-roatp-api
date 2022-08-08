using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Provider.Queries.GetProvider
{
    public class GetProviderQueryValidator : AbstractValidator<GetProviderQuery>
    {
        public GetProviderQueryValidator(IProviderReadRepository providerReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));
        }
    }
}
