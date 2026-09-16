using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;

public class GetProviderNotRestrictedApprenticeshipsQueryHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<GetProviderNotRestrictedApprenticeshipsQuery, ValidatedResponse<GetProviderNotRestrictedApprenticeshipsQueryResult>>
{
    public async Task<ValidatedResponse<GetProviderNotRestrictedApprenticeshipsQueryResult>> Handle(GetProviderNotRestrictedApprenticeshipsQuery request, CancellationToken cancellationToken)
    {
        var standards = await _standardsReadRepository.GetCoursesByCourseType(request.CourseType, cancellationToken);

        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(request.Ukprn, request.CourseType, cancellationToken);


        var notRestrictedApprenticeships = standards
            .Where(s =>
                (s.RestrictedCourseView != null &&
                 providerAllowedCourses.Any(pac =>
                     pac.Ukprn == request.Ukprn &&
                     pac.LarsCode == s.LarsCode &&
                     pac.LastDateStarts == null))
                ||
                (s.RestrictedCourseView == null &&
                 !providerAllowedCourses.Any(pac =>
                     pac.Ukprn == request.Ukprn &&
                     pac.LarsCode == s.LarsCode &&
                     pac.LastDateStarts != null)))
            .Select(s => (NotRestrictedApprenticeshipModel)s).ToList();

        var response = new GetProviderNotRestrictedApprenticeshipsQueryResult() { Courses = notRestrictedApprenticeships };

        return new ValidatedResponse<GetProviderNotRestrictedApprenticeshipsQueryResult>(response);
    }
}
