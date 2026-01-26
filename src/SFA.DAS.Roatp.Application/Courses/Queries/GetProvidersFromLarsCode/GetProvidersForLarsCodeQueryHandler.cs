using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;

public class GetProvidersForLarsCodeQueryHandler : IRequestHandler<GetProvidersForLarsCodeQuery, ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>>
{
    private readonly IProvidersReadRepository _providersReadRepository;
    private readonly ILogger<GetProvidersForLarsCodeQueryHandler> _logger;
    public GetProvidersForLarsCodeQueryHandler(IProvidersReadRepository providersReadRepository, ILogger<GetProvidersForLarsCodeQueryHandler> logger)
    {
        _providersReadRepository = providersReadRepository;
        _logger = logger;
    }

    public async Task<ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>> Handle(GetProvidersForLarsCodeQuery request, CancellationToken cancellationToken)
    {
        var isWorkplace = (bool?)null;
        var isProvider = (bool?)null;
        var isBlockRelease = (bool?)null;
        var isDayRelease = (bool?)null;
        var isOnline = (bool?)null;

        if (request.DeliveryModes.Contains(DeliveryModeV2.Workplace)) isWorkplace = true;
        if (request.DeliveryModes.Contains(DeliveryModeV2.Provider)) isProvider = true;
        if (request.DeliveryModes.Contains(DeliveryModeV2.BlockRelease)) isBlockRelease = true;
        if (request.DeliveryModes.Contains(DeliveryModeV2.DayRelease)) isDayRelease = true;
        if (request.DeliveryModes.Contains(DeliveryModeV2.Online)) isOnline = true;

        string qar = null;

        if (request.Qar.Count > 0)
        {
            qar = string.Join(',', request.Qar);
        }

        string employerProviderRatings = null;
        if (request.EmployerProviderRatings.Count > 0)
        {
            employerProviderRatings = string.Join(',', request.EmployerProviderRatings);
        }

        string apprenticeProviderRatings = null;
        if (request.ApprenticeProviderRatings.Count > 0)
        {
            apprenticeProviderRatings = string.Join(',', request.ApprenticeProviderRatings);
        }

        _logger.LogInformation("calling provider details for larsCode {larscode}", request.LarsCode);

        var parameters = new GetProvidersFromLarsCodeOptionalParameters
        {
            Page = request.Page,
            PageSize = request.PageSize,
            IsWorkplace = isWorkplace,
            IsProvider = isProvider,
            IsBlockRelease = isBlockRelease,
            IsDayRelease = isDayRelease,
            IsOnline = isOnline,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Location = request.Location,
            Distance = request.Distance,
            QarRange = qar,
            EmployerProviderRatings = employerProviderRatings,
            ApprenticeProviderRatings = apprenticeProviderRatings,
            UserId = request.UserId
        };

        var results = await _providersReadRepository.GetProvidersByLarsCode(request.LarsCode,
            (ProviderOrderBy)request.OrderBy!, parameters, cancellationToken);

        var first = results[0];

        var result = new GetProvidersForLarsCodeQueryResultV2
        {
            Page = first.Page,
            PageSize = first.PageSize,
            TotalCount = first.TotalCount,
            TotalPages = first.TotalPages,
            LarsCode = first.LarsCode,
            StandardName = first.StandardName,
            QarPeriod = first.QarPeriod,
            ReviewPeriod = first.ReviewPeriod,
            Providers = new List<ProviderDataV2>()
        };

        if (first.Ukprn == 0) return new ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>(result);

        foreach (var item in results)
        {
            var provider = new ProviderDataV2
            {
                Ordering = item.Ordering,
                Ukprn = item.Ukprn,
                ProviderName = item.ProviderName,
                HasOnlineDeliveryOption = item.HasOnlineDeliveryOption,
                ShortlistId = item.ShortlistId,
                Leavers = item.Leavers,
                AchievementRate = item.AchievementRate,
                EmployerReviews = item.EmployerReviews,
                EmployerStars = item.EmployerStars,
                EmployerRating = item.EmployerRating,
                ApprenticeReviews = item.ApprenticeReviews,
                ApprenticeStars = item.ApprenticeStars,
                ApprenticeRating = item.ApprenticeRating,
                Locations = new List<ProviderLocationModel>()
            };

            var numberOfLocations = item.LocationsCount;

            var locations = new List<ProviderLocationModel>();
            List<LocationType> locationTypes = item.LocationTypes.Split(',').Select(x => (LocationType)int.Parse(x.Trim())).ToList();
            List<decimal> courseDistances = item.CourseDistances.Split(',').Select(x => decimal.Parse(x.Trim())).ToList();
            List<bool> atEmployers = item.AtEmployers.Split(',').Select(x => int.Parse(x.Trim()) != 0).ToList();
            List<bool> dayReleases = item.DayReleases.Split(',').Select(x => int.Parse(x.Trim()) != 0).ToList();
            List<bool> blockReleases = item.BlockReleases.Split(',').Select(x => int.Parse(x.Trim()) != 0).ToList();

            for (var i = 0; i < numberOfLocations; i++)
            {
                var location = new ProviderLocationModel
                {
                    Ordering = i + 1,
                    LocationType = locationTypes[i],
                    CourseDistance = courseDistances[i],
                    AtEmployer = atEmployers[i],
                    DayRelease = dayReleases[i],
                    BlockRelease = blockReleases[i]
                };
                locations.Add(location);
            }

            provider.Locations = locations;
            result.Providers.Add(provider);
        }

        return new ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>(result);
    }
}