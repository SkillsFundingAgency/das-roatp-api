using System;

namespace SFA.DAS.Roatp.Domain.Entities;

public class ProviderCourseForecast
{
    public int Id { get; set; }
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string TimePeriod { get; set; }
    public int Quarter { get; set; }
    public int EstimatedLearners { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
