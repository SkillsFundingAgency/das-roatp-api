using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProvidersNotAllowed;

public class GetProvidersNotAllowedQueryHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, IProviderCourseTypesRepository _providerCourseTypesReadRepository) : IRequestHandler<GetProvidersNotAllowedQuery, RestrictedCourseDetailsModel>
{
    public async Task<RestrictedCourseDetailsModel> Handle(GetProvidersNotAllowedQuery request, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        if (standard == null)
        {
            return null;
        }

        var providers = await BuildNotAllowedProviders(request.LarsCode, standard.CourseType, cancellationToken);

        return new RestrictedCourseDetailsModel
        {
            LarsCode = standard.LarsCode,
            IfateReferenceNumber = standard.IfateReferenceNumber,
            CourseName = standard.Title,
            Level = standard.Level,
            Route = standard.Route,
            LearningType = standard.LearningType,
            CourseType = standard.CourseType,
            IsActiveAvailable = standard.IsActiveAvailable,
            LastDateStarts = standard.LastDateStarts,
            IsCourseRestricted = standard.RestrictedCourseView != null,
            Providers = providers.ToList()
        };
    }

    private async Task<IEnumerable<ProviderModel>> BuildNotAllowedProviders(string larsCode, CourseType courseType, CancellationToken cancellationToken)
    {
        var providers = await _providerCourseTypesReadRepository.GetAllProvidersByCourseType(courseType, cancellationToken);
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken);

        return providers
            .Where(p => !providerAllowedCourses.Any(pac => pac.Ukprn == p.Provider.Ukprn && pac.LarsCode == larsCode))
            .Select(p => new ProviderModel
            {
                Ukprn = p.Provider.Ukprn,
                ProviderName = p.Provider.LegalName
            });
    }
}
