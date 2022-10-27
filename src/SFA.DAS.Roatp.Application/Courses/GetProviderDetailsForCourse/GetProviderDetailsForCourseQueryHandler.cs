using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQueryHandler : IRequestHandler<GetProviderDetailsForCourseQuery, GetProviderDetailsForCourseQueryResult>
{

    private readonly IProviderDetailsReadRepository _providerRetailsReadRepository;
    private readonly INationalAchievementRatesReadRepository _nationalAchievementRatesReadRepository;


    public GetProviderDetailsForCourseQueryHandler(IProviderDetailsReadRepository providerRetailsReadRepository, INationalAchievementRatesReadRepository nationalAchievementRatesReadRepository)
    {
        _providerRetailsReadRepository = providerRetailsReadRepository;
        _nationalAchievementRatesReadRepository = nationalAchievementRatesReadRepository;
    }
    // private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;


    public async Task<GetProviderDetailsForCourseQueryResult> Handle(GetProviderDetailsForCourseQuery request, CancellationToken cancellationToken)
    {


        var providerDetails = await _providerRetailsReadRepository.GetProviderDetailsWithDistance(request.Ukprn, request.Lat,
            request.Lon);

        // getting providerDetails
        var result = (GetProviderDetailsForCourseQueryResult)providerDetails;

        // get NAR details
        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByUkprn(request.Ukprn);
        var achievementRates = nationalAchievementRates.Select(nar => (NationalAchievementRateModel)nar).ToList(); ;
       
        // note the original SQL filters it down to Age == 4??? and the level entered + level==1

        if (string.IsNullOrEmpty(request.SectorSubjectArea))
            result.AchievementRates = achievementRates;
        else
        {
            result.AchievementRates = achievementRates
                .Where(x => x.SectorSubjectArea.Equals( request.SectorSubjectArea,StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        // GET provider location details
        return result;
    }
}