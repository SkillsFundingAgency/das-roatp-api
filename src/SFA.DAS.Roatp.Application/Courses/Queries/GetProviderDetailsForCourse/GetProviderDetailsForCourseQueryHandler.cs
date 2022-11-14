using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
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
        var level = (ApprenticeshipLevel)standard.Level;
        var providerDetails = await _providerDetailsReadRepository.GetProviderDetailsWithDistance(request.Ukprn, request.LarsCode, request.Latitude,
            request.Longitude);
        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByUkprn(request.Ukprn);

        var filteredNationalAchievementRates =
            nationalAchievementRates?.Where(x => (x.ApprenticeshipLevel == ApprenticeshipLevel.AllLevels || x.ApprenticeshipLevel == level)
                                                && x.Age == Age.AllAges && x.SectorSubjectArea == standard.SectorSubjectArea).ToList();
        var rate = filteredNationalAchievementRates?.MaxBy(a => a.ApprenticeshipLevel);

        var providerLocations = await _providerDetailsReadRepository.GetProviderlocationDetailsWithDistance(
            request.Ukprn, request.LarsCode, request.Latitude,
            request.Longitude);

        var result = (GetProviderDetailsForCourseQueryResult)providerDetails;

        var deliveryModels = _processProviderCourseLocationsService.ConvertProviderLocationsToDeliveryModels(providerLocations);

        result.DeliveryModels = deliveryModels;

        _logger.LogInformation("Provider {ukprn} has rate {apprenticeshipLevel}", request.Ukprn, rate?.ApprenticeshipLevel);

        if (rate != null)
            result.AchievementRates.Add(rate);

        return result;
    }
}