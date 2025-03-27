using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryHandler(ICourseProviderDetailsReadRepository _courseProviderDetailsReadRepository) : IRequestHandler<GetCourseProviderDetailsQuery, ValidatedResponse<GetCourseProviderDetailsQueryResult>>
{
    public async Task<ValidatedResponse<GetCourseProviderDetailsQueryResult>> Handle(GetCourseProviderDetailsQuery query, CancellationToken cancellationToken)
    {
        var providerDetails = await _courseProviderDetailsReadRepository.GetCourseProviderDetails(
            new GetCourseProviderDetailsParameters()
            {
                LarsCode = query.LarsCode,
                Ukprn = query.Ukprn,
                Lat = query.Latitude,
                Lon = query.Longitude,
                Location = query.Location,
                ShortlistUserId = query.ShortlistUserId
            },
            cancellationToken
        );

        if(providerDetails.Count < 1)
        {
            return new ValidatedResponse<GetCourseProviderDetailsQueryResult>((GetCourseProviderDetailsQueryResult)null);
        }

        var result = (GetCourseProviderDetailsQueryResult)providerDetails[0];

        result.Locations = providerDetails.Select(model => (LocationModel)model);

        return new ValidatedResponse<GetCourseProviderDetailsQueryResult>(result);
    }
}
