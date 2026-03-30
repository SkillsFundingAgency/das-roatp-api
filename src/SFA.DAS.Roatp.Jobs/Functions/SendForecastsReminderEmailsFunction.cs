using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class SendForecastsReminderEmailsFunction
{
    private readonly ILogger<SendForecastsReminderEmailsFunction> _logger;
    private readonly IProviderCourseTypesReadRepository _providerCourseTypesReadRepository;
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
    private readonly IProviderCourseForecastRepository _providerCourseForecastRepository;
    private readonly IProviderEmailProcessingService _providerEmailProcessingService;
    private readonly ForecastEmailConfiguration _forecastEmailConfiguration;

    public SendForecastsReminderEmailsFunction(
        ILogger<SendForecastsReminderEmailsFunction> logger,
        IProviderCoursesReadRepository providerCoursesReadRepository,
        IProviderCourseForecastRepository providerCourseForecastRepository,
        IProviderEmailProcessingService providerEmailProcessingService,
        IOptions<ForecastEmailConfiguration> forecastEmailConfiguration,
        IProviderCourseTypesReadRepository providerCourseTypesReadRepository)
    {
        _logger = logger;
        _providerCoursesReadRepository = providerCoursesReadRepository;
        _providerCourseForecastRepository = providerCourseForecastRepository;
        _providerEmailProcessingService = providerEmailProcessingService;
        _forecastEmailConfiguration = forecastEmailConfiguration.Value;
        _providerCourseTypesReadRepository = providerCourseTypesReadRepository;
    }

    [Function(nameof(SendForecastsReminderEmailsFunction))]
    public async Task Run([TimerTrigger("%SendForecastsReminderEmailsFunctionSchedule%", RunOnStartup = true)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# Timer trigger function executing at: {ExecutionTime}", DateTime.Now);
        List<int> allowedProviders = await _providerCourseTypesReadRepository.GetAllProvidersWithShortCourses(cancellationToken);
        List<UkprnLarsCodeModel> allShortCoursesCourses = await _providerCoursesReadRepository.GetAllShortCourses(cancellationToken);
        List<ProviderCourseWithLastForecastDate> providerCoursesWithUpToDateForecasts = await _providerCourseForecastRepository.GetProviderCoursesWithRecentForecasts(DateTime.UtcNow.Date.AddDays(ForecastEmailConfiguration.ForecastGraceDays), cancellationToken);

        // build lookup of (Ukprn, LarsCode) for courses that already have up-to-date forecasts
        var upToDateKeys = providerCoursesWithUpToDateForecasts
            .Select(x => (x.Ukprn, x.LarsCode))
            .ToHashSet();

        // filter all provider courses to only those NOT in the lookup and select distinct ukprns
        IEnumerable<int> providersNeedingReminder = allShortCoursesCourses
            .Where(pc => allowedProviders.Contains(pc.Ukprn) && !upToDateKeys.Contains((pc.Ukprn, pc.LarsCode)))
            .Select(c => c.Ukprn)
            .Distinct();

        IEnumerable<ProviderEmailModel> models = providersNeedingReminder.Select(ukprn => ConvertToEmailModel(ukprn));
        if (models.Any()) await _providerEmailProcessingService.SendEmailsInBatches(models);
    }

    private ProviderEmailModel ConvertToEmailModel(int ukprn)
    {
        Dictionary<string, string> tokens = new()
        {
            { ProviderEmailTokens.Ukprn, ukprn.ToString() },
            { ProviderEmailTokens.CourseManagementWebUrl, _forecastEmailConfiguration.CourseManagementWebUrl },
            { ProviderEmailTokens.ProviderAccountWebUrl, _forecastEmailConfiguration.ProviderAccountsWebUrl }
        };

        return new(_forecastEmailConfiguration.ForecastPeriodicalReminderEmailTemplateId, tokens);
    }
}
