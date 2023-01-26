using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQueryHandler : IRequestHandler<GetProvidersForCourseQuery, ValidatedResponse<GetProvidersForCourseQueryResult>>
{
    private readonly IProviderDetailsReadRepository _providerDetailsReadRepository;
    private readonly IStandardsReadRepository _standardsReadRepository;
    private readonly INationalAchievementRatesReadRepository _nationalAchievementRatesReadRepository;
    private readonly IProcessProviderCourseLocationsService _processProviderCourseLocationsService;
    private readonly ILogger<GetProvidersForCourseQueryHandler> _logger;

    public GetProvidersForCourseQueryHandler(IProviderDetailsReadRepository providerDetailsReadRepository, INationalAchievementRatesReadRepository nationalAchievementRatesReadRepository, IStandardsReadRepository standardsReadRepository, IProcessProviderCourseLocationsService processProviderCourseLocationsService, ILogger<GetProvidersForCourseQueryHandler> logger)
    {
        _providerDetailsReadRepository = providerDetailsReadRepository;
        _nationalAchievementRatesReadRepository = nationalAchievementRatesReadRepository;
        _standardsReadRepository = standardsReadRepository;
        _processProviderCourseLocationsService = processProviderCourseLocationsService;
        _logger = logger;
    }

    public async Task<ValidatedResponse<GetProvidersForCourseQueryResult>> Handle(GetProvidersForCourseQuery request, CancellationToken cancellationToken)
    {
        var providerDetails = 
            await _providerDetailsReadRepository.GetProvidersForLarsCodeWithDistance( request.LarsCode, request.Latitude, request.Longitude);
        
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        ApprenticeshipLevel apprenticeshipLevel;
        if (standard.Level >= (int)ApprenticeshipLevel.FourPlus)
            apprenticeshipLevel = ApprenticeshipLevel.FourPlus;
        else
            apprenticeshipLevel = (ApprenticeshipLevel)standard.Level;

        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByProvidersLevelsSectorSubjectArea(
            providerDetails.Select(p=>p.ProviderId).ToList(), 
            new List<ApprenticeshipLevel>{ApprenticeshipLevel.AllLevels, apprenticeshipLevel},
            standard.SectorSubjectArea);

        var providerLocations =
            await _providerDetailsReadRepository.GetAllProviderlocationDetailsWithDistance(request.LarsCode,
                request.Latitude, request.Longitude);

        var providers = new List<ProviderSummation>();
        _logger.LogInformation("Providers to process: {count}", providers.Count);
        foreach (var provider in providerDetails)
        {
            _logger.LogInformation("Provider: {ukprn}", provider.Ukprn);

            var result = (ProviderSummation)provider;
            var rate= nationalAchievementRates.Where(r => r.ProviderId == provider.ProviderId).MaxBy(a=>a.ApprenticeshipLevel);

            _logger.LogInformation("Provider {ukprn} has apprenticeship level: {apprenticeshipLevel}",provider.Ukprn,rate?.ApprenticeshipLevel);
            
            if (rate!=null)
                result.AchievementRates.Add(rate);

            result.DeliveryModels = _processProviderCourseLocationsService.ConvertProviderLocationsToDeliveryModels(providerLocations.Where(p=>p.ProviderId==provider.ProviderId).ToList());
            
            providers.Add(result);
        }

        return new ValidatedResponse<GetProvidersForCourseQueryResult>(
            new GetProvidersForCourseQueryResult
        {
            LarsCode = standard.LarsCode, Level = standard.Level, CourseTitle = standard.Title,
            Providers = providers.OrderBy(x=>x.Ukprn).ToList()
        });
    }
}