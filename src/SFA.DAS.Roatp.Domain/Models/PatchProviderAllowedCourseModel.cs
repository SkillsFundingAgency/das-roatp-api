using System;

namespace SFA.DAS.Roatp.Domain.Models;

public class PatchProviderAllowedCourseModel
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public DateTime? LastDateStarts { get; set; }
}
