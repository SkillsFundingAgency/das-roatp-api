using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQueryHandler : IRequestHandler<GetProvidersForCourseQuery, GetProvidersForCourseQueryResult>
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

    public async Task<GetProvidersForCourseQueryResult> Handle(GetProvidersForCourseQuery request, CancellationToken cancellationToken)
    {
        var providerDetails = await _providerDetailsReadRepository.GetAllProviderDetailsWithDistance( request.LarsCode, request.Latitude,
            request.Longitude);

        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        var level = (ApprenticeshipLevel)standard.Level;

        var nationalAchievementRates = await _nationalAchievementRatesReadRepository
            .GetAll();

        var filteredNationalAchievementRates =
            nationalAchievementRates.Where(x => (x.ApprenticeshipLevel == ApprenticeshipLevel.AllLevels || x.ApprenticeshipLevel == level) 
                                                && x.Age==Age.AllAges && x.SectorSubjectArea==standard.SectorSubjectArea 
                                                && providerDetails.Select(p=>p.ProviderId).Contains(x.ProviderId)).ToList();

        var providerLocations =
            await _providerDetailsReadRepository.GetAllProviderlocationDetailsWithDistance(request.LarsCode,
                request.Latitude, request.Longitude);

        var providers = new List<ProviderDetails>();
        _logger.LogInformation("Providers to process: {count}", providers.Count);
        foreach (var provider in providerDetails)
        {
            _logger.LogInformation("Provider: {ukprn}", provider.Ukprn);

            var result = (ProviderDetails)provider;
            var rate= filteredNationalAchievementRates.Where(r => r.ProviderId == provider.ProviderId).MaxBy(a=>a.ApprenticeshipLevel);

            _logger.LogInformation("Providers {ukprn} has rate {apprenticeshipLevel}",provider.Ukprn,rate?.ApprenticeshipLevel);
            
            if (rate!=null)
                result.AchievementRates.Add(rate);

            result.DeliveryModels = _processProviderCourseLocationsService.ConvertProviderLocationsToDeliveryModels(providerLocations.Where(p=>p.ProviderId==provider.ProviderId).ToList());
            
            providers.Add(result);
        }

        return new GetProvidersForCourseQueryResult
        {
            LarsCode = standard.LarsCode, Level = standard.Level, CourseTitle = standard.Title,
            Providers = providers.OrderBy(x=>x.Ukprn).ToList()
        };
    }
}