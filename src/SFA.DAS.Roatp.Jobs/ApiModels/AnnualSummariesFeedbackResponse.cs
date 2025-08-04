namespace SFA.DAS.Roatp.Jobs.ApiModels;
public record AnnualSummariesFeedbackResponse(IEnumerable<FeedbackStarsSummary> EmployersFeedback, IEnumerable<FeedbackStarsSummary> ApprenticesFeedback);

public class FeedbackStarsSummary
{
    public long Ukprn { get; set; }
    public int Stars { get; set; }
    public int ReviewCount { get; set; }
}
