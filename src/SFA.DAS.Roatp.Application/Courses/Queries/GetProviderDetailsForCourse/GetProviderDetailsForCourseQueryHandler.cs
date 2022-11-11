using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQueryHandler : IRequestHandler<GetProviderDetailsForCourseQuery, GetProviderDetailsForCourseQueryResult>
{
    private readonly IProviderDetailsReadRepository _providerDetailsReadRepository;
    private readonly INationalAchievementRatesReadRepository _nationalAchievementRatesReadRepository;
    private readonly IProcessProviderCourseLocationsService _processProviderCourseLocationsService;

    public GetProviderDetailsForCourseQueryHandler(IProviderDetailsReadRepository providerDetailsReadRepository, INationalAchievementRatesReadRepository nationalAchievementRatesReadRepository, IProcessProviderCourseLocationsService processProviderCourseLocationsService)
    {
        _providerDetailsReadRepository = providerDetailsReadRepository;
        _nationalAchievementRatesReadRepository = nationalAchievementRatesReadRepository;
        _processProviderCourseLocationsService = processProviderCourseLocationsService;
    }

    public async Task<GetProviderDetailsForCourseQueryResult> Handle(GetProviderDetailsForCourseQuery request, CancellationToken cancellationToken)
    {

        var providerDetails = await _providerDetailsReadRepository.GetProviderDetailsWithDistance(request.Ukprn, request.LarsCode,request.Latitude,
            request.Longitude);
        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByUkprn(request.Ukprn);
        var providerLocations = await _providerDetailsReadRepository.GetProviderlocationDetailsWithDistance(
            request.Ukprn, request.LarsCode, request.Latitude,
            request.Longitude);

        var result = (GetProviderDetailsForCourseQueryResult)providerDetails;

        var deliveryModels = _processProviderCourseLocationsService.ConvertProviderLocationsToDeliveryModels(providerLocations);

        result.DeliveryModels = deliveryModels;

        if (nationalAchievementRates!=null)
            result.AchievementRates = nationalAchievementRates.Select(nar => (NationalAchievementRateModel)nar).ToList();

        return result;
    }
}