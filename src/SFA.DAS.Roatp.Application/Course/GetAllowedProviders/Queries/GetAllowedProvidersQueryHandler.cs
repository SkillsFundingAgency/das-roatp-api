using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;

public class GetAllowedProvidersQueryHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, IProviderCoursesReadRepository _providerCoursesReadRepository) : IRequestHandler<GetAllowedProvidersQuery, CourseAllowedProvidersModel>
{
    public async Task<CourseAllowedProvidersModel> Handle(GetAllowedProvidersQuery request, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        if (standard == null)
        {
            return null;
        }

        var isRestrictedCourse = standard.RestrictedCourseView != null;

        var providers = isRestrictedCourse
            ? await BuildRestrictedCourseProviders(request.LarsCode, cancellationToken)
            : await BuildNotRestrictedCourseProviders(request.LarsCode, cancellationToken);

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
            IsCourseRestricted = isRestrictedCourse,
            Providers = providers
        };
    }

    private async Task<List<ProviderModel>> BuildRestrictedCourseProviders(string larsCode, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken);

        return providerAllowedCourses
            .Select(pac => new ProviderModel
            {
                Ukprn = pac.Ukprn,
                ProviderName = pac.Provider.LegalName,
                DateLastStarts = pac.LastDateStarts
            })
            .ToList();
    }

    private async Task<List<ProviderModel>> BuildNotRestrictedCourseProviders(string larsCode, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken);
        var providerCourses = await _providerCoursesReadRepository.GetProviderCoursesByLarsCode(larsCode);

        return providerCourses
            .Select(pc => new ProviderModel
            {
                Ukprn = pc.Provider.Ukprn,
                ProviderName = pc.Provider.LegalName,
                DateLastStarts = providerAllowedCourses.FirstOrDefault(pac => pac.Ukprn == pc.Provider.Ukprn)?.LastDateStarts
            })
            .ToList();
    }
}
