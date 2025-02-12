namespace SFA.DAS.Roatp.Domain.Models;

public class ProviderLocationModel
{
    public int Ordering { get; set; }
    public LocationType LocationType { get; set; }
    public bool AtEmployer { get; set; }
    public bool DayRelease { get; set; }
    public bool BlockRelease { get; set; }

    public decimal CourseDistance { get; set; }
}
