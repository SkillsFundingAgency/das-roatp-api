using System;

namespace SFA.DAS.Roatp.Domain.Models;

public class PutProviderAllowedCourseModel
{
    public DateTime? LastDateStarts { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
}
