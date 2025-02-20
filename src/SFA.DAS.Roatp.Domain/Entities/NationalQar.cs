using System;

namespace SFA.DAS.Roatp.Domain.Entities;
public class NationalQar
{
    public string TimePeriod { get; set; }
    public string Leavers { get; set; }

    public string AchievementRate { get; set; }
    public DateTime CreatedDate { get; set; }
}
