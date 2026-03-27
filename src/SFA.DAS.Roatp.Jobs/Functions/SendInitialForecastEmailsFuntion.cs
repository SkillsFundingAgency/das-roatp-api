using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class SendInitialForecastEmailsFuntion
{
    private readonly ILogger<SendInitialForecastEmailsFuntion> _logger;
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly ForecastEmailConfiguration _forecastEmailConfiguration;

    public SendInitialForecastEmailsFuntion(ILogger<SendInitialForecastEmailsFuntion> logger, IProviderCoursesReadRepository providerCoursesReadRepository, ICourseManagementOuterApiClient courseManagementOuterApiClient, IOptions<ForecastEmailConfiguration> forecastEmailConfiguration)
    {
        _logger = logger;
        _providerCoursesReadRepository = providerCoursesReadRepository;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _forecastEmailConfiguration = forecastEmailConfiguration.Value;
    }

    [Function(nameof(SendInitialForecastEmailsFuntion))]
    public async Task Run([TimerTrigger("%SendInitialForecastEmailsFuntionSchedule%", RunOnStartup = false)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {ExecutionTime}", DateTime.Now);

        List<ProviderCourse> shortCourses = await _providerCoursesReadRepository.GetShortCoursesAddedOnDate(DateTime.UtcNow.AddDays(-1).Date, cancellationToken);

        var batches = shortCourses.Chunk(10);

        foreach (var batch in batches) await ProcessBatch(batch);
    }

    private async Task ProcessBatch(IEnumerable<ProviderCourse> shortCourses)
    {
        IEnumerable<Task> tasks = shortCourses.Select(course => _courseManagementOuterApiClient.Post<ProviderEmailModel, object>($"providers/{course.Provider.Ukprn}/email", ConvertToEmailModel(course, _forecastEmailConfiguration)));
        tasks = tasks.Append(Task.Delay(1000));
        await Task.WhenAll(tasks);
    }

    internal static ProviderEmailModel ConvertToEmailModel(ProviderCourse providerCourse, ForecastEmailConfiguration configuration)
    {
        Dictionary<string, string> tokens = new()
        {
            { "larscode", providerCourse.Standard.LarsCode },
            { "coursename", providerCourse.Standard.Title },
            { "ukprn", providerCourse.Provider.Ukprn.ToString() },
            { "providercmweb", configuration.CourseManagementWebUrl },
            { "provideraccountsweb", configuration.ProviderAccountsWebUrl }
        };

        return new(configuration.InitialForecastEmailTemplateId, tokens);
    }
}
