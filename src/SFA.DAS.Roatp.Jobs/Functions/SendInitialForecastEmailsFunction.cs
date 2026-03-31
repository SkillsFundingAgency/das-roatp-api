using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class SendInitialForecastEmailsFunction
{
    private readonly ILogger<SendInitialForecastEmailsFunction> _logger;
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
    private readonly IProviderEmailProcessingService _providerEmailProcessingService;
    private readonly ForecastEmailConfiguration _forecastEmailConfiguration;

    public SendInitialForecastEmailsFunction(
        ILogger<SendInitialForecastEmailsFunction> logger,
        IProviderCoursesReadRepository providerCoursesReadRepository,
        IProviderEmailProcessingService providerEmailProcessingService,
        IOptions<ForecastEmailConfiguration> forecastEmailConfiguration)
    {
        _logger = logger;
        _providerCoursesReadRepository = providerCoursesReadRepository;
        _providerEmailProcessingService = providerEmailProcessingService;
        _forecastEmailConfiguration = forecastEmailConfiguration.Value;
    }

    [Function(nameof(SendInitialForecastEmailsFunction))]
    public async Task Run([TimerTrigger("%SendInitialForecastEmailsFunctionSchedule%", RunOnStartup = true)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# Timer trigger function executing at: {ExecutionTime}", DateTime.Now);

        List<ProviderCourse> shortCourses = await _providerCoursesReadRepository.GetShortCoursesAddedOnDate(DateTime.UtcNow.AddDays(-1).Date, cancellationToken);

        IEnumerable<ProviderEmailModel> models = shortCourses.Select(providerCourse => ConvertToEmailModel(providerCourse));
        if (models.Any()) await _providerEmailProcessingService.SendEmailsInBatches(models);
    }

    private ProviderEmailModel ConvertToEmailModel(ProviderCourse providerCourse)
    {
        Dictionary<string, string> tokens = new()
        {
            { ProviderEmailTokens.LarsCode, providerCourse.Standard.LarsCode },
            { ProviderEmailTokens.CourseName, providerCourse.Standard.Title },
            { ProviderEmailTokens.Ukprn, providerCourse.Provider.Ukprn.ToString() },
            { ProviderEmailTokens.CourseManagementWebUrl, _forecastEmailConfiguration.CourseManagementWebUrl },
            { ProviderEmailTokens.ProviderAccountWebUrl, _forecastEmailConfiguration.ProviderAccountsWebUrl }
        };

        return new(_forecastEmailConfiguration.InitialForecastEmailTemplateId, tokens);
    }
}
