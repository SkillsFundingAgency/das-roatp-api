using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class GetProviderRestrictedApprenticeshipsQuery : IRequest<ValidatedResponse<GetProviderRestrictedApprenticeshipsQueryResult>>
{
    public int Ukprn { get; set; }
}
