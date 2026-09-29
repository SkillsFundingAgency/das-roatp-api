using System;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public record ProviderAllowedCourseModel(string LarsCode, string Title, int Level, DateTime? LastDateStarts, bool IsClosedToNewStarts)
{
    public static implicit operator ProviderAllowedCourseModel(ProviderAllowedCourse providerAllowedCourse)
    {
        return new ProviderAllowedCourseModel(
            providerAllowedCourse.LarsCode,
            providerAllowedCourse.Standard.Title,
            providerAllowedCourse.Standard.Level,
            providerAllowedCourse.LastDateStarts == DateConstants.StartRestrictedDate ? null : providerAllowedCourse.LastDateStarts,
            providerAllowedCourse.LastDateStarts < DateTime.UtcNow.Date);
    }

    public static implicit operator ProviderAllowedCourseModel(Standard standard)
    {
        return new ProviderAllowedCourseModel(
            standard.LarsCode,
            standard.Title,
            standard.Level,
            null,
            false);
    }
}
