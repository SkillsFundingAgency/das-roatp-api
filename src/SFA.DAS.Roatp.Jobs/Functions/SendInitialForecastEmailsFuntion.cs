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
    private readonly ILogger _logger;
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly ForecastEmailConfiguration _forecastEmailConfiguration;

    public SendInitialForecastEmailsFuntion(ILoggerFactory loggerFactory, IProviderCoursesReadRepository providerCoursesReadRepository, ICourseManagementOuterApiClient courseManagementOuterApiClient, IOptions<ForecastEmailConfiguration> forecastEmailConfiguration)
    {
        _logger = loggerFactory.CreateLogger<SendInitialForecastEmailsFuntion>();
        _providerCoursesReadRepository = providerCoursesReadRepository;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _forecastEmailConfiguration = forecastEmailConfiguration.Value;
    }

    [Function(nameof(SendInitialForecastEmailsFuntion))]
    public async Task Run([TimerTrigger("%SendInitialForecastEmailsFuntionSchedule%", RunOnStartup = true)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {ExecutionTime}", DateTime.Now);

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {NextSchedule}", myTimer.ScheduleStatus.Next);
        }

        var shortCourses = await _providerCoursesReadRepository.GetShortCoursesAddedOnDate(DateTime.UtcNow.AddDays(-1).Date, cancellationToken);

        IEnumerable<Task> tasks = shortCourses.Select(course => _courseManagementOuterApiClient.Post<ProviderEmailModel, object>($"providers/{course.Provider.Ukprn}/email", ConvertToEmailModel(course, _forecastEmailConfiguration)));

        await Task.WhenAll(tasks);
    }

    internal static ProviderEmailModel ConvertToEmailModel(ProviderCourse providerCourse, ForecastEmailConfiguration configuration)
    {
        var initialForecastEmailTemplateId = configuration.InitialForecastEmailTemplateId;
        Dictionary<string, string> tokens = new()
        {
            { "larscode", providerCourse.Standard.LarsCode },
            { "coursename", providerCourse.Standard.Title },
            { "ukprn", providerCourse.Provider.Ukprn.ToString() },
            { "providercmweb", configuration.CourseManagementWebUrl },
            { "provideraccountsweb", configuration.ProviderAccountsWebUrl }
        };

        return new(initialForecastEmailTemplateId, tokens);
    }
}
