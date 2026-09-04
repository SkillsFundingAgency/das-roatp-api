using System;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public record ProviderAllowedCourseModel(string LarsCode, string Title, int Level, DateTime? LastDateStarts, bool IsStartRestricted, bool IsActive)
{
    public static implicit operator ProviderAllowedCourseModel(ProviderAllowedCourse providerAllowedCourse)
    {
        return new ProviderAllowedCourseModel(
            providerAllowedCourse.LarsCode,
            providerAllowedCourse.Standard.Title,
            providerAllowedCourse.Standard.Level,
            providerAllowedCourse.LastDateStarts == DateConstants.StartRestrictedDate ? null : providerAllowedCourse.LastDateStarts,
            providerAllowedCourse.LastDateStarts == DateConstants.StartRestrictedDate,
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
            allowedCourse?.LastDateStarts == DateConstants.StartRestrictedDate ? null : allowedCourse?.LastDateStarts,
            allowedCourse?.LastDateStarts == DateConstants.StartRestrictedDate,
            source.ProviderCourse != null);
    }
}
