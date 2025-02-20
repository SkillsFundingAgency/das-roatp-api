using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.AcademicYears.Queries.GetLatest;
public class GetAcademicYearsLatestQueryHandler : IRequestHandler<GetAcademicYearsLatestQuery, GetAcademicYearsLatestQueryResult>
{
    private readonly INationalQarReadRepository _nationalQarReadRepository;
    private readonly IProviderEmployerStarsReadRepository _providerEmployerStarsReadRepository;

    public GetAcademicYearsLatestQueryHandler(INationalQarReadRepository nationalQarReadRepository, IProviderEmployerStarsReadRepository providerEmployerStarsReadRepository)
    {
        _nationalQarReadRepository = nationalQarReadRepository;
        _providerEmployerStarsReadRepository = providerEmployerStarsReadRepository;
    }

    public async Task<GetAcademicYearsLatestQueryResult> Handle(GetAcademicYearsLatestQuery request, CancellationToken cancellationToken)
    {
        var nationalQars = await _nationalQarReadRepository.GetAll();

        var maxNationalQarTimePeriod = nationalQars.Max(x => x.TimePeriod);

        var providerEmployerStarsTimePeriods = await _providerEmployerStarsReadRepository.GetTimePeriods();

        string reviewPeriodCalculated = string.Concat(DateTime.UtcNow.AddMonths(-19).Year.ToString().AsSpan(2, 2),
                                                        DateTime.UtcNow.AddMonths(-7).Year.ToString().AsSpan(2, 2));

        var feedbackPeriods = providerEmployerStarsTimePeriods.Where(x => x.StartsWith("AY") && int.Parse(x.Replace("AY", string.Empty)) <= int.Parse(reviewPeriodCalculated)).ToList();

        var reviewPeriod = reviewPeriodCalculated;

        if (feedbackPeriods.Count > 0)
        {
            reviewPeriod = feedbackPeriods.MaxBy(x => x)!.Replace("AY", string.Empty);
        }

        var result = new GetAcademicYearsLatestQueryResult
        {
            QarPeriod = maxNationalQarTimePeriod,
            ReviewPeriod = reviewPeriod
        };
        return result;
    }
}
