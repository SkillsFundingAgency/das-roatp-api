using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderRegisterDetailsLookup;

public class GetProviderRegisterDetailsLookupQueryValidator : AbstractValidator<GetProviderRegisterDetailsLookupQuery>
{
    public GetProviderRegisterDetailsLookupQueryValidator(IProvidersReadRepository providersReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));
    }
}
