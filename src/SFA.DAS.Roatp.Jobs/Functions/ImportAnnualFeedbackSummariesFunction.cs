using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class ImportAnnualFeedbackSummariesFunction(
    ILogger<ImportAnnualFeedbackSummariesFunction> _logger,
    IImportAnnualFeedbackSummariesService _service,
    IImportFeedbackSummariesRepository _repository)
{
    [Function(nameof(ImportAnnualFeedbackSummariesFunction))]
    public async Task Run([TimerTrigger("%ImportAnnualFeedbackSummariesFunctionSchedule%", RunOnStartup = false)] TimerInfo myTimer)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting import of annual feedback summaries");


        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Import feedback summaries function next timer schedule at: {NextSchedule}", myTimer.ScheduleStatus.Next);
        }

        var timePeriod = _service.GetTimePeriod();
        var dataExists = await _service.CheckIfDataExists(timePeriod);
        if (dataExists)
        {
            _logger.LogInformation("Data for the time period {TimePeriod} already exists. Import will not proceed.", timePeriod);
            return;
        }

        var (apprenticeStarts, employerStars) = await _service.GetFeedbackSummaries(timePeriod);

        await _repository.Import(startTime, apprenticeStarts, employerStars);

        _logger.LogInformation("Import of annual feedback summaries completed successfully.");
    }
}