using System;

namespace SFA.DAS.Roatp.Domain.Entities;

public class ProviderContact
{
    public long Id { get; set; }
    public int ProviderId { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual Provider Provider { get; set; }
}