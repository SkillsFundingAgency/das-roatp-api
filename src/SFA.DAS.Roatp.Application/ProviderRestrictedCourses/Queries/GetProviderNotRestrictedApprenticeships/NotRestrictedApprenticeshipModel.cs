using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;

public class NotRestrictedApprenticeshipModel
{
    public string LarsCode { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }

    public static implicit operator NotRestrictedApprenticeshipModel(Standard source)
        => new()
        {
            LarsCode = source.LarsCode,
            Title = source.Title,
            Level = source.Level
        };
}
