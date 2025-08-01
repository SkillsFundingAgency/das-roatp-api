using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;

public class GetLatestProviderContactQueryHandler(IContactDetailsReadRepository _contactDetailsReadRepository) : IRequestHandler<GetLatestProviderContactQuery, ValidatedResponse<GetLatestProviderContactQueryResult>>
{
    public async Task<ValidatedResponse<GetLatestProviderContactQueryResult>> Handle(GetLatestProviderContactQuery query, CancellationToken cancellationToken)
    {
        var contactDetails = await _contactDetailsReadRepository.GetLatestProviderContact(query.Ukprn);

        GetLatestProviderContactQueryResult response = contactDetails;
        return new ValidatedResponse<GetLatestProviderContactQueryResult>(response);
    }
}

