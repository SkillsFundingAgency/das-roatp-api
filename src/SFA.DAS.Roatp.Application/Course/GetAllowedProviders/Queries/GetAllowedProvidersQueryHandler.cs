using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;

public class GetAllowedProvidersQueryHandler(IStandardsReadRepository _standardsReadRepository, IRestrictedCourseViewRepository _restrictedCourseViewRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, IProviderCoursesReadRepository _providerCoursesReadRepository, IProviderCourseTypesReadRepository _providerCourseTypesReadRepository) : IRequestHandler<GetAllowedProvidersQuery, ValidatedResponse<GetAllowedProvidersQueryResult>>
{
    public async Task<ValidatedResponse<GetAllowedProvidersQueryResult>> Handle(GetAllowedProvidersQuery request, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        var isRestrictedCourse = (await _restrictedCourseViewRepository.GetRestrictedCourses(cancellationToken)).Any(x => x.LarsCode == request.LarsCode);

        var providers = isRestrictedCourse
            ? await BuildRestrictedCourseProviders(request.LarsCode, standard.CourseType, cancellationToken)
            : await BuildNotRestrictedCourseProviders(request.LarsCode, standard.CourseType, cancellationToken);

        return new ValidatedResponse<GetAllowedProvidersQueryResult>(new GetAllowedProvidersQueryResult
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
        });
    }

    private async Task<List<ProviderModel>> BuildRestrictedCourseProviders(string larsCode, CourseType courseType, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken);
        var providerCourseTypes = await _providerCourseTypesReadRepository.GetAllProviderCourseTypes(cancellationToken);

        return providerAllowedCourses
            .Where(pac => providerCourseTypes.Any(pct =>
                pct.Ukprn == pac.Ukprn &&
                pct.CourseType == courseType))
            .Select(pac => new ProviderModel
            {
                Ukprn = pac.Ukprn,
                ProviderName = pac.Provider.LegalName,
                DateLastStarts = pac.LastDateStarts
            })
            .ToList();
    }

    private async Task<List<ProviderModel>> BuildNotRestrictedCourseProviders(string larsCode, CourseType courseType, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken);
        var providerCourses = await _providerCoursesReadRepository.GetProviderCoursesByLarsCode(larsCode);
        var providerCourseTypes = await _providerCourseTypesReadRepository.GetAllProviderCourseTypes(cancellationToken);

        return providerCourses
            .Where(pc => providerCourseTypes.Any(pct =>
                pct.Ukprn == pc.Provider.Ukprn &&
                pct.CourseType == courseType))
            .Select(pc => new ProviderModel
            {
                Ukprn = pc.Provider.Ukprn,
                ProviderName = pc.Provider.LegalName,
                DateLastStarts = providerAllowedCourses.FirstOrDefault(pac => pac.Ukprn == pc.Provider.Ukprn)?.LastDateStarts
            })
            .ToList();
    }
}
