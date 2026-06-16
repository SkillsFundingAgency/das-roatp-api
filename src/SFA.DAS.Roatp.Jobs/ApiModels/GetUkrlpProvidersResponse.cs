namespace SFA.DAS.Roatp.Jobs.ApiModels;

public class GetUkrlpProvidersResponse
{
    public IEnumerable<ProviderDetails> Providers { get; set; } = [];
}

public class ProviderDetails
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public Address LegalAddress { get; set; }
    public ProviderContactModel PrimaryContact { get; set; }
}
public record ProviderContactModel(string Title, string FirstName, string LastName, string Email, string Telephone, string Website);

public record Address(string Address1, string Address2, string Address3, string Address4, string Town, string County, string Postcode);
