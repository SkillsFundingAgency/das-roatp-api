using System;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public record ProviderAllowedCourseModel(string LarsCode, string Title, int Level, DateTime? LastDateStarts, bool IsStartRestricted, bool IsActive)
{
    private static readonly DateTime StartRestrictedDate = new(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static implicit operator ProviderAllowedCourseModel(ProviderAllowedCourse providerAllowedCourse)
    {
        return new ProviderAllowedCourseModel(
            providerAllowedCourse.LarsCode,
            providerAllowedCourse.Standard.Title,
            providerAllowedCourse.Standard.Level,
            providerAllowedCourse.LastDateStarts == StartRestrictedDate ? null : providerAllowedCourse.LastDateStarts,
            providerAllowedCourse.LastDateStarts == StartRestrictedDate,
            providerAllowedCourse.ProviderCourse != null);
    }

    public static implicit operator ProviderAllowedCourseModel(
        (Standard Standard, Domain.Entities.ProviderCourse ProviderCourse) source)
    {
        var allowedCourse = source.ProviderCourse?.ProviderAllowedCourse;

        return new ProviderAllowedCourseModel(
            source.Standard.LarsCode,
            source.Standard.Title,
            source.Standard.Level,
            allowedCourse?.LastDateStarts == StartRestrictedDate ? null : allowedCourse?.LastDateStarts,
            allowedCourse?.LastDateStarts == StartRestrictedDate,
            source.ProviderCourse != null);
    }
}
