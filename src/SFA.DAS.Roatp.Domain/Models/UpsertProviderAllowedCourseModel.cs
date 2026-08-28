using System;

namespace SFA.DAS.Roatp.Domain.Models;

public class UpsertProviderAllowedCourseModel
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public DateTime? LastDateStarts { get; set; }
    public bool IsStartRestricted { get; set; }
}
