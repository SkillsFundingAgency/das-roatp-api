using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetAllowedProviders;

public class GetAllowedProvidersQueryHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, IProviderCoursesReadRepository _providerCoursesReadRepository) : IRequestHandler<GetAllowedProvidersQuery, RestrictedCourseDetailsModel>
{
    public async Task<RestrictedCourseDetailsModel> Handle(GetAllowedProvidersQuery request, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        if (standard == null)
        {
            return null;
        }

        var isRestrictedCourse = standard.RestrictedCourseView != null;

        var providers = isRestrictedCourse
            ? await BuildRestrictedCourseProviders(request.LarsCode, cancellationToken)
            : await BuildNotRestrictedCourseProviders(request.LarsCode, standard.CourseType, cancellationToken);

        return new RestrictedCourseDetailsModel
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

    private async Task<List<ProviderModel>> BuildNotRestrictedCourseProviders(string larsCode, CourseType courseType, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken);
        var providerCourses = await _providerCoursesReadRepository.GetProviderCoursesByLarsCode(larsCode);

        var providers = providerCourses
            .Where(pc => !pc.Provider.ProviderCourseTypes.Any(pct =>
                pct.CourseType == courseType &&
                pct.IsRestrictedProvider))
            .Select(pc => new ProviderModel
            {
                Ukprn = pc.Provider.Ukprn,
                ProviderName = pc.Provider.LegalName,
                DateLastStarts = providerAllowedCourses
                    .FirstOrDefault(pac => pac.Ukprn == pc.Provider.Ukprn)?
                    .LastDateStarts
            })
            .ToList();

        var restrictedProviders = BuildRestrictedProviders(providerCourses, providerAllowedCourses, courseType);

        if (restrictedProviders.Count != 0)
        {
            providers.AddRange(restrictedProviders);
        }

        return providers;
    }

    private static List<ProviderModel> BuildRestrictedProviders(IEnumerable<Domain.Entities.ProviderCourse> providerCourses, IEnumerable<ProviderAllowedCourse> providerAllowedCourses, CourseType courseType)
    {
        return providerCourses
            .Where(pc => pc.Provider.ProviderCourseTypes.Any(pct =>
                pct.CourseType == courseType &&
                pct.IsRestrictedProvider))
            .Select(pc => providerAllowedCourses
                .FirstOrDefault(pac => pac.Ukprn == pc.Provider.Ukprn))
            .Where(pac => pac != null)
            .Select(pac => new ProviderModel
            {
                Ukprn = pac.Ukprn,
                ProviderName = pac.Provider.LegalName,
                DateLastStarts = pac.LastDateStarts
            })
            .ToList();
    }
}
