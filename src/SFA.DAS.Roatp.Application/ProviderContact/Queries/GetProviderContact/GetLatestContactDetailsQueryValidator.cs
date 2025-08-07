using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;
public class GetLatestProviderContactQueryValidator : AbstractValidator<GetLatestProviderContactQuery>
{
    public GetLatestProviderContactQueryValidator(IProvidersReadRepository providersReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));
    }
}
