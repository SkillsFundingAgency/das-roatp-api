using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetAllProviderDetailsForCourseQueryHandler : IRequestHandler<GetAllProviderDetailsForCourseQuery, GetAllProviderDetailsForCourseQueryResult>
{
    private readonly IProviderDetailsReadRepository _providerDetailsReadRepository;
    private readonly IStandardsReadRepository _standardsReadRepository;
    private readonly INationalAchievementRatesReadRepository _nationalAchievementRatesReadRepository;
    private readonly ILogger<GetAllProviderDetailsForCourseQueryHandler> _logger;

    public GetAllProviderDetailsForCourseQueryHandler(IProviderDetailsReadRepository providerDetailsReadRepository, INationalAchievementRatesReadRepository nationalAchievementRatesReadRepository, IStandardsReadRepository standardsReadRepository, ILogger<GetAllProviderDetailsForCourseQueryHandler> logger)
    {
        _providerDetailsReadRepository = providerDetailsReadRepository;
        _nationalAchievementRatesReadRepository = nationalAchievementRatesReadRepository;
        _standardsReadRepository = standardsReadRepository;
        _logger = logger;
    }

    public async Task<GetAllProviderDetailsForCourseQueryResult> Handle(GetAllProviderDetailsForCourseQuery request, CancellationToken cancellationToken)
    {

        //MFCMFC You need to rewrite this to look like the SQL queries in CourseDelivery
        // SEE await _providerService.GetProvidersByStandardId(
        // and await _providerService.GetProvidersByStandardAndLocation
        var providerDetails = await _providerDetailsReadRepository.GetAllProviderDetailsWithDistance( request.LarsCode, request.Latitude,
            request.Longitude);

        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        var level = (ApprenticeshipLevel)standard.Level;

        var nationalAchievementRates = await _nationalAchievementRatesReadRepository
            .GetAll();

        //provider not gathered.....
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
            var rates= filteredNationalAchievementRates.Where(r => r.ProviderId == provider.ProviderId).ToList();

            _logger.LogInformation("Providers {ukprn} has {count} rates",provider.Ukprn,rates.Count);
            foreach (var rate in rates)
            {
                result.AchievementRates.Add(rate);
            }
            
            var locations = providerLocations.Where(p => p.ProviderId == provider.ProviderId).ToList();
            _logger.LogInformation("Providers {ukprn} has {count} locations", provider.Ukprn, locations.Count);
            
            foreach (var location in locations)
            {
                result.LocationDetails.Add(location);
            }

            providers.Add(result);

        }
        return request.QuerySortOrder == 1 || request.Latitude==null || request.Longitude==null
            ? new GetAllProviderDetailsForCourseQueryResult { Providers = providers.OrderBy(p => p.Name).ToList() }
            : new GetAllProviderDetailsForCourseQueryResult { Providers = providers.OrderBy(p => p.ShortestLocationDistanceInMiles).ThenBy(p=>p.Name).ToList() };
    }
}