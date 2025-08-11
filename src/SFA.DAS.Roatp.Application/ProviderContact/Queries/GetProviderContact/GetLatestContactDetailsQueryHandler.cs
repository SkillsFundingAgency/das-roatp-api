using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;

public class GetLatestProviderContactQueryHandler(IProviderContactsReadRepository _providerContactsReadRepository) : IRequestHandler<GetLatestProviderContactQuery, ValidatedResponse<GetLatestProviderContactQueryResult>>
{
    public async Task<ValidatedResponse<GetLatestProviderContactQueryResult>> Handle(GetLatestProviderContactQuery query, CancellationToken cancellationToken)
    {
        var providerContacts = await _providerContactsReadRepository.GetLatestProviderContact(query.Ukprn);

        GetLatestProviderContactQueryResult response = providerContacts;
        return new ValidatedResponse<GetLatestProviderContactQueryResult>(response);
    }
}

