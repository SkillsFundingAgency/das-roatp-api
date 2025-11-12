using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;

public class GetRegisteredProviderQueryResult
{
    public int Ukprn { get; set; }
    public string Name { get; set; }
    public string TradingName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ContactUrl { get; set; }
    public int ProviderTypeId { get; set; }
    public int StatusId { get; set; }
    /// <summary>
    /// Read only property to get the value of Provider can access Apprenticeship Service.
    /// </summary>
    public bool CanAccessApprenticeshipService =>
        (ProviderType)ProviderTypeId is ProviderType.Main or ProviderType.Employer
        && (ProviderStatusType)StatusId is ProviderStatusType.Active or ProviderStatusType.OnBoarding or ProviderStatusType.ActiveNoStarts;
    public ProviderAddressModel Address { get; set; } = new ProviderAddressModel();

    public static implicit operator GetRegisteredProviderQueryResult(ProviderRegistrationDetail source) =>
        source == null ? null : new GetRegisteredProviderQueryResult
        {
            Ukprn = source.Ukprn,
            Name = source.LegalName,
            TradingName = source.Provider?.TradingName,
            Email = source.Provider?.Email,
            Phone = source.Provider?.Phone,
            ContactUrl = source.Provider?.Website,
            ProviderTypeId = source.ProviderTypeId,
            StatusId = source.StatusId,
            Address = source
        };
}
