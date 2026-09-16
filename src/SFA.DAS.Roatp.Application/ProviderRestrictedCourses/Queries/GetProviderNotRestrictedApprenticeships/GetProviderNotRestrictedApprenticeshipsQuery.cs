using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;

public class GetProviderNotRestrictedApprenticeshipsQuery : IRequest<ValidatedResponse<GetProviderNotRestrictedApprenticeshipsQueryResult>>, IProviderCourseTypeRestriction
{
    public int Ukprn { get; set; }
    public CourseType CourseType { get; set; } = CourseType.Apprenticeship;
}