using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Courses.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQueryHandler : IRequestHandler<GetProviderDetailsForCourseQuery, GetProviderDetailsForCourseQueryResult>
{

    private readonly IProviderDetailsReadRepository _providerDetailsReadRepository;
    private readonly INationalAchievementRatesReadRepository _nationalAchievementRatesReadRepository;


    public GetProviderDetailsForCourseQueryHandler(IProviderDetailsReadRepository providerDetailsReadRepository, INationalAchievementRatesReadRepository nationalAchievementRatesReadRepository)
    {
        _providerDetailsReadRepository = providerDetailsReadRepository;
        _nationalAchievementRatesReadRepository = nationalAchievementRatesReadRepository;
    }
    // private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;


    public async Task<GetProviderDetailsForCourseQueryResult> Handle(GetProviderDetailsForCourseQuery request, CancellationToken cancellationToken)
    {

        // Getting provider and course details
        var providerDetails = await _providerDetailsReadRepository.GetProviderDetailsWithDistance(request.Ukprn, request.LarsCode,request.Lat,
            request.Lon);

        if (providerDetails == null)
            return null;

        var result = (GetProviderDetailsForCourseQueryResult)providerDetails;

        // Getting national achievement rates
        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByUkprn(request.Ukprn);
        result.AchievementRates = nationalAchievementRates.Select(nar => (NationalAchievementRateModel)nar).ToList(); ;
        

        // Getting provider location and delivery details
        var providerLocations = await _providerDetailsReadRepository.GetProviderlocationDetailsWithDistance(
            request.Ukprn, request.LarsCode, request.Lat,
            request.Lon);
        result.LocationAndDeliveryDetails = providerLocations.Select(pl => (LocationAndDeliveryDetail)pl).ToList();
        return result;
    }
}