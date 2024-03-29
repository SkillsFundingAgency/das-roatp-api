﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQueryHandler : IRequestHandler<GetProviderDetailsForCourseQuery, ValidatedResponse<GetProviderDetailsForCourseQueryResult>>
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

    public async Task<ValidatedResponse<GetProviderDetailsForCourseQueryResult>> Handle(GetProviderDetailsForCourseQuery request, CancellationToken cancellationToken)
    {
        Standard standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        var providerDetails = await _providerDetailsReadRepository.GetProviderForUkprnAndLarsCodeWithDistance(request.Ukprn, request.LarsCode, request.Latitude, request.Longitude);

        if (providerDetails == null)
            return new ValidatedResponse<GetProviderDetailsForCourseQueryResult>((GetProviderDetailsForCourseQueryResult)null);

        var providerLocations = await _providerDetailsReadRepository.GetProviderLocationDetailsWithDistance(request.Ukprn, request.LarsCode, request.Latitude, request.Longitude);

        GetProviderDetailsForCourseQueryResult result = providerDetails;

        result.DeliveryModels = _processProviderCourseLocationsService.ConvertProviderLocationsToDeliveryModels(providerLocations);

        await FindAndAddProviderRatingsForCourse(result, request.Ukprn, standard.SectorSubjectAreaTier1, standard.Level);

        return new ValidatedResponse<GetProviderDetailsForCourseQueryResult>(result);
    }

    private async Task FindAndAddProviderRatingsForCourse(GetProviderDetailsForCourseQueryResult result, int ukprn, int sectorSubjectAreaTier1, int standardLevel)
    {
        var nationalAchievementRates = await _nationalAchievementRatesReadRepository.GetByUkprn(ukprn);

        var apprenticeshipLevel = standardLevel >= (int)ApprenticeshipLevel.FourPlus ? ApprenticeshipLevel.FourPlus : (ApprenticeshipLevel)standardLevel;

        var filteredNationalAchievementRates = nationalAchievementRates.Where(
                x => (x.ApprenticeshipLevel == ApprenticeshipLevel.AllLevels || x.ApprenticeshipLevel == apprenticeshipLevel)
                    && x.Age == Age.AllAges
                    && x.SectorSubjectAreaTier1 == sectorSubjectAreaTier1);

        var rate = filteredNationalAchievementRates.MaxBy(a => a.ApprenticeshipLevel);

        if (rate != null) result.AchievementRates.Add(rate);
    }
}