namespace SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;


public class GetLatestProviderContactQueryResult
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }

    public static implicit operator GetLatestProviderContactQueryResult(Domain.Entities.ProviderContact source)
    {
        if (source == null) return null;

        return new GetLatestProviderContactQueryResult
        {
            EmailAddress = source.EmailAddress,
            PhoneNumber = source.PhoneNumber,
        };
    }
}