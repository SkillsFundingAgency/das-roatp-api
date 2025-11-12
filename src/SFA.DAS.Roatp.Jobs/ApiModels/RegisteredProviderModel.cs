using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Jobs.ApiModels;
public class RegisteredProviderModel
{
    public int Ukprn { get; set; }
    public ProviderStatusType Status { get; set; }
    public DateTime StatusDate { get; set; }
    public int OrganisationTypeId { get; set; }
    public ProviderType ProviderType { get; set; }
    public string LegalName { get; set; }

    public static implicit operator ProviderRegistrationDetail(RegisteredProviderModel source)
        => new ProviderRegistrationDetail
        {
            Ukprn = source.Ukprn,
            StatusId = (int)source.Status,
            StatusDate = source.StatusDate,
            OrganisationTypeId = source.OrganisationTypeId,
            ProviderTypeId = (int)source.ProviderType,
            LegalName = source.LegalName,
        };
}
