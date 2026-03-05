using System;

namespace SFA.DAS.Roatp.Domain.Entities;

public class ForecastQuarter
{
    public int Quarter { get; set; }
    public string TimePeriod { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
