using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQueryHandler : IRequestHandler<GetProviderDetailsForCourseQuery, GetProviderDetailsForCourseQueryResult>
{
    private readonly IProviderDetailsReadRepository _providerDetailsReadRepository;
    private readonly INationalAchievementRatesReadRepository _nationalAchievementRatesReadRepository;
    private readonly IProcessProviderCourseLocationsService _processProviderCourseLocationsService;
    private readonly IStandardsReadRepository _standardsReadRepository;
    private readonly ILogger<GetProviderDetailsForCourseQueryHandler> _logger;
    public GetProviderDetailsForCourseQueryHandler(IProviderDetailsReadRepository providerDetailsReadRepository, INationalAchievementRatesReadRepository nationalAchievementRatesReadRepository, IProcessProviderCourseLocationsService processProviderCourseLocationsService, IStandardsReadRepository standardsReadRepository, ILogger<GetProviderDetailsForCourseQueryHandler> logger)
    {
        _providerDetailsReadRepository = providerDetailsReadRepository;
        _nationalAchievementRatesReadRepository = nationalAchievementRatesReadRepository;
        _processProviderCourseLocationsService = processProviderCourseLocationsService;
        _standardsReadRepository = standardsReadRepository;
        _logger = logger;
    }

    public async Task<GetProviderDetailsForCourseQueryResult> Handle(GetProviderDetailsForCourseQuery request, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);
        ApprenticeshipLevel level;
        if (standard.Level >= (int)ApprenticeshipLevel.FourPlus)
            level = ApprenticeshipLevel.FourPlus;
        else
            level = (ApprenticeshipLevel)standard.Level;

        var providerDetails = await _providerDetailsReadRepository.GetProviderForUkprnAndLarsCodeWithDistance(request.Ukprn, request.LarsCode, request.Latitude,
            request.Longitude);
        if (providerDetails == null)
            return null;
        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByUkprn(request.Ukprn);

        var filteredNationalAchievementRates =
            nationalAchievementRates.Where(x => (x.ApprenticeshipLevel == ApprenticeshipLevel.AllLevels || x.ApprenticeshipLevel == level)
                                                && x.Age == Age.AllAges && x.SectorSubjectArea == standard.SectorSubjectArea);
        var rate = filteredNationalAchievementRates?.MaxBy(a => a.ApprenticeshipLevel);

        _logger.LogInformation("Provider {ukprn} has apprenticeship level {apprenticeshipLevel}", request.Ukprn, rate?.ApprenticeshipLevel);

        var providerLocations = await _providerDetailsReadRepository.GetProviderLocationDetailsWithDistance(
            request.Ukprn, request.LarsCode, request.Latitude,
            request.Longitude);

        var result = (GetProviderDetailsForCourseQueryResult)providerDetails;
       

        var deliveryModels = _processProviderCourseLocationsService.ConvertProviderLocationsToDeliveryModels(providerLocations);

        result.DeliveryModels = deliveryModels;

        if (rate != null)
        {
            result.AchievementRates.Add(rate);
        }

        return result;
    }
}