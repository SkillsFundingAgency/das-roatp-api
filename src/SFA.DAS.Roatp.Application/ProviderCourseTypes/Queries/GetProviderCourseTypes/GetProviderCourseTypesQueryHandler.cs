using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;


public class GetProviderCourseTypesQueryHandler : IRequestHandler<GetProviderCourseTypesQuery, ValidatedResponse<List<ProviderCourseTypeModel>>>
{
    private readonly IProviderCourseTypesRepository _providerCourseTypesReadRepository;
    private readonly IProviderAllowedCoursesRepository _providerAllowedCoursesRepository;
    private readonly IRestrictedCourseViewRepository _restrictedCourseViewRepository;

    public GetProviderCourseTypesQueryHandler(IProviderCourseTypesRepository providerCourseTypesReadRepository, IProviderAllowedCoursesRepository providerAllowedCoursesRepository, IRestrictedCourseViewRepository restrictedCourseViewRepository)
    {
        _providerCourseTypesReadRepository = providerCourseTypesReadRepository;
        _providerAllowedCoursesRepository = providerAllowedCoursesRepository;
        _restrictedCourseViewRepository = restrictedCourseViewRepository;
    }

    public async Task<ValidatedResponse<List<ProviderCourseTypeModel>>> Handle(GetProviderCourseTypesQuery request, CancellationToken cancellationToken)
    {
        var providerCourseTypes = await _providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellationToken);

        var result = new List<ProviderCourseTypeModel>();

        foreach (var providerCourseType in providerCourseTypes)
        {
            var courseType = new ProviderCourseTypeModel();

            if (providerCourseType.CourseType == CourseType.Apprenticeship)
            {
                courseType = await GetApprenticeshipProviderCourseType(providerCourseType, request.Ukprn, providerCourseType.CourseType, cancellationToken);
            }
            if (providerCourseType.CourseType == CourseType.ShortCourse)
            {
                courseType = await GetShortCourseProviderCourseType(providerCourseType, request.Ukprn, providerCourseType.CourseType, cancellationToken);
            }

            result.Add(courseType);
        }

        return new ValidatedResponse<List<ProviderCourseTypeModel>>(result);
    }

    private async Task<ProviderCourseTypeModel> GetApprenticeshipProviderCourseType(ProviderCourseType providerCourseType, int ukprn, CourseType courseType, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(ukprn, courseType, cancellationToken);

        var model = (ProviderCourseTypeModel)providerCourseType;

        if (providerCourseType.IsRestrictedProvider)
        {
            model.AllowedCount = providerAllowedCourses.Count(pac =>
                pac.LastDateStarts == null);

            model.RestrictedCount = null;
        }
        else
        {
            var restrictedCourses = await _restrictedCourseViewRepository.GetRestrictedCourses(cancellationToken);

            var restrictedCoursesCount = restrictedCourses.Count(rc => rc.Standard.CourseType == courseType);

            restrictedCoursesCount += providerAllowedCourses.Count(pac =>
                pac.LastDateStarts != null &&
                !restrictedCourses.Any(rc =>
                    rc.LarsCode == pac.LarsCode));

            restrictedCoursesCount -= providerAllowedCourses.Count(pac =>
                pac.LastDateStarts == null &&
                restrictedCourses.Any(rc =>
                    rc.LarsCode == pac.LarsCode));

            model.AllowedCount = null;
            model.RestrictedCount = restrictedCoursesCount;
        }

        return model;
    }

    private async Task<ProviderCourseTypeModel> GetShortCourseProviderCourseType(ProviderCourseType providerCourseType, int ukprn, CourseType courseType, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(ukprn, courseType, cancellationToken);

        var model = (ProviderCourseTypeModel)providerCourseType;

        model.AllowedCount = providerAllowedCourses.Count;

        model.RestrictedCount = null;

        return model;
    }
}