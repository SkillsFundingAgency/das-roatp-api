using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Domain.Entities;

[ExcludeFromCodeCoverage]
public class ProviderEmployerStars
{
    public string TimePeriod { get; set; }
    public long Ukprn { get; set; }
    public int ReviewCount { get; set; }
    public int Stars { get; set; }
    public string Rating { get; set; }
}
