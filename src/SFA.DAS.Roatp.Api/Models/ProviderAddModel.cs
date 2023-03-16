using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;

namespace SFA.DAS.Roatp.Api.Models;

public class ProviderAddModel
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    
    public static implicit operator CreateProviderCommand(ProviderAddModel source)
        => new()
        {
            Ukprn = source.Ukprn,
            LegalName = source.LegalName,
            TradingName = source.TradingName,
        };
}