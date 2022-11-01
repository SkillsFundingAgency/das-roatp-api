﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQueryHandler : IRequestHandler<GetProviderDetailsForCourseQuery, GetProviderDetailsForCourseQueryResult>
{
    private readonly IProviderDetailsReadRepository _providerDetailsReadRepository;
    private readonly INationalAchievementRatesReadRepository _nationalAchievementRatesReadRepository;

    public GetProviderDetailsForCourseQueryHandler(IProviderDetailsReadRepository providerDetailsReadRepository, INationalAchievementRatesReadRepository nationalAchievementRatesReadRepository)
    {
        _providerDetailsReadRepository = providerDetailsReadRepository;
        _nationalAchievementRatesReadRepository = nationalAchievementRatesReadRepository;
    }

    public async Task<GetProviderDetailsForCourseQueryResult> Handle(GetProviderDetailsForCourseQuery request, CancellationToken cancellationToken)
    {
        var providerDetails = await _providerDetailsReadRepository.GetProviderDetailsWithDistance(request.Ukprn, request.LarsCode,request.Lat,
            request.Lon);

        if (providerDetails == null)
            return null;

        var result = (GetProviderDetailsForCourseQueryResult)providerDetails;

        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByUkprn(request.Ukprn);
        result.AchievementRates = nationalAchievementRates.Select(nar => (NationalAchievementRateModel)nar).ToList(); 
        
        var providerLocations = await _providerDetailsReadRepository.GetProviderlocationDetailsWithDistance(
            request.Ukprn, request.LarsCode, request.Lat,
            request.Lon);
        result.LocationDetails = providerLocations.Select(pl => (CourseLocationModel)pl).ToList();
        return result;
    }
}