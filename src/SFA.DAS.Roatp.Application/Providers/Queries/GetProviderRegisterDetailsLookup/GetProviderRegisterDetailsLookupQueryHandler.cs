using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderRegisterDetailsLookup;

public class GetProviderRegisterDetailsLookupQueryHandler : IRequestHandler<GetProviderRegisterDetailsLookupQuery, ValidatedResponse<GetProviderRegisterDetailsLookupQueryResult>>
{
    private readonly IProviderRegistrationDetailsReadRepository _providersRegistrationDetailReadRepository;
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
    private readonly IProviderCourseTypesReadRepository _providerCourseTypesReadRepository;

    public GetProviderRegisterDetailsLookupQueryHandler(IProviderRegistrationDetailsReadRepository providersRegistrationDetailReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository, IProviderCourseTypesReadRepository providerCourseTypesReadRepository)
    {
        _providersRegistrationDetailReadRepository = providersRegistrationDetailReadRepository;
        _providerCoursesReadRepository = providerCoursesReadRepository;
        _providerCourseTypesReadRepository = providerCourseTypesReadRepository;
    }

    public async Task<ValidatedResponse<GetProviderRegisterDetailsLookupQueryResult>> Handle(GetProviderRegisterDetailsLookupQuery request, CancellationToken cancellationToken)
    {
        var registration = await _providersRegistrationDetailReadRepository.GetProviderRegistrationDetail(request.Ukprn, cancellationToken);
        if (registration is null)
        {
            return new ValidatedResponse<GetProviderRegisterDetailsLookupQueryResult>((GetProviderRegisterDetailsLookupQueryResult)null);
        }

        var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(request.Ukprn);
        var providerCourseTypes = await _providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(request.Ukprn);

        var allowedTypes = providerCourseTypes
            .Select(pct => pct.CourseType)
            .ToHashSet();

        var result = new GetProviderRegisterDetailsLookupQueryResult
        {
            Ukprn = registration.Ukprn,
            Status = (OrganisationStatus)registration.StatusId,
            Type = (ProviderType)registration.ProviderTypeId,
            CourseTypes = new()
        };

        var courseTypesFromCourses = providerCourses
            .Where(pc => pc?.Standard != null && allowedTypes.Contains(pc.Standard.CourseType))
            .GroupBy(pc => pc.Standard.CourseType)
            .Select(g => new CourseTypeModel
            {
                CourseType = g.Key,
                Courses = g.Select(pc => new CourseModel
                {
                    LarsCode = pc.LarsCode,
                    EffectiveFrom = pc.CreatedDate is DateTime dt ? DateOnly.FromDateTime(dt) : null,
                    EffectiveTo = null
                }).ToList()
            })
            .ToList();

        foreach (var allowed in allowedTypes)
        {
            if (!courseTypesFromCourses.Exists(ct => ct.CourseType == allowed))
            {
                courseTypesFromCourses.Add(new CourseTypeModel
                {
                    CourseType = allowed,
                    Courses = new()
                });
            }
        }

        result.CourseTypes = courseTypesFromCourses;

        return new ValidatedResponse<GetProviderRegisterDetailsLookupQueryResult>(result);
    }
}
