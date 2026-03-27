using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class SendForecastsReminderEmailsFunction
{
    private readonly ILogger<SendForecastsReminderEmailsFunction> _logger;
    private readonly IProviderCourseTypesReadRepository _providerCourseTypesReadRepository;
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
    private readonly IProviderCourseForecastRepository _providerCourseForecastRepository;
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly ForecastEmailConfiguration _forecastEmailConfiguration;

    public SendForecastsReminderEmailsFunction(
        ILogger<SendForecastsReminderEmailsFunction> logger,
        IProviderCoursesReadRepository providerCoursesReadRepository,
        IProviderCourseForecastRepository providerCourseForecastRepository,
        ICourseManagementOuterApiClient courseManagementOuterApiClient,
        IOptions<ForecastEmailConfiguration> forecastEmailConfiguration,
        IProviderCourseTypesReadRepository providerCourseTypesReadRepository)
    {
        _logger = logger;
        _providerCoursesReadRepository = providerCoursesReadRepository;
        _providerCourseForecastRepository = providerCourseForecastRepository;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _forecastEmailConfiguration = forecastEmailConfiguration.Value;
        _providerCourseTypesReadRepository = providerCourseTypesReadRepository;
    }

    [Function(nameof(SendForecastsReminderEmailsFunction))]
    public async Task Run([TimerTrigger("%SendForecastsReminderEmailsFunctionSchedule%", RunOnStartup = false)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {ExecutionTime}", DateTime.Now);
        List<int> allowedProviders = await _providerCourseTypesReadRepository.GetAllProvidersWithShortCourses(cancellationToken);
        List<ProviderCourseModel> allShortCoursesCourses = await _providerCoursesReadRepository.GetAllShortCourses(cancellationToken);
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

        var batches = providersNeedingReminder.Chunk(10);

        foreach (var batch in batches) await ProcessBatch(batch);
    }

    private async Task ProcessBatch(IEnumerable<int> ukprns)
    {
        List<Task> tasks = [];
        foreach (var ukprn in ukprns)
        {
            var model = ConvertToEmailModel(ukprn, _forecastEmailConfiguration);
            tasks.Add(_courseManagementOuterApiClient.Post<ProviderEmailModel, object>($"providers/{ukprn}/email", model));
        }

        tasks.Add(Task.Delay(1000));
        await Task.WhenAll(tasks);
    }

    internal static ProviderEmailModel ConvertToEmailModel(int ukprn, ForecastEmailConfiguration configuration)
    {
        Dictionary<string, string> tokens = new()
        {
            { "ukprn", ukprn.ToString() },
            { "providercmweb", configuration.CourseManagementWebUrl },
            { "provideraccountsweb", configuration.ProviderAccountsWebUrl }
        };

        return new(configuration.ForecastPeriodicalReminderEmailTemplateId, tokens);
    }
}