using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;

public class CreateProviderCommand : IRequest<ValidatedResponse<int>>, IUkprn, IUserInfo
{
    // validator to check it isn't in providers?
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }

    public static implicit operator Domain.Entities.Provider(CreateProviderCommand source)
        => new()
        {
            Ukprn = source.Ukprn,
            LegalName = source.LegalName,
            TradingName = source.TradingName,
            IsImported = false
        };
}