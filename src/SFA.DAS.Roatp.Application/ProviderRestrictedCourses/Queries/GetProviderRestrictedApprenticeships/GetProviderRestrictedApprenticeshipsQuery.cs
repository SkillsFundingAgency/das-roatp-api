using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class GetProviderRestrictedApprenticeshipsQuery : IRequest<ValidatedResponse<GetProviderRestrictedApprenticeshipsQueryResult>>, IProviderCourseTypeRestriction
{
    public int Ukprn { get; set; }
    public CourseType CourseType { get; set; }
}
