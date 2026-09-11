using System;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class RestrictedApprenticeshipModel
{
    public string LarsCode { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public DateTime? LastDateStarts { get; set; }
    public bool IsClosedToNewStarts { get; set; }

    public static implicit operator RestrictedApprenticeshipModel(Standard source)
        => new()
        {
            LarsCode = source.LarsCode,
            Title = source.Title,
            Level = source.Level,
            LastDateStarts = source.LastDateStarts == DateConstants.StartRestrictedDate ? null : source.LastDateStarts
        };
}
