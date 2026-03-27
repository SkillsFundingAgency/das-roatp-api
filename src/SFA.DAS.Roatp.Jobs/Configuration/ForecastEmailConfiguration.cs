namespace SFA.DAS.Roatp.Jobs.Configuration;

public class ForecastEmailConfiguration
{
    public const int ForecastGraceDays = -14;
    public string InitialForecastEmailTemplateId { get; set; }
    public string ForecastPeriodicalReminderEmailTemplateId { get; set; }
    public string CourseManagementWebUrl { get; set; }
    public string ProviderAccountsWebUrl { get; set; }
}
