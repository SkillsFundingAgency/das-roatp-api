using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Course.GetProvidersNotAllowed.Queries;

public class GetProvidersNotAllowedQueryHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, IProviderCourseTypesReadRepository _providerCourseTypesReadRepository) : IRequestHandler<GetProvidersNotAllowedQuery, CourseAllowedProvidersModel>
{
    public async Task<CourseAllowedProvidersModel> Handle(GetProvidersNotAllowedQuery request, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        if (standard == null)
        {
            return null;
        }

        var providers = await BuildNotAllowedProviders(request.LarsCode, standard.CourseType, cancellationToken);

        return new CourseAllowedProvidersModel
        {
            LarsCode = standard.LarsCode,
            IfateReferenceNumber = standard.IfateReferenceNumber,
            CourseName = standard.Title,
            Route = standard.Route,
            LearningType = standard.LearningType,
            CourseType = standard.CourseType,
            IsActiveAvailable = standard.IsActiveAvailable,
            DateLastStarts = standard.LastDateStarts,
            IsCourseRestricted = standard.RestrictedCourseView != null,
            Providers = providers
        };
    }

    private async Task<List<ProviderModel>> BuildNotAllowedProviders(string larsCode, CourseType courseType, CancellationToken cancellationToken)
    {
        var providers = await _providerCourseTypesReadRepository.GetAllProvidersByCourseType(courseType, cancellationToken);
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken);

        return providers
            .Where(p => !providerAllowedCourses.Any(pac => pac.Ukprn == p.Provider.Ukprn && pac.LarsCode == larsCode))
            .Select(p => new ProviderModel
            {
                Ukprn = p.Provider.Ukprn,
                ProviderName = p.Provider.LegalName
            })
            .ToList();
    }
}
