using System;

namespace SFA.DAS.Roatp.Domain.Models;

public class ProviderModel
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public DateTime? LastDateStarts { get; set; }
}
