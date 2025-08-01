using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;

namespace SFA.DAS.Roatp.Jobs.Services;

public class ImportAnnualFeedbackSummariesService(
    ILogger<ImportAnnualFeedbackSummariesService> _logger,
    IDateTimeProvider _dateTimeProvider,
    IProviderEmployerStarsReadRepository _providerEmployerStarsReadRepository,
    ICourseManagementOuterApiClient _courseManagementOuterApiClient)
    : IImportAnnualFeedbackSummariesService
{
    // The function is meant to run only once a year in the month of August after end of the academic year. Hence there is an assumption here that the time period is always the last academic year.
    public string GetTimePeriod()
    {
        var currentYear = int.Parse(_dateTimeProvider.UtcNow.ToString("yy"));
        return $"AY{currentYear - 1}{currentYear}";
    }

    public async Task<bool> CheckIfDataExists(string timePeriod)
    {
        _logger.LogInformation("Checking if data exists for annual feedback summaries...");
        var timePeriods = await _providerEmployerStarsReadRepository.GetTimePeriods();
        var dataExists = timePeriods.Contains(timePeriod);
        if (dataExists)
        {
            _logger.LogInformation("Data exists for annual feedback summaries.");
        }
        else
        {
            _logger.LogInformation("No data found for annual feedback summaries.");
        }
        return dataExists;
    }

    public async Task<(IEnumerable<ProviderApprenticeStars>, IEnumerable<ProviderEmployerStars>)> GetFeedbackSummaries(string timePeriod)
    {
        var (success, result) = await _courseManagementOuterApiClient.Get<AnnualSummariesFeedbackResponse>($"feedback/lookup/annual-summarised?academicYear={timePeriod}");
        if (!success)
        {
            throw new InvalidOperationException($"Failed to retrieve annual feedback summaries for time period {timePeriod}");
        }
        return (MapToApprenticeStars(result.ApprenticesFeedback, timePeriod),
                MapToEmployerStars(result.EmployersFeedback, timePeriod));
    }

    private static IEnumerable<ProviderApprenticeStars> MapToApprenticeStars(IEnumerable<FeedbackStarsSummary> apprenticesFeedback, string timePeriod)
    {
        return apprenticesFeedback.Select(f => new ProviderApprenticeStars
        {
            Ukprn = f.Ukprn,
            Stars = f.Stars,
            ReviewCount = f.ReviewCount,
            TimePeriod = timePeriod
        });
    }

    private static IEnumerable<ProviderEmployerStars> MapToEmployerStars(IEnumerable<FeedbackStarsSummary> employersFeedback, string timePeriod)
    {
        return employersFeedback.Select(f => new ProviderEmployerStars
        {
            Ukprn = f.Ukprn,
            Stars = f.Stars,
            ReviewCount = f.ReviewCount,
            TimePeriod = timePeriod
        });
    }
}
